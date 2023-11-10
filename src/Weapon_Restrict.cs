using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Timers;

namespace Weapon_Restrict;
public class WeaponRestrict : BasePlugin
{
	public override string ModuleName => "Weapon Restrict";
    public override string ModuleVersion => "1.1.1";
    public override string ModuleAuthor => "Quake1011";
    public override string ModuleDescription => "Prohibits purchase or picking up restricted weapons";
    private const string MyLink = "https://github.com/Quake1011";

    private const int MinVersion = 28;
    private List<WeaponMeta> _weaponList = new();
    private List<RestrictConfig>? _restrictions = new();
    private static Config? _config = new();

    public override void Load(bool hotReload)
    {
	    if (Api.GetVersion() < MinVersion)
		    AddTimer(5.0f, Informer, TimerFlags.REPEAT);
	    else
	    {
		    RegisterEventHandler<EventItemPurchase>(OnEventItemPurchasePost);
		    RegisterEventHandler<EventItemPickup>(OnEventItemPickupPost);
		    PrintInfo();
		    
		    _weaponList = MetaData.Load();
		    LoadConfigs();
	    }
    }

    private static void Informer() { Server.PrintToConsole($"[WeaponRestrict] The server needs to be updated to Counter Strike Sharp version no lower than {MinVersion}"); }
    
    [GameEventHandler(mode: HookMode.Post)]
    private HookResult OnEventItemPurchasePost(EventItemPurchase @event, GameEventInfo info)
    {
	    if (IsAdmin(@event.Userid)) return HookResult.Continue;
	    
	    var restrictedWeapon = _weaponList.FirstOrDefault(w => w.WeaponName == @event.Weapon);
	    if (restrictedWeapon == null) return HookResult.Continue;

	    var checking = RestrictedWeaponCheck(restrictedWeapon.DefIndex, @event.Userid.TeamNum);
	    
	    if (checking.ReturnedResult == false)
		    return HookResult.Continue;

	    var refunded = restrictedWeapon.Price;
	    @event.Userid.InGameMoneyServices!.Account += refunded;
	    if (!_config!.RefundMessageStatus) return HookResult.Continue;
	    
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
	    }

	    return HookResult.Continue;
    }

    [GameEventHandler(mode: HookMode.Pre)]
    private HookResult OnEventItemPickupPost(EventItemPickup @event, GameEventInfo info)
    {
	    if (IsAdmin(@event.Userid)) return HookResult.Continue;
	    
	    var checking = RestrictedWeaponCheck(@event.Defindex, @event.Userid.TeamNum);
	    
	    if (checking.ReturnedResult == false)
		    return HookResult.Continue;
	    
		var restrictedWeapon = _weaponList.FirstOrDefault(w => w.DefIndex == @event.Defindex);
		if (restrictedWeapon == null) return HookResult.Continue;

		foreach (var ownerWeapon in @event.Userid.PlayerPawn.Value.WeaponServices!.MyWeapons)
		{
			if (ownerWeapon is not { IsValid: true, Value.IsValid: true }) continue;
			if (ownerWeapon.Value.AttributeManager.Item.ItemDefinitionIndex != @event.Defindex) continue;
			
			ownerWeapon.Value.Remove();
			if (_config!.RestrictMessageStatus)
			{
				var message = _config.RestrictMessageText;
				message = message?.Replace("{TAG}", _config.Tag);
				message = message?.Replace("{WEAPON}", restrictedWeapon.Name);
				message = message?.Replace("{COUNT}", $"{checking.ReturnedCount}");
				message = ReplaceTags(message!);
				
				switch (_config.DestinationTypeRestrictMessage)
				{
					case (int)PrintType.Chat:
						@event.Userid.PrintToChat(" " + message);
						break;
					case (int)PrintType.Html:
						@event.Userid.PrintToCenterHtml(message);
						break;
				}
			}
		    
		    NativeAPI.IssueClientCommand((int)@event.Userid.EntityIndex!.Value.Value-1, "lastinv");
		    
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
	    var res = new Result() { ReturnedCount = 0, ReturnedResult = true };
	    
	    var wpn = _restrictions?.FirstOrDefault(k => k.Weapon == _weaponList.FirstOrDefault(w => w.DefIndex == defIndex)?.WeaponName);

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
				    if (player is not { IsValid: true, PawnIsAlive: true } || player.TeamNum != team) continue;

				    weapons += player.PlayerPawn.Value.WeaponServices!.MyWeapons.Where(ownerWeapon => ownerWeapon is { IsValid: true, Value.IsValid: true }).Count(ownerWeapon => defIndex == ownerWeapon.Value.AttributeManager.Item.ItemDefinitionIndex);
			    }
			    
			    if (wpn.WeaponQuota != null)
			    {
				    res.ReturnedResult = (team == 3 && wpn.WeaponQuota["CT"] < weapons) || (team == 2 && wpn.WeaponQuota["T"] < weapons);
				    res.ReturnedCount = team switch
				    {
					    2 => wpn.WeaponQuota["T"],
					    3 => wpn.WeaponQuota["CT"],
					    _ => res.ReturnedCount
				    };
			    }
			    break;
		    }
		    case (int)Method.Players:
		    {
			    var players = 0;
			    for (var i = 0; i < Server.MaxPlayers; i++)
			    {
				    CCSPlayerController player = new(NativeAPI.GetEntityFromIndex(i));
				    if (player is not { IsValid: true, PawnIsAlive: true } || player.TeamNum != team) continue;
				    players++;
			    }

			    res.ReturnedCount = 0;
			    res.ReturnedResult = true;
			    
			    var ctr = 0;
			    foreach (var w in team == 2 ? wpn.PlayerQuota?["T"]! : wpn.PlayerQuota?["CT"]!)
			    {
				    foreach (var key in w.Keys)
				    {
					    if (players >= Convert.ToInt32(key))
					    {
						    res.ReturnedCount = ctr == 0 ? 0 : w[key];
						    res.ReturnedResult = true;
						    ctr++;
					    }
					    else break;
				    }
			    }

			    break;
		    }
	    }

	    return res;
    }
    
    private void LoadConfigs()
    {
	    var configPath = Path.Join(ModuleDirectory, "Config.json");
	    _config = !File.Exists(configPath) ? CreateConfig(configPath) : JsonSerializer.Deserialize<Config>(File.ReadAllText(configPath));
	    
	    configPath = Path.Join(ModuleDirectory, "RestrictConfig.json");
	    _restrictions = !File.Exists(configPath) ? CreateRestrictions(configPath) : JsonSerializer.Deserialize<List<RestrictConfig>>(File.ReadAllText(configPath));
    }

    private static Config CreateConfig(string configPath)
    {
        var data = Initializer.LoadConfig();
        File.WriteAllText(configPath, JsonSerializer.Serialize(data, new JsonSerializerOptions{ WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
        return data;
    }

    private static List<RestrictConfig> CreateRestrictions(string configPath)
    {
	    var data = Initializer.LoadRestrictions();
	    File.WriteAllText(configPath, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
	    return data;
    }

    private string ReplaceTags(string text)
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
	    }

	    return text;
    }

    private enum PrintType
    {
	    Chat = 1,
	    Html
    }

    private enum Method
    {
	    Players = 1,
	    Count
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
	public string? Tag { get; set; }
	public int DestinationTypeRestrictMessage { get; set; }
	public int DestinationTypeRefundMessage { get; set; }
	public string? RestrictMessageText { get; init; }
	public bool RestrictMessageStatus { get; init; }
	public string? RefundMessage { get; init; }
	public bool RefundMessageStatus { get; init; }
	// public bool RestrictSound { get; set; }
	public int RestrictMethod { get; set; }
	public string? AdminImmunityFlag { get; set; }
}

public class RestrictConfig
{
	public string? Weapon { get; init; }
	public Dictionary<string, int>? WeaponQuota { get; init; }
	public Dictionary<string, List<Dictionary<string, int>>>? PlayerQuota { get; init; }
}

public class Result
{
	public bool ReturnedResult { get; set; }
	public int ReturnedCount { get; set; }
}