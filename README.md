# Annelitrice 3.0 Retheme — Dark Souls

将安妮莉模组的相关设定、物品、建筑、小型单位、文字和四组效果改为Dark Souls风格。以灰烬、人性、黑暗之环、篝火与火焰风暴组织主题。

## 使用与依赖

- 适用RimWorld 1.7，依赖Annelitrice 3.0及其依赖，加载顺序位于原模组之后。
- 包名为`Verniy709.Annelitrice3.0.Retheme.DarkSouls`；原模组包名为`Verniy709.Annelitrice3.0`。
- 使用本retheme时关闭旧独立效果模组`Annelitrice-AshToHumanity`，以免同一事件重复播放。
- 不建议同时启用其他针对同一原模组的retheme，以免显示内容和效果相互覆盖。
- 修改Def、贴图、声音或DLL后，重启游戏重新加载。

## 主题与中文用语

| 原有对象 | 本retheme的显示内容 |
|---|---|
| 安妮莉种族 | 安妮莉不死人 |
| 幼体 | 安妮莉人性 |
| 卵 | 安妮莉灰烬、同伴灰烬 |
| 蛹 | 安妮莉人像 |
| 营养液 | 安妮莉灵魂 |
| 驱虫药 | 安妮莉开花苔藓 |
| 喷泉设施 | 安妮莉篝火 |
| 木偶剧场 | 安妮莉绘画世界 |
| 寄生感染能力 | 黑暗之环能力 |

原有DefName与翻译键保持。例如`Anneli_Pupa`仍是人像的ThingDef，`Anneli_Infest`仍是能力标识，背景故事中的`Worm`也沿用原模组，不随显示名称重命名。

本retheme保留原先的主题化资源改动：成体和小型单位不产肉，屠宰分别产30与10份`Anneli_NutrientSolution`；拆解灰烬产5份该资源。资源在界面中显示为灵魂。原种族已有的其他字段继续由原模组提供。本次整理没有改变这些既有设定。

## 四组效果与声音

| 事件 | 表现 | SoundDef与音量 |
|---|---|---|
| 人性凝结成人像 | 灰雾、碎片、人性黑影与眼睛逐步出现 | `Anneli_Retheme_DarkSouls_HumanityToPupa`，18~18 |
| 人像释放不死人 | 火柱、火焰风暴环、火星与余烟 | `Anneli_Retheme_DarkSouls_PupaHatch`，36~36 |
| 灰烬唤起人性 | 短促暗色触须、人性核心与烟尘 | `Anneli_Retheme_DarkSouls_EggHatch`，30~30 |
| 黑暗之环传递 | 源端印记、连接雾流、目标印记与火星 | `Anneli_Retheme_DarkSouls_ShareDarksign`，30~30 |

四份声音素材位于`Sounds/Anneli_Retheme_DarkSouls/`，保留原文件、响度和播放配置。独立目录避免覆盖原模组其他声音。声音由原Effecter入口触发，灰烬唤起人性采用XML粒子，另外三组由C#控制器分批生成粒子；没有新增Harmony依赖。

`AshAnimation`控制器持续150tick，`FirestormHatchAnimation`持续165tick，`DarksignTransferAnimation`持续80tick；实际可见尾部同时由Fleck的渐隐时间决定。效果只负责表现，不接管原模组的实体生成、复活、阵营和行动流程。

开发者模式可用小型单位的`DEV:Cocoon`检查凝结；用灰烬的`DEV:Hatch`检查唤起人性；用人像的`DEV:Hatch`检查火焰风暴。两个Hatch按钮属于不同对象。

## 文件布局与命名

```text
About/About.xml
LoadFolders.xml
README.md
1.7/
  Assemblies/Annelitrice.Retheme.DarkSouls.dll
  Defs/Effects/Fleck_Visual.xml
  Defs/SoundDefs/SoundEffects.xml
  Patches/                         按原模组Def类型分类
  Languages/ChineseSimplified/    DefInjected与Keyed
  Languages/English/Keyed/
  Source/Annelitrice.Retheme.DarkSouls/
Sounds/Anneli_Retheme_DarkSouls/
Textures/Anneli_Retheme_DarkSouls/
```

- 文本、贴图和效果补丁按`AbilityDef.xml`、`EffecterDef.xml`、`ThingDef.xml`等Def类型组织。
- 18个新增FleckDef和4个SoundDef统一使用`Anneli_Retheme_DarkSouls_`前缀，不覆盖原DefName。
- 新C#类型统一使用`Annelitrice.Retheme.DarkSouls`命名空间，位于原模组`Annelitrice`之下；XML类名与DLL类型一致。
- 中文按原模组的Def类型目录组织，键使用`DefName.字段`；代码界面文案使用原有`Anneli.*` Keyed键。`AlienRace.ThingDef_AlienRace`和`AlienRace.AlienBackstoryDef`目录与原模组一致。
- 源码与程序集位于对应版本的`1.7`目录，工程默认根据安装位置解析RimWorld目录，也可通过`RimWorldDirectory`属性指定。

## 贴图与维护

常规贴图位于独立贴图根的`Anneli_Building`、`Anneli_Item`、`Anneli_Larva`和`Anneli_UI`。方向后缀`_north`、`_east`、`_south`与染色遮罩后缀`m`沿用原模组规则。方向图和遮罩即使内容相同，也保留各自文件名供Graphic_Multi读取。`Dessicated_Larva_east.png`沿用原资源路径的拼写。

灰烬与同伴灰烬各有A–F六种随机外观，人像有A–E五种。小型单位保留默认外观和1–5号替代外观，并移除原模组没有相应retheme图的6–8号选择，绘制尺寸为1.25。随机图、堆叠图、方向图及遮罩应保留当前命名方式。

