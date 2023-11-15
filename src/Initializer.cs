namespace Weapon_Restrict
{
    public static class Initializer
    {
        public static Config LoadConfig()
        {
            return new Config
            {
                Tag = "{GOLD}[WeaponRestrict] ",
                DestinationTypeRestrictMessage = 1,
                DestinationTypeRefundMessage = 1,

                RestrictMessageStatus = true,
                RestrictMessageText = "{TAG}{DEFAULT}Weapon {OLIVE}{WEAPON}{DEFAULT} is {DARKRED}restricted{DEFAULT} to {GREEN}{COUNT}{DEFAULT} at the moment",

                RefundMessageStatus = true,
                RefundMessage = "{TAG}{DEFAULT}Money refunded {GOLD}{MONEY}",

                RestrictMethod = 2,
                AdminImmunityFlag = "@css/root"
            };
        }

        public static List<RestrictConfig> LoadRestrictions()
        {
            return new List<RestrictConfig>
            {
                new()
                {
                    Weapon = "weapon_ak47",
                    WeaponQuota = new Dictionary<string, int>{ { "CT", 4 }, { "T", 2 } },
                    PlayersInTeamQuota = new Dictionary<string, List<Dictionary<string, int>>>
                    {
                        { "CT", new List<Dictionary<string, int>> { new(){ { "4", 1 }, { "8", 2 }, { "16", 3 } } }},
                        { "T", new List<Dictionary<string, int>> { new(){ { "4", 1 }, { "8", 2 }, { "16", 3 } } }}
                    },
                    PlayersAllQuota = new Dictionary<string, int> { { "4", 1 }, { "8", 2 }, { "16", 3 } }
                },
                new()
                {
                    Weapon = "weapon_deagle",
                    WeaponQuota = new Dictionary<string, int> { { "CT", 0 }, { "T", 0 } },
                    PlayersInTeamQuota = new Dictionary<string, List<Dictionary<string, int>>>
                    {
                        { "CT", new List<Dictionary<string, int>> { new(){ { "4", 0 }, { "8", 0 }, { "16", 0 } } }},
                        { "T", new List<Dictionary<string, int>> { new(){ { "4", 0 }, { "8", 0 }, { "16", 0 } } }}
                    },
                    PlayersAllQuota = new Dictionary<string, int> { { "4", 1 }, { "8", 2 }, { "16", 3 } }
                }
            };
        }
    }    
}
