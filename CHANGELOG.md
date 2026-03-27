# Changelog

All notable changes to this mod will be documented in this file.

## [1.0.1] - 2026-03-27

### Fixes
- Fixed double-forbid false positive on animal corpses. When pigs or other animals died, vanilla re-forbade the corpse immediately after SmartAllow allowed it, triggering a false permanent lock. Now uses a cooldown: re-forbids within 8 seconds are treated as game code (ignored), re-forbids after that are treated as player intent (permanent lock).
- Faction.OfPlayer null-safe in combat detection.

## [1.0.0] - 2026-03-26

### Features
- Automatically allows forbidden items after combat or events
- 11 category toggles: weapons, apparel, drugs, food, resources, corpses, chunks, body parts, medicine, artifacts, and other
- Drugs excluded by default -- raiders' yayo stays forbidden unless you opt in
- Combat-aware: holds auto-allow during active raids so pawns don't run into firefights
- Home zone filter: optionally only allow items within your home zone
- Double-forbid = permanent: forbid an item twice and Smart Allow stops touching it
- 7 languages: English, German, Chinese Simplified, Japanese, Korean, Russian, Spanish
