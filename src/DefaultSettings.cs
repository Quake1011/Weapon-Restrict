using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace Weapon_Restrict
{
	public static class DefaultSettings
	{
		public static List<Weapon> GetDefault()
		{
			return new List<Weapon>
			{
				new()
				{
					DefIndex = (long)ItemDefinition.M4A4,
					Name = "M4A1",
					WeaponName = "weapon_m4a1",
					CountCT = -1,
					CountT = -1,
					Price = 3100,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.M4A1_S,
					Name = "M4A1-S",
					WeaponName = "weapon_m4a1_silencer",
					CountCT = -1,
					CountT = -1,
					Price = 2900,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.FAMAS,
					Name = "Famas",
					WeaponName = "weapon_famas",
					CountCT = -1,
					CountT = -1,
					Price = 2050,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.AUG,
					Name = "AUG",
					WeaponName = "weapon_aug",
					CountCT = -1,
					CountT = -1,
					Price = 3300,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.AK_47,
					Name = "AK-47",
					WeaponName = "weapon_ak47",
					CountCT = -1,
					CountT = -1,
					Price = 2700,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.GALIL_AR,
					Name = "Galil",
					WeaponName = "weapon_galilar",
					CountCT = -1,
					CountT = -1,
					Price = 1800,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.SG_553,
					Name = "Sg553",
					WeaponName = "weapon_sg556",
					CountCT = -1,
					CountT = -1,
					Price = 3000,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.SCAR_20,
					Name = "Scar-20",
					WeaponName = "weapon_scar20",
					CountCT = -1,
					CountT = -1,
					Price = 5000,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.AWP,
					Name = "AWP",
					WeaponName = "weapon_awp",
					CountCT = -1,
					CountT = -1,
					Price = 4750,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.SSG_08,
					Name = "SSG08",
					WeaponName = "weapon_ssg08",
					CountCT = -1,
					CountT = -1,
					Price = 1700,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.G3SG1,
					Name = "G3SG1",
					WeaponName = "weapon_g3sg1",
					CountCT = -1,
					CountT = -1,
					Price = 5000,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.MP9,
					Name = "MP9",
					WeaponName = "weapon_mp9",
					CountCT = -1,
					CountT = -1,
					Price = 1250,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.MP7,
					Name = "MP7",
					WeaponName = "weapon_mp7",
					CountCT = -1,
					CountT = -1,
					Price = 1500,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.MP5_SD,
					Name = "MP5-SD",
					WeaponName = "weapon_mp5sd",
					CountCT = -1,
					CountT = -1,
					Price = 1500,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.UMP_45,
					Name = "UMP-45",
					WeaponName = "weapon_ump45",
					CountCT = -1,
					CountT = -1,
					Price = 1200,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.P90,
					Name = "P-90",
					WeaponName = "weapon_p90",
					CountCT = -1,
					CountT = -1,
					Price = 2350,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.PP_BIZON,
					Name = "PP-19 Bizon",
					WeaponName = "weapon_bizon",
					CountCT = -1,
					CountT = -1,
					Price = 1400,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.MAC_10,
					Name = "MAC-10",
					WeaponName = "weapon_mac10",
					CountCT = -1,
					CountT = -1,
					Price = 1050,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.USP_S,
					Name = "USP-S",
					WeaponName = "weapon_usp_silencer",
					CountCT = -1,
					CountT = -1,
					Price = 200,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.P2000,
					Name = "P2000",
					WeaponName = "weapon_hkp2000",
					CountCT = -1,
					CountT = -1,
					Price = 200,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.GLOCK_18,
					Name = "Glock-18",
					WeaponName = "weapon_glock",
					CountCT = -1,
					CountT = -1,
					Price = 200,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.DUAL_BERETTAS,
					Name = "Dual berettas",
					WeaponName = "weapon_elite",
					CountCT = -1,
					CountT = -1,
					Price = 300,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.P250,
					Name = "P250",
					WeaponName = "weapon_p250",
					CountCT = -1,
					CountT = -1,
					Price = 300,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.FIVE_SEVEN,
					Name = "Five-SeveN",
					WeaponName = "weapon_fiveseven",
					CountCT = -1,
					CountT = -1,
					Price = 500,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.CZ75_AUTO,
					Name = "CZ75-Auto",
					WeaponName = "weapon_cz75a",
					CountCT = -1,
					CountT = -1,
					Price = 500,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.TEC_9,
					Name = "Tec-9",
					WeaponName = "weapon_tec9",
					CountCT = -1,
					CountT = -1,
					Price = 500,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.R8_REVOLVER,
					Name = "Revolver R8",
					WeaponName = "weapon_revolver",
					CountCT = -1,
					CountT = -1,
					Price = 600,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.DESERT_EAGLE,
					Name = "Desert Eagle",
					WeaponName = "weapon_deagle",
					CountCT = -1,
					CountT = -1,
					Price = 700,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.NOVA,
					Name = "Nova",
					WeaponName = "weapon_nova",
					CountCT = -1,
					CountT = -1,
					Price = 1050,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.XM1014,
					Name = "XM1014",
					WeaponName = "weapon_xm1014",
					CountCT = -1,
					CountT = -1,
					Price = 2000,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.MAG_7,
					Name = "MAG-7",
					WeaponName = "weapon_mag7",
					CountCT = -1,
					CountT = -1,
					Price = 1300,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.SAWED_OFF,
					Name = "Sawed-off",
					WeaponName = "weapon_sawedoff",
					CountCT = -1,
					CountT = -1,
					Price = 1100,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.M249,
					Name = "M429",
					WeaponName = "weapon_m249",
					CountCT = -1,
					CountT = -1,
					Price = 5200,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.NEGEV,
					Name = "Negev",
					WeaponName = "weapon_negev",
					CountCT = -1,
					CountT = -1,
					Price = 1700,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.ZEUS_X27,
					Name = "Zeus x27",
					WeaponName = "weapon_taser",
					CountCT = -1,
					CountT = -1,
					Price = 200,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.HIGH_EXPLOSIVE_GRENADE,
					Name = "High Explosive Grenade",
					WeaponName = "weapon_hegrenade",
					CountCT = -1,
					CountT = -1,
					Price = 300,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.MOLOTOV,
					Name = "Molotov",
					WeaponName = "weapon_molotov",
					CountCT = -1,
					CountT = -1,
					Price = 400,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.INCENDIARY_GRENADE,
					Name = "Incendiary Grenade",
					WeaponName = "weapon_incgrenade",
					CountCT = -1,
					CountT = -1,
					Price = 600,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.SMOKE_GRENADE,
					Name = "Smoke Grenade",
					WeaponName = "weapon_smokegrenade",
					CountCT = -1,
					CountT = -1,
					Price = 300,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.FLASHBANG,
					Name = "Flashbang",
					WeaponName = "weapon_flashbang",
					CountCT = -1,
					CountT = -1,
					Price = 200,
					RefundMessage = true
				},
				new()
				{
					DefIndex = (long)ItemDefinition.DECOY_GRENADE,
					Name = "Decoy Grenade",
					WeaponName = "weapon_decoy",
					CountCT = -1,
					CountT = -1,
					Price = 50
				}
			};
		}
	}
}
