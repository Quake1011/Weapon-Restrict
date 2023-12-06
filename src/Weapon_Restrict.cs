using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace Weapon_Restrict;

[MinimumApiVersion(53)]
public class WeaponRestrict : BasePlugin
{
	public override string ModuleName => "Weapon Restrict";
    public override string ModuleVersion => "1.2";
    public override string ModuleAuthor => "Quake1011";
    public override string ModuleDescription => "Prohibits purchase or picking up restricted weapons";
    private const string MyLink = "https://github.com/Quake1011";
    
    private List<WeaponMeta> _weaponList = new();
    private List<RestrictConfig>? _restrictions = new();
    private static Config? _config = new();

    public override void Load(bool hotReload)
    {
	    RegisterEventHandler<EventItemPurchase>(OnEventItemPurchasePost);
	    RegisterEventHandler<EventItemPickup>(OnEventItemPickupPost);
	    RegisterEventHandler<EventGameStart>(OnEventGameStart);
	    
	    PrintInfo();
	    
	    _weaponList = MetaData.Load();
	    LoadConfigs();
    }

    [GameEventHandler(mode: HookMode.Post)]
    private HookResult OnEventGameStart(EventGameStart @event, GameEventInfo info)
    {
	    ConVar.Find("mp_weapons_max_gun_purchases_per_weapon_per_match")?.SetValue(-1);
	    return HookResult.Continue;
    }
    
    [GameEventHandler(mode: HookMode.Post)]
    private HookResult OnEventItemPurchasePost(EventItemPurchase @event, GameEventInfo info)
    {
	    if (IsAdmin(@event.Userid)) 
		    return HookResult.Continue;
	    
	    var restrictedWeapon = _weaponList.FirstOrDefault(w => w.WeaponName == @event.Weapon);
	    if (restrictedWeapon == null) 
		    return HookResult.Continue;

	    var checking = RestrictedWeaponCheck(restrictedWeapon.DefIndex, @event.Userid.TeamNum);
	    
	    if (checking.ReturnedResult == false)
		    return HookResult.Continue;

	    var refunded = restrictedWeapon.Price;
	    @event.Userid.InGameMoneyServices!.Account += refunded;
	    if (!_config!.RefundMessageStatus) 
		    return HookResult.Continue;
	    
	    var message = _config.RefundMessage;
	    message = message?.Replace("{TAG}", _config.Tag);
	    message = message?.Replace("{WEAPON}", restrictedWeapon.Name);
	    message = message?.Replace("{MONEY}", $"{refunded}");
	    message = ReplaceTags(message!);
	    switch (_config.DestinationTypeRefundMessage)
	    {
		    case (int)PrintType.Chat:
			    @event.Userid.PrintToChat(" " + message);
			    break;
		    case (int)PrintType.Html:
			    @event.Userid.PrintToCenterHtml(message);
			    break;
		    case (int)PrintType.Center:
			    @event.Userid.PrintToCenter(message);
			    break;
	    }

	    return HookResult.Continue;
    }