效果贴图位于`Textures/Anneli_Retheme_DarkSouls/Anneli_Effect/`：

- `Anneli_PupationStart`：Smoke、Haze、Shadow、Eyes、Fragment、Orb、HatchEmber。
- `Anneli_PupationFinish`：FirestormColumn、FirestormRing。
- `Anneli_Hatch`：Tendrils。
- `Anneli_Infest`：Brand。

这些贴图由18个FleckDef共享引用，配合源码中的位置、速度、颜色与出现时机形成效果，不应按某一文件夹只服务单一事件来删除共用资源。

另保留`Textures/Anneli_Item/Pupa/PupaA.png`作为结茧开关图标。原`CompLarva`按该固定路径读取，因此这张独立按钮图不能作为遗留文件删除。

旧Fleck的无效贴图重定向已删除；旧Def仍由原模组提供并使用原资源路径。新效果使用独立前缀的FleckDef。

## 源码编译

需要.NET 9 SDK及当前RimWorld 1.7的程序集。在本模组根目录执行以下PowerShell命令，临时编译文件放在模组外：

```powershell
dotnet build .\1.7\Source\Annelitrice.Retheme.DarkSouls\Annelitrice.Retheme.DarkSouls.csproj -c Release -p:BaseIntermediateOutputPath="$env:TEMP\AnneliDarkSouls\obj\" -o "$env:TEMP\AnneliDarkSouls\bin"
Copy-Item -LiteralPath "$env:TEMP\AnneliDarkSouls\bin\Annelitrice.Retheme.DarkSouls.dll" -Destination .\1.7\Assemblies\Annelitrice.Retheme.DarkSouls.dll
```

## 本次整理

已合并效果补丁、粒子定义、声音定义与说明，删除旧无效贴图重定向和空占位说明；旧粒子移除补丁改为精确指定原节点，声音移入独立目录，版本源码归入`1.7/Source`。65张贴图、4份声音素材、四组效果参数与播放配置保持。中文一处旧种族称呼统一为当前“不死人”用语。

`.git`与`.gitattributes`保留。本文件是模组内唯一Markdown说明；检查记录、完整备份、回退脚本与发布包保存在模组目录外。


## 安妮莉篝火的环境表现（Fountain）

`Anneli_Fountain` 增加两个原生 `CompProperties_ThrownFleckEmitter`，分别发射少量灰烟与暖橙火星。灰烟每 45 tick 一缕，火星每 18 tick 一颗，向上轻飘并自然淡出。复用本 Retheme 的 `Haze.png` 与 `HatchEmber.png`，专用 FleckDef 位于 `1.7/Defs/Effects/Fleck_Bonfire.xml`，不会修改其他技能使用的粒子参数。

原生 `CompProperties_AmbientSound` 播放轻微的燃烧噼啪循环：独立 SoundDef 位于 `1.7/Defs/SoundDefs/BonfireAmbience.xml`，引用游戏自带的 `Misc/Fire/BurningLoop`，无需复制声音文件。音量为 `30~60`，距离范围 `5~25`，短淡入淡出，暂停时静音，最多同时保留两个最近声源。建筑移走或拆除时由原生组件结束声音。

上述组件只增加环境表现。篝火建成并放置在地图上后即持续工作，不要求正在执行配方；未添加燃料、电力、热量、照明或新的玩法结算。补丁保留现有和继承的工作台组件，并明确使用 Normal tick 驱动粒子及环境音。当前配方调整与已更新的十二张灰烬贴图保持原样。

这次仅更新 XML 与本说明，程序集、源码及全部原有素材不变。离线检查覆盖补丁的有/无 comps 两种情况、继承组件保留、Normal tick、原生字段与资源引用；实际游戏视觉与听感需重启游戏体验。


## 绘画世界的落雪（PuppetTheatre）

`Anneli_PuppetTheatre` 增加一个原生 `CompProperties_ThrownFleckEmitter`，每 12 tick 发射一颗细小的淡蓝白雪点。从塔尖上方约 0.88–0.98 格处，在横向 1.2 格范围内生成，沿屏幕由上向下缓缓落下，略有横向偏移，在台面附近淡出。粒子大小 0.025–0.045 格，寿命约 3.62 秒，常速稳定时约 18 颗可见雪点，形成局部、稀疏的持续落雪。

专用 FleckDef 位于 `1.7/Defs/Effects/Fleck_PaintedWorld.xml`，使用专用单颗雪点贴图 `Textures/Anneli_Retheme_DarkSouls/Anneli_Effect/Anneli_PuppetTheatre/SnowDot.png`。该贴图只有一颗柔边圆点，避免原版 `SnowBit` 在游戏中呈现为细碎雪屑簇。原生发射组件的 `velocityX` 实际传入速度角度，因此使用 173–187 度朝下发射，`velocityY` 为 0.35–0.42 的速度范围。雪点方向不随建筑朝向旋转，始终在屏幕上从上往下落。

建成放置后持续播放，不依赖观众、季节或天气，也不会产生实际积雪、温度或天气变化。补丁保留既有组件并明确采用 Normal tick。篝火的烟雾、火星和低声燃烧循环保持原样；此次仅更新 XML 和本说明，没有改动程序集、源码、既有贴图或声音。更新后重启游戏加载。


落雪第二版修正了首版预览与实际素材不一致的问题：首版用圆点示意轨迹，但游戏引用原版 SnowBit；现在预览和游戏均读取同一张 SnowDot.png。出生位置、大小、下落速度、发射间隔、透明度和寿命保持原样，继续使用原生 CompProperties_ThrownFleckEmitter。离线预览仍非游戏实录，游戏缩放和渲染可能影响细节。
