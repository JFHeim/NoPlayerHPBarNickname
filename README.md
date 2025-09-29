# NoPlayerHPBarNickname

Hide other players' health bars and names. Optionally applies to mobs with fine-grained distance controls. Designed for immersion, PvP fairness, and cleaner HUD.

- Players: hide nameplates and HP bars based on distance or entirely
- Mobs: hide HP bars, names, alert signs, and stars independently
- Optional global override for vanilla HUD max show distance

Tested with Valheim 1-0.221.4. Mod works on client. install it on server to force config sync with server one. 

## Config
```
[General]

## Locks client config file so it can't be modified [Synced with Server]
# Setting type: Boolean
# Default value: true
ServerConfigLock = true

[Mobs]

## If mob is more than this distance from player, its health bar will be hidden. Set to 0 to hide health bar completely [Synced with Server]
# Setting type: Int32
# Default value: 6
Mobs healthBar distance = 6

## If mob is more than this distance from player, its name will be hidden. Set to 0 to hide name completely [Synced with Server]
# Setting type: Int32
# Default value: 2
Mobs name distance = 2

## If mob is more than this distance from player, its alert sign will be hidden. Set to 0 to hide alert sign completely [Synced with Server]
# Setting type: Int32
# Default value: 5
Mobs alert sign distance = 5

## If mob is more than this distance from player, its stars will be hidden. Set to 0 to hide stars completely [Synced with Server]
# Setting type: Int32
# Default value: 5
Mobs stars distance = 5

[Other]

## This overrides vanilla hud max show distance. Set to 0 to disable and keep vanilla value. Vanilla value is 30. [Synced with Server]
# Setting type: Int32
# Default value: 0
Any hud max show distance. Warning: Read description = 0

[Players]

## If player is more than this distance from player, his/her health bar will be hidden. Set to 0 to hide health bar completely [Synced with Server]
# Setting type: Int32
# Default value: 6
Players healthBar distance = 6

## If player is more than this distance from player, his/her name will be hidden. Set to 0 to hide name completely [Synced with Server]
# Setting type: Int32
# Default value: 2
Players name distance = 2

```

### <ins>If something does not work for you, you have found any bugs, there are any suggestions, then be sure to write to me!</ins><br>
 
Discord ```justafrogger```