    [GameEventHandler(mode: HookMode.Pre)]
    private HookResult OnEventItemPickupPost(EventItemPickup @event, GameEventInfo info)
    {
	    if (IsAdmin(@event.Userid)) 
		    return HookResult.Continue;
	    
	    var checking = RestrictedWeaponCheck(@event.Defindex, @event.Userid.TeamNum);
	    
	    if (checking.ReturnedResult == false)
		    return HookResult.Continue;
	    
		var restrictedWeapon = _weaponList.FirstOrDefault(w => w.DefIndex == @event.Defindex);
		
		if (restrictedWeapon == null) 
			return HookResult.Continue;

		foreach (var ownerWeapon in @event.Userid.PlayerPawn.Value!.WeaponServices!.MyWeapons)
		{
			if (ownerWeapon is not { IsValid: true, Value.IsValid: true }) 
				continue;
			if (ownerWeapon.Value.AttributeManager.Item.ItemDefinitionIndex != @event.Defindex) 
				continue;

			var tempNum = 0;
			
			switch (ownerWeapon.Value.AttributeManager.Item.ItemDefinitionIndex)
			{
				case (int)ItemDefinition.FLASHBANG:
					tempNum += @event.Userid.PlayerPawn.Value.WeaponServices.Ammo[(int)GrenadesPos.FlashAmmo];
					break;
				case (ushort)ItemDefinition.HIGH_EXPLOSIVE_GRENADE:
				case (ushort)ItemDefinition.FRAG_GRENADE:
					tempNum += @event.Userid.PlayerPawn.Value.WeaponServices.Ammo[(int)GrenadesPos.HegrenadeAmmo];
					break;
				case (ushort)ItemDefinition.DECOY_GRENADE:
					tempNum += @event.Userid.PlayerPawn.Value.WeaponServices.Ammo[(int)GrenadesPos.DecoyAmmo];
					break;
				case (ushort)ItemDefinition.SMOKE_GRENADE:
					tempNum += @event.Userid.PlayerPawn.Value.WeaponServices.Ammo[(int)GrenadesPos.SmokeAmmo];
					break;
				case (ushort)ItemDefinition.MOLOTOV:
				case (ushort)ItemDefinition.INCENDIARY_GRENADE:
					tempNum += @event.Userid.PlayerPawn.Value.WeaponServices.Ammo[(int)GrenadesPos.IncAmmo];
					break;
			}
			
			ownerWeapon.Value.Remove();
			
			tempNum--;
			for (var i = tempNum; i > 0; i--)
				@event.Userid.GiveNamedItem(restrictedWeapon.WeaponName!);

			if (_config!.RestrictMessageStatus)
			{
				var message = _config.RestrictMessageText;
				message = message?.Replace("{TAG}", _config.Tag);
				message = message?.Replace("{WEAPON}", restrictedWeapon.Name);
				message = message?.Replace("{COUNT}", $"{checking.ReturnedCount}");
				message = ReplaceTags(message!);

				var printMethods = new Dictionary<int, Action<string, CCSPlayerController>> 
				{
					{(int)PrintType.Chat, (msg, user) => user.PrintToChat(" " + msg)},
					{(int)PrintType.Html, (msg, user) => user.PrintToCenterHtml(msg)},
					{(int)PrintType.Center, (msg, user) => user.PrintToCenter(msg)} 
				};

				printMethods[_config.DestinationTypeRestrictMessage](message, @event.Userid);
			}
		    
			@event.Userid.ExecuteClientCommand("lastinv");

			return HookResult.Continue;
		}

		return HookResult.Continue;
    }

    private void PrintInfo()
    {
        Server.PrintToConsole(" ");
        Server.PrintToConsole("#####################################");
        Server.PrintToConsole($"Plugin name - {ModuleName} [v{ModuleVersion}]");
        Server.PrintToConsole($"Author - {ModuleAuthor}");
        Server.PrintToConsole($"Description - {ModuleDescription}");
        Server.PrintToConsole($"Github - {MyLink}");
        Server.PrintToConsole("#####################################");
        Server.PrintToConsole(" ");
    }
    
