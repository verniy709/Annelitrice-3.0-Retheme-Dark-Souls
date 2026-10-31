# Annelitrice 3.0 Retheme DarkSouls (RimWorld 1.7)

Load this add-on after **Annelitrice 3.0**. Its XML patches replace the requested labels, descriptions, and childhood backstories with `[TODO]` placeholders. `Anneli_Infest` also has placeholder text. The Chinese Simplified placeholders are in `1.7/Languages/ChineseSimplified/DefInjected/`.

All retheme PNGs live under `Textures/Anneli_Retheme_DarkSouls/`. The XML patches in `1.7/Patches/` are organized by Def type; each file contains its category's text, texture-path, and gameplay changes. See `Textures/TEXTURE_SLOTS.md` for the available artwork slots.

The two eggs and pupa use `Graphic_Random`. Add only the variants you want selected to their folders (`EggA.png`, `EggB.png`, etc.). Each currently contains one copy of the base art as a placeholder. Replace that placeholder when your own artwork is ready. The base mod's other A–I variants cannot be selected through these paths.

The larva has the default graphic and alternate variants 1–5 here. `1.7/Patches/PawnKindDef.xml` removes variants 6–8 from the base `Anneli_Larva` PawnKindDef so the game cannot choose missing retheme variants, and changes its body graphic draw size to 1.25. The dessicated larva graphic uses the retheme's `Dessicated_Larva_east.png`.

`1.7/Patches/AlienRace.ThingDef_AlienRace.xml` and `1.7/Patches/ThingDef.xml` set `MeatAmount` to 0 for the adult and larva and add `Anneli_NutrientSolution` as the sole butcher yield: 30 for an adult Annelitrice and 10 for `AnnelitriceLarvaAsAnimal`. Both races already have `LeatherAmount` set to 0. Their `useMeatFrom` fields remain for meat-def resolution but produce no meat at zero amount.

`1.7/Patches/RecipeDef.xml` adds placeholder labels and descriptions for hatching, butchering, cremating, anthelmintic production, and curing the infection. Butchering one egg now gives five `Anneli_NutrientSolution` instead of insect meat and jelly.

`1.7/Patches/ThoughtDef.xml` adds placeholder stage labels and descriptions for both nutrient-solution drinking thoughts.

`Anneli_PupationStart` and `Anneli_PupationFinish` are EffecterDefs without display text or direct texture fields; their FleckDef artwork is redirected to the retheme folder. The `Annelitrice` race def has no direct `texPath`; its full pawn rendering uses HAR/NareisLib definitions and is outside this initial lore scaffold.

The retheme now includes four integrated effects and their sounds: humanity becoming a pupa, a pupa emerging in a firestorm, egg hatching in a brief humanity burst, and the Share the Darksign ability. Their textures are in `Textures/Anneli_Retheme_DarkSouls/Anneli_Effect/`, sounds in `Sounds/`, FleckDefs and SoundDefs in `1.7/Defs/`, and effecter patches in `1.7/Patches/`.

The base mod already spawns the three lifecycle EffecterDefs. This retheme attaches custom SubEffecters to the pupation and pupa-emergence effecters, uses XML alone for egg hatching, and attaches an ability comp for Darksign transfer. The animation assembly has no Harmony dependency; its source and project file are in `Source/Annelitrice.Retheme.DarkSouls/`. The former standalone `Annelitrice-AshToHumanity` add-on should be disabled when this retheme is enabled, or each effect will play twice.

Restart RimWorld after changing PNGs or effect definitions. The XML and paths have been checked against the installed 1.7 base mod; an in-game load test is still needed.
