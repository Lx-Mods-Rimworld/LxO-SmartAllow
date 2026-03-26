# LxO - Smart Allow

A RimWorld mod that **automatically allows forbidden items** with smart per-category filtering.

## The Problem

Every RimWorld player knows: items dropped during combat, social fights, or pawn deaths get auto-forbidden. You have to manually click "Allow" on dozens of weapons, apparel, and resources after every raid. It's tedious, repetitive, and adds nothing to gameplay.

## The Solution

**Smart Allow** does it for you -- automatically, silently, with full control over what gets allowed and what stays forbidden.

## Features

- **Auto-allow all forbidden items** -- weapons, apparel, food, resources, everything
- **Per-category filtering** -- 11 toggleable categories in mod settings
- **Drugs excluded by default** -- because you probably don't want raiders' yayo auto-allowed
- **Combat-aware** -- holds auto-allow during active raids so pawns don't run into firefights for loot
- **Home zone filter** -- optionally only auto-allow items within your home zone
- **Double-forbid = permanent** -- forbid an item, Smart Allow unforbids it, forbid it again and Smart Allow leaves it alone forever
- **Zero performance impact** -- lightweight scan every 4 seconds, no per-tick overhead

## Category Toggles

| Category | Default | What It Covers |
|----------|---------|---------------|
| Weapons | ON | Guns, melee weapons |
| Apparel | ON | Clothing, armor, hats |
| Drugs | **OFF** | All drugs including beer, smokeleaf, yayo |
| Food | ON | Meals, raw food, packaged survival meals |
| Resources | ON | Steel, wood, components, plasteel, etc. |
| Corpses | ON | Human and animal corpses |
| Chunks | ON | Stone chunks, slag |
| Body Parts | ON | Bionics, prosthetics |
| Medicine | ON | Herbal, industrial, glitterworld |
| Artifacts | ON | Psychic items, special items |
| Other | ON | Everything not in above categories |

## How Double-Forbid Works

1. A weapon drops during combat -- it's auto-forbidden by RimWorld
2. Smart Allow unforbids it after combat ends
3. You decide you don't want THAT specific weapon -- you forbid it manually
4. Smart Allow sees you forbade something it already allowed -- marks it as **permanently forbidden**
5. Smart Allow never touches that item again

This lets you keep specific items forbidden without disabling entire categories.

## Compatibility

- Works with any mod. No dependencies except Harmony.
- Does not conflict with Allow Tool -- they solve different problems (Allow Tool is manual batch operations, Smart Allow is automatic).
- Safe to add or remove mid-save.

## Requirements

- RimWorld 1.6+
- [Harmony](https://steamcommunity.com/workshop/filedetails/?id=2009463077)

## Languages

English, German, Chinese Simplified, Japanese, Korean, Russian, Spanish

## Credits

Developed by **Lexxers** ([Lx-Mods-Rimworld](https://github.com/Lx-Mods-Rimworld))

Free forever. If you enjoy this mod, consider supporting development:
**[Ko-fi](https://ko-fi.com/lexxers)**

## License

- **Code:** MIT License
- **Content (textures, XML):** CC-BY-SA 4.0