    private Result RestrictedWeaponCheck(long defIndex, int team)
    {
	    var res = new Result { ReturnedCount = 0, ReturnedResult = true };
	    
	    var wpn = _restrictions?.FirstOrDefault(k =>
		    k.Weapon == _weaponList.FirstOrDefault(w => w.DefIndex == defIndex)?.WeaponName);

	    if (wpn == null)
	    {
		    res.ReturnedResult = false;
		    return res;
	    }

	    switch (_config?.RestrictMethod)
	    {
		    case (int)Method.Count:
		    {
			    var weapons = 0;
			    for (var i = 0; i < Server.MaxPlayers; i++)
			    {
				    CCSPlayerController player = new(NativeAPI.GetEntityFromIndex(i));
				    if (player is not { IsValid: true, PawnIsAlive: true } || player.TeamNum != team) 
					    continue;

				    var playerWeapons = player.PlayerPawn.Value!.WeaponServices!.MyWeapons;

				    weapons += playerWeapons.Sum(wn => GetCount(defIndex, wn, player));
			    }
			    
			    if (wpn.WeaponQuota != null)
			    {
				    res.ReturnedResult = (team == 3 && wpn.WeaponQuota["CT"] < weapons) ||
				                         (team == 2 && wpn.WeaponQuota["T"] < weapons);
				    res.ReturnedCount = team switch
				    {
					    2 => wpn.WeaponQuota["T"],
					    3 => wpn.WeaponQuota["CT"],
					    _ => res.ReturnedCount
				    };
			    }
			    break;
		    }
		    case (int)Method.PlayersTeam:
		    {
			    var totalTeamPlayers = Utilities.GetPlayers().Where(pl => pl.TeamNum == team).ToList();

			    var sameWeapons = (from player in totalTeamPlayers from playerWeapon in player.PlayerPawn.Value!.WeaponServices!.MyWeapons select GetCount(defIndex, playerWeapon, player)).Sum();

			    var playerDictByTeam = new List<Dictionary<string, int>>();
			    switch (team)
			    {
				    case 3:
					    playerDictByTeam = wpn.PlayersInTeamQuota?["CT"];
					    break;
				    case 2:
					    playerDictByTeam = wpn.PlayersInTeamQuota?["T"];
					    break;
			    }

			    if (playerDictByTeam != null)
			    {
				    foreach (var quota in from line in playerDictByTeam from quota in line where Convert.ToInt32(quota.Key) <= totalTeamPlayers.Count select quota)
					    res.ReturnedCount = quota.Value;

				    if (res.ReturnedCount >= sameWeapons)
					    res.ReturnedResult = false;
			    }
			    break;
		    }
		    case (int)Method.PlayersAll:
		    {
			    var activePlayers = Utilities.GetPlayers().Where(pl => pl.TeamNum > 1).ToList();

			    var sameWeapons = activePlayers.Sum(player => player.PlayerPawn.Value!.WeaponServices!.MyWeapons.Sum(playerWeapon => GetCount(defIndex, playerWeapon, player)));

			    var myList = wpn.PlayersAllQuota;

			    if (myList != null)
			    {
				    foreach (var line in myList.Where(line => Convert.ToInt32(line.Key) <= activePlayers.Count)) 
					    res.ReturnedCount = line.Value;

				    if (res.ReturnedCount >= sameWeapons)
						res.ReturnedResult = false;
			    }

			    break;
		    }
	    }

	    return res;
    }

    private static int GetCount(long defIndex, CHandle<CBasePlayerWeapon> wn, CCSPlayerController player)
    {
	    var total = 0;
	    if (wn is not { IsValid: true, Value.IsValid: true }) 
		    return total;
	    if (wn.Value.AttributeManager.Item.ItemDefinitionIndex != defIndex) 
		    return total;
	    
	    switch (wn.Value.AttributeManager.Item.ItemDefinitionIndex)
	    {
		    case (ushort)ItemDefinition.FRAG_GRENADE:
		    case (ushort)ItemDefinition.HIGH_EXPLOSIVE_GRENADE:
			    total += player.PlayerPawn.Value!.WeaponServices!.Ammo[(int)GrenadesPos.HegrenadeAmmo];
			    break;
		    case (ushort)ItemDefinition.FLASHBANG:
			    total += player.PlayerPawn.Value!.WeaponServices!.Ammo[(int)GrenadesPos.FlashAmmo];
			    break;
		    case (ushort)ItemDefinition.SMOKE_GRENADE:
			    total += player.PlayerPawn.Value!.WeaponServices!.Ammo[(int)GrenadesPos.SmokeAmmo];
			    break;
		    case (ushort)ItemDefinition.MOLOTOV:
		    case (ushort)ItemDefinition.INCENDIARY_GRENADE:
			    total += player.PlayerPawn.Value!.WeaponServices!.Ammo[(int)GrenadesPos.IncAmmo];
			    break;
		    case (ushort)ItemDefinition.DECOY_GRENADE:
			    total += player.PlayerPawn.Value!.WeaponServices!.Ammo[(int)GrenadesPos.DecoyAmmo];
			    break;
		    default:
			    total++;
			    break;
	    }

	    return total;
    }
    
