# Weapon Restrict
The plugin allows to restrict certain weapons depending on the players in the teams or the number of specified weapons. It is also possible to set immunity for administrators to restriction

## Requirements
- [Metamod](https://www.sourcemm.net/downloads.php/?branch=master)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases/tag/v42) >= v42

## Install
- Drop folder `build/Weapon_Restrict` to `addons/counterstrikesharp/plugins/`

- Configuration files will be generated after the plugin is launched in the folder `addons/counterstrikesharp/plugins/Weapon_Restrict`:
	- `Config.json`				- Common settings
	- `RestrictConfig.json`		- Restrictions settings

## Configs

### Config.json
```
{
	"Tag": "{GOLD}[WeaponRestrict] ",		// Tag before text of plugin
	"DestinationTypeRestrictMessage": 1,	// Specifies how the prohibition message will be displayed	[ 1 - chat | 2 - center]
	"DestinationTypeRefundMessage": 1,	// Specifies how the refund message will be displayed
	"RestrictMessageText": "{TAG}{DEFAULT}Weapon {OLIVE}{WEAPON}{DEFAULT} is {DARKRED}restricted{DEFAULT} to {GREEN}{COUNT}{DEFAULT} at the moment", // Restrict message
	"RestrictMessageStatus": true,					// Indicates whether the restrict message is enabled		[ true - on | false - off]
	"RefundMessage": "Money redunded {GOLD}{MONEY}",	// Refund message
	"RefundMessageStatus": true,						// Indicates whether the refund message is enabled			[ true - on | false - off]
	"RestrictMethod": 2								// Method of restricting [ 1 - by players | 2 - by weapons ]
	"AdminImmunityFlag": "@css/root"				// Admin-flag to enable immunity for player. Admin list can be finded here:`addons/counterstrikesharp/configs/admins.json`
}
```
### RestrictConfig.json
```
{
	"Weapon": "weapon_deagle",	// weapon class for restriction
	"WeaponQuota": {			// using this method if RestrictMethod = 2
		"CT": 1,					// Allowed deagles for CT = 1
		"T": 4					// Allowed deagles for T = 4
	},
	"PlayerQuota": {			// using this method if RestrictMethod = 1
		"CT": [					// team
			{
				"4": 0,				// Allowed deagles for CT = 0 if players less or = 4
				"8": 4,				// Allowed deagles for CT = 4 if players less or = 8 and more then 4
				"16": 7				// Allowed deagles for CT = 7 if players less or = 16  and more then 8
				// here can add more player conditions using ","
			}
		],
		"T": [					// team
			{
				"4": 3,               // Allowed deagles for TT = 3 if players less or = 4
				"8": 4,               // Allowed deagles for TT = 4 if players less or = 8 and more then 4
				"16": 9               // Allowed deagles for TT = 9 if players less or = 16  and more then 8
				// here can add more player conditions using ","
			}
		]
	}
}
// here can add more weapons using ","
```
```diff
- Do not try to copy this jsons into your config. Json does not support commenting! Comments have been added here for your convenience.
```

## About possible problems, please let me know: 
[<img src="https://i.ibb.co/LJz83MH/a681b18dd681f38e599286a07a92225d.png" width="15.3%"/>](https://discordapp.com/users/858709381088935976/)
[<img src="https://i.ibb.co/tJTTmxP/vk-process-mining.png" width="15.3%"/>](https://vk.com/bgtroll)
[<img src="https://i.ibb.co/VjhryGb/png-transparent-brand-logo-steam-gump-s.png" width="15.3%"/>](https://hlmod.ru/members/palonez.92448/)
[<img src="https://i.ibb.co/xHZPN0g/s-l500.png" width="15.3%"/>](https://steamcommunity.com/id/comecamecame)
[<img src="https://i.ibb.co/S0LyzmX/tg-process-mining.png" width="16.3%"/>](https://t.me/ArrayListX)
[<img src="https://i.ibb.co/Tb2gprD/2056021.png" width="15.3%"/>](https://github.com/Quake1011)
