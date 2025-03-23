# Prefab Dump Max
The repo consists of two folders, one is Dumps and another is Mod.

## Dumps
* This folder contains Prefab Dumps to browse any and all Prefab assets in the vanilla game.
* Each game version's dump will have a heavily compressed zip (using 7zip) for easy download and a log. (The unzipped current version has been removed to reduce file count)
* Somewhat of a known issue: the UnityObjectsMap ID on the prefab files are known to differ between dumps, so those line changes **should be ignored**. (From 1.1.8f1, I've replaced all UnityObjectsMap ID to `x*(2^5)` (previously `xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`) so it's easy to track other differences.)

## Mod
* This folder contains a simple mod to create the prefabs. The mod is based on the idea of yenyang's Prefab ID Dump. There's also a zip for direct download of the mod.

## Version Check
* 1.1.2f1 - 2024-07-11 - 40053 prefabs
* 1.1.7f1 - 2024-07-11 - 40403 prefabs
* 1.1.8f1 - 2024-09-04 - 40417 prefabs
* 1.1.10f1 - 2024-10-24 - 40466 prefabs
* 1.1.12f1 - 2024-11-26 - 40466 prefabs
* 1.2.0f1 - 2024-12-11 - 40805 prefabs
* 1.2.3f1 - 2025-01-22 - 40808 prefabs
* 1.2.5f1 - 2025-03-18 - 42050 prefabs:
 * Base Game* => 40840 items
 * ModernArchitecture => 257 items
 * UrbanPromenades => 204 items
 * FreeUpdate02 => 131 items
 * DragonGate => 207 items
 * LeisureVenues => 122 items
 * MediterraneanHeritage => 289 items
\* Base Game includes CS1 Treasure Hunt, San Francisco Pack, Landmark Pack.

## Notable Changes
See PREFAB_CHANGELOG.md