    // public static void DropPlayerWeapon(CCSPlayerController player, CBasePlayerWeapon weapon)
    // {
	   //  foreach (var wpn in player.PlayerPawn.Value.WeaponServices!.MyWeapons)
	   //  {
		  //   if (weapon.Handle != wpn.Handle) continue;
		  //   var service = new CCSPlayer_ItemServices(player.PlayerPawn.Value.ItemServices!.Handle);
		  //   var dropWeapon = VirtualFunction.CreateVoid<nint, nint>(service.Handle, 19);
		  //   dropWeapon(service.Handle, weapon.Handle);
	   //  }
    // }
    
    private void LoadConfigs()
    {
	    var configPath = Path.Join(ModuleDirectory, "Config.json");
	    _config = !File.Exists(configPath)
		    ? CreateAndWriteConfig(configPath, Initializer.LoadConfig)
		    : JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath));
	    
	    configPath = Path.Join(ModuleDirectory, "RestrictConfig.json");
	    _restrictions = !File.Exists(configPath)
		    ? CreateAndWriteConfig(configPath, Initializer.LoadRestrictions)
		    : JsonSerializer.Deserialize<List<RestrictConfig>>(File.ReadAllText(configPath));
    }

    private static T CreateAndWriteConfig<T>(string configPath, Func<T> dataLoader)
    {
	    var data = dataLoader();
	    File.WriteAllText(configPath,
		    JsonSerializer.Serialize(data,
			    new JsonSerializerOptions
			    {
				    WriteIndented = true,
				    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			    }));
	    return data;
    }

    private static string ReplaceTags(string text)
    {
	    switch (_config!.DestinationTypeRestrictMessage)
	    {
		    case (byte)PrintType.Html:
			    text = text.Replace("{DEFAULT}", $"{HtmlColors.White}");
			    text = text.Replace("{WHITE}", $"{HtmlColors.WhiteBlue}");
			    text = text.Replace("{DARKRED}", $"{HtmlColors.DarkRed}");
			    text = text.Replace("{GREEN}", $"{HtmlColors.Green}");
			    text = text.Replace("{LIGHTYELLOW}", $"{HtmlColors.LightYellow}");
			    text = text.Replace("{LIGHTBLUE}", $"{HtmlColors.LightBlue}");
			    text = text.Replace("{OLIVE}", $"{HtmlColors.Olive}");
			    text = text.Replace("{LIME}", $"{HtmlColors.Lime}");
			    text = text.Replace("{RED}", $"{HtmlColors.Red}");
			    text = text.Replace("{PURPLE}", $"{HtmlColors.Purple}");
			    text = text.Replace("{GREY}", $"{HtmlColors.GrayWolf}");
			    text = text.Replace("{YELLOW}", $"{HtmlColors.Yellow}");
			    text = text.Replace("{GOLD}", $"{HtmlColors.Gold}");
			    text = text.Replace("{SILVER}", $"{HtmlColors.Silver}");
			    text = text.Replace("{BLUE}", $"{HtmlColors.Blue}");
			    text = text.Replace("{DARKBLUE}", $"{HtmlColors.DarkBlue}");
			    text = text.Replace("{BLUEGREY}", $"{HtmlColors.GrayCloud}");
			    text = text.Replace("{MAGENTA}", $"{HtmlColors.MagentaPink}");
			    text = text.Replace("{LIGHTRED}", $"{HtmlColors.LightRed}");
			    break;
		    case (byte)PrintType.Chat:
			    text = text.Replace("{DEFAULT}", $"{ChatColors.Default}");
			    text = text.Replace("{WHITE}", $"{ChatColors.White}");
			    text = text.Replace("{DARKRED}", $"{ChatColors.Darkred}");
			    text = text.Replace("{GREEN}", $"{ChatColors.Green}");
			    text = text.Replace("{LIGHTYELLOW}", $"{ChatColors.LightYellow}");
			    text = text.Replace("{LIGHTBLUE}", $"{ChatColors.LightBlue}");
			    text = text.Replace("{OLIVE}", $"{ChatColors.Olive}");
			    text = text.Replace("{LIME}", $"{ChatColors.Lime}");
			    text = text.Replace("{RED}", $"{ChatColors.Red}");
			    text = text.Replace("{PURPLE}", $"{ChatColors.Purple}");
			    text = text.Replace("{GREY}", $"{ChatColors.Grey}");
			    text = text.Replace("{YELLOW}", $"{ChatColors.Yellow}");
			    text = text.Replace("{GOLD}", $"{ChatColors.Gold}");
			    text = text.Replace("{SILVER}", $"{ChatColors.Silver}");
			    text = text.Replace("{BLUE}", $"{ChatColors.Blue}");
			    text = text.Replace("{DARKBLUE}", $"{ChatColors.DarkBlue}");
			    text = text.Replace("{BLUEGREY}", $"{ChatColors.BlueGrey}");
			    text = text.Replace("{MAGENTA}", $"{ChatColors.Magenta}");
			    text = text.Replace("{LIGHTRED}", $"{ChatColors.LightRed}");
			    break;
		    case (byte)PrintType.Center:
			    text = text.Replace("{DEFAULT}", "");
			    text = text.Replace("{WHITE}", "");
			    text = text.Replace("{DARKRED}", "");
			    text = text.Replace("{GREEN}", "");
			    text = text.Replace("{LIGHTYELLOW}", "");
			    text = text.Replace("{LIGHTBLUE}", "");
			    text = text.Replace("{OLIVE}", "");
			    text = text.Replace("{LIME}", "");
			    text = text.Replace("{RED}", "");
			    text = text.Replace("{PURPLE}", "");
			    text = text.Replace("{GREY}", "" );
			    text = text.Replace("{YELLOW}", "");
			    text = text.Replace("{GOLD}", "" );
			    text = text.Replace("{SILVER}", "");
			    text = text.Replace("{BLUE}", "" );
			    text = text.Replace("{DARKBLUE}", "");
			    text = text.Replace("{BLUEGREY}", "");
			    text = text.Replace("{MAGENTA}", "");
			    text = text.Replace("{LIGHTRED}", "");
			    break;
	    }

	    return text;
    }

    private enum PrintType
    {
	    Chat = 1,
	    Html,
	    Center
    }

    private enum GrenadesPos
    {
	    HegrenadeAmmo = 13,
	    FlashAmmo = 14,
	    SmokeAmmo = 15,
		IncAmmo = 16,
		DecoyAmmo = 17,
    }
    
    private enum Method
    {
	    PlayersTeam = 1,
	    Count,
	    PlayersAll
    }

    private static bool IsAdmin(CCSPlayerController player)
    {
	    return _config?.AdminImmunityFlag != null && AdminManager.PlayerHasPermissions(player, _config.AdminImmunityFlag);
    }
}

public class WeaponMeta
{
	public string? WeaponName { get; init; }
	public string? Name { get; init; }
	public int Price { get; init; }
	public long DefIndex { get; init; }
}

public class Config
{
	public string? Tag { get; init; }
	public int DestinationTypeRestrictMessage { get; init; }
	public int DestinationTypeRefundMessage { get; init; }
	public string? RestrictMessageText { get; init; }
	public bool RestrictMessageStatus { get; init; }
	public string? RefundMessage { get; init; }
	public bool RefundMessageStatus { get; init; }
	public int RestrictMethod { get; init; }
	public string? AdminImmunityFlag { get; init; }
}

public class RestrictConfig
{
	public string? Weapon { get; init; }
	public Dictionary<string, int>? WeaponQuota { get; init; }
	public Dictionary<string, List<Dictionary<string, int>>>? PlayersInTeamQuota { get; init; }
	public Dictionary<string, int>? PlayersAllQuota { get; init; }
}

public class Result
{
	public bool ReturnedResult { get; set; }
	public int ReturnedCount { get; set; }
}