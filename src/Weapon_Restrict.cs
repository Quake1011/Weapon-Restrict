using System.Text.Json;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;

namespace Weapon_Restrict;
public class WeaponRestrict : BasePlugin
{
    public override string ModuleName => "Weapon Restrict";
    public override string ModuleVersion => "1.0";
    public override string ModuleAuthor => "Quake1011";
    public override string ModuleDescription => "Prohibits purchase or picking up certain weapons";
    private const string MyLink = "https://github.com/Quake1011";

    private List<Weapon> _weaponList = new();

    public override void Load(bool hotReload)
    {
	    if (Api.GetVersion() < 10)
	    {
		    AddTimer(5.0f, Informer, TimerFlags.REPEAT);
		    return;
	    }
	    RegisterEventHandler<EventItemPurchase>(OnEventItemPurchasePost);
	    RegisterEventHandler<EventItemPickup>(OnEventItemPickupPost);
	    PrintInfo();
	    _weaponList = LoadConfig();
    }

    private static void Informer() { Server.PrintToConsole("The server needs to be updated to Counter Strike Sharp version no lower than 10"); }
    
    [GameEventHandler(mode: HookMode.Post)]
    private HookResult OnEventItemPurchasePost(EventItemPurchase @event, GameEventInfo info)
    {
	    var restrictedWeapon = _weaponList.FirstOrDefault(w => w.WeaponName == @event.Weapon);
	    if (restrictedWeapon == null) return HookResult.Continue;

	    if (RestrictedWeaponCheck(restrictedWeapon.DefIndex, @event.Userid.TeamNum) == false)
		    return HookResult.Continue;

	    var refunded = restrictedWeapon.Price;
	    @event.Userid.InGameMoneyServices!.Account += refunded;
	    if (restrictedWeapon.RefundMessage) @event.Userid.PrintToChat($"Money refunded {ChatColors.Gold}{refunded}");
	    return HookResult.Continue;
    }

    [GameEventHandler(mode: HookMode.Pre)]
    private HookResult OnEventItemPickupPost(EventItemPickup @event, GameEventInfo info)
    {
	    if (RestrictedWeaponCheck(@event.Defindex, @event.Userid.TeamNum) == false) return HookResult.Continue;

		var restrictedWeapon = _weaponList.FirstOrDefault(w => w.DefIndex == @event.Defindex);
		if (restrictedWeapon == null) return HookResult.Continue;

		foreach (var ownerWeapon in @event.Userid.PlayerPawn.Value.WeaponServices!.MyWeapons)
		{
			if (ownerWeapon is not { IsValid: true, Value.IsValid: true }) continue;
			if (ownerWeapon.Value.AttributeManager.Item.ItemDefinitionIndex != @event.Defindex) continue;
			
			ownerWeapon.Value.Remove();
		    @event.Userid.PrintToChat($"Weapon {restrictedWeapon.Name} is {ChatColors.Darkred}restricted{ChatColors.Default} to {ChatColors.Green}{(@event.Userid.TeamNum == 3 ?restrictedWeapon.CountCT : restrictedWeapon.CountT)}{ChatColors.Default} at the moment");
		    
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
    
    private bool RestrictedWeaponCheck(long defIndex, int team)
    {
        var counter = 0;
        for (var i = 0; i < Server.MaxPlayers; i++)
        {
            CCSPlayerController player = new(NativeAPI.GetEntityFromIndex(i));
            if (player is not { IsValid: true, PawnIsAlive: true } || player.TeamNum != team) continue;

            counter += player.PlayerPawn.Value.WeaponServices!.MyWeapons.Where(ownerWeapon => ownerWeapon is
	            {
		            IsValid: true,
		            Value.IsValid: true
	            })
	            .Count(ownerWeapon => defIndex == ownerWeapon.Value.AttributeManager.Item.ItemDefinitionIndex);
        }

        if ((team == 3 && _weaponList.FirstOrDefault(w => w.DefIndex == defIndex)?.CountCT == -1) || (team == 2 && _weaponList.FirstOrDefault(w => w.DefIndex == defIndex)?.CountT == -1))
            return false;

        return (team == 3 && _weaponList.FirstOrDefault(w => w.DefIndex == defIndex)?.CountCT < counter) ||
               (team == 2 && _weaponList.FirstOrDefault(w => w.DefIndex == defIndex)?.CountT < counter);
    }

    private List<Weapon> LoadConfig()
    {
        var configPath = Path.Join(ModuleDirectory, "weapons.json");
        return !File.Exists(configPath)
            ? CreateConfig(configPath)
            : JsonSerializer.Deserialize<List<Weapon>>(File.ReadAllText(configPath))!;
    }

    private static List<Weapon> CreateConfig(string configPath)
    {
        var weapons = DefaultSettings.GetDefault();
        File.WriteAllText(configPath,
            JsonSerializer.Serialize(weapons,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));
        return weapons;
    }
}

public class Weapon
{
	public string? WeaponName { get; init; }
	public string? Name { get; init; }
	public int CountT { get; init; }
	public int CountCT { get; init; }
	public int Price { get; init; }
	public long DefIndex { get; init; }
	public bool RefundMessage { get; init; }
}
