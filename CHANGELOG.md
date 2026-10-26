# Changelog · 更新日志

## 1.0.0

### English

- Initial public release of the **client display addon** for RollOfTheScout.
- GUID `com.peakmodding.RollOfTheScoutClient`; requires the host to run the main mod `RollOfTheScout` with class-display network sync enabled.
- Shows the class-assignment panel, effect notifications (toasts) and the overhead class prefix on the client's own screen, using the host's broadcast (Photon custom event 197).
- Receive-only: never sends events and never changes gameplay.
- Self-disables when the main mod is installed on the same machine.
- Includes a compatibility layer for third-party name-tag mods (BetterPlayerDistance / Sense of Direction).
- Bilingual config (Chinese / English), selected automatically by the game language.

### 中文

- **RollOfTheScout 的客机显示附属模组**首个公开发布版本。
- GUID `com.peakmodding.RollOfTheScoutClient`；需主机运行主模组 `RollOfTheScout` 并开启「职业显示-网络同步」。
- 借助主机的广播（Photon 自定义事件 197），在客机自己屏幕上显示职业分配面板、职业效果提示（Toast）与头顶职业前缀。
- 纯接收：从不发送事件、绝不改变玩法。
- 本机已装主模组时自动停用。
- 含第三方头顶 id 显示模组（BetterPlayerDistance / Sense of Direction）兼容层。
- 配置中英双语，按游戏语言自动切换。

## 1.0.1

### English

- Urgently fixed some issues with the mod configuration file.

### 中文

- 紧急修复了模组配置文件的一些问题。

## 1.0.2

### English

- Add a class introduction document for this mod (**ModClassIntroduction.md**, bilingual in Chinese and English).
- **New** The class-assignment panel now also shows **your own class's** keywords and a short introduction right below it, so you can understand your class without opening the class document. The text follows the game language (Chinese / English); only the local player's class is shown.
- **New** The host now also broadcasts the keywords / intro text for the classes in play (event 197, kinds 4 / 5). If a future main-mod update adds a class that this addon does not know yet, the host-provided text is shown instead — **this addon no longer has to be updated in lockstep with the main mod** just to display a new class's introduction. Text for known classes is still taken from the local table, so each client keeps its own language.
- **Improved** Performance: the class panel text is cached and only rebuilt when the assignment changes, the TMP panel update is version-gated (no per-frame string building while the panel is open), and the overhead prefix caches its per-name state component instead of calling `GetComponent` for every player every frame.
- **Changed** The intro line now reads `【Team core】 …` instead of `Intro: Team core: …` (the former "Intro: " label is folded into a bracketed name; Chinese and English follow the same shape).
- Data source: the same class codes the host already broadcasts (Photon custom event 197); this addon still never sends anything.

### 中文

- 为本模组补充职业介绍文档（**ModClassIntroduction.md**，中英双语）。
- **新增** 职业分配面板下方现在会同时显示**本人职业**的关键词与简介，无需翻阅职业文档即可快速了解自己的职业。文本随游戏语言（中文 / 英文），只显示本机玩家自己的职业。
- **新增** 主机现在会额外广播在场职业的关键词 / 简介文本（事件 197，Kind 4 / 5）。主模组日后新增职业、而本附属模组还不认识该职业时，会改用主机广播的文本显示 —— **本附属模组不必与主模组同步更新**才能显示新职业的介绍。已知职业仍优先使用本地文案，各客户端保持自己的语言。
- **优化** 性能：职业面板文本改为缓存（仅在分配变化时重建），TMP 面板按版本号跳过重排（按住面板键时不再每帧拼接字符串）；头顶前缀的状态组件改为缓存，不再每帧对每个玩家调用 `GetComponent`。
- **调整** 简介行改为「【团队核心】…」格式（去掉「简介：」前缀，中英一致）。
- 数据来源仍是主机已广播的职业数值（Photon 自定义事件 197）；本附属模组依旧只接收、不发送。

## 1.0.4

### English

- **Changed** Bilingual display and config: the class panel shows `Name：EnglishClass · 中文职业`; intro lines show English keywords/intro followed by the Chinese ones; effect notices can carry an English line plus a Chinese line; the config file is now permanently bilingual (`General通用` / `classDisplayKey职业显示-按键`) and old files are auto-backed-up and regenerated. The overhead default is now **off** (`overheadClassPrefix = false`).
- **New** Supports targeted notices (event 197 kind 6) used by the main mod for per-class warnings (e.g. the 10 s forced-backflip warning); non-targeted clients ignore them.
- **Changed** Config keys follow the Chinese key's hyphen segments with capitalized English parts (e.g. `ClassDisplay-Key职业显示-按键`), and **old config files are backed up and regenerated automatically**.
- **Changed** Overlay text is no longer bold, and the addon prefers a thinner font when the game provides one, so English lines are clearer.

### 中文

- **调整** 显示与配置改为中英双语：职业面板显示 `玩家名：EnglishClass · 中文职业`；介绍行先英文关键词 / 简介、后中文；效果提示可带「英文一行 + 中文一行」；配置文件固定中英双语（`General通用` / `classDisplayKey职业显示-按键`），旧配置自动备份并重建。头顶职业前缀默认改为**关闭**（`overheadClassPrefix = false`）。
- **新增** 支持定向提示（事件 197 Kind 6），供主模组发送「仅对应职业可见」的提醒（如强制后空翻前 10 秒预警）；非目标客机收到即忽略。
- **调整** 配置键名的英文部分改为按中文分段、每段首字母大写（如 `ClassDisplay-Key职业显示-按键`）；**旧配置自动备份并重建**。
- **调整** 屏幕文字取消加粗；游戏提供更细字体时优先使用，英文行更清晰。

## 1.0.5

### English

- **Fixed** The class intro panel could show another class's description (usually after a reclass: the title showed the correct new class, but the description was **M's**).
    - Root cause: `M = 8` and `Savage = 9` in the class enum, but this addon's intro table had **case 8 and case 9 swapped**, so a Savage rendered M's text and vice versa.
- **Changed** The class intro panel now sits in the **top-left corner** instead of below the class list, so a large lobby can no longer push it off-screen; while it is visible, top-left notifications are shifted below it and return to the top-left afterwards.

### 中文

- **修复** 职业简介板块可能显示成其他职业的简介（多数发生在转职后：标题里的新职业正确，简介却显示成 **M** 的简介）。
    - 根因：职业枚举 `M = 8`、`Savage = 9`，而本附属模组的简介表把 **case 8 / case 9 写反了** —— 野人显示 M 的简介（反之亦然）。
- **调整** 职业介绍板块移至屏幕**左上角**（不再挂在职业分配面板下方），人数较多时不会被挤出屏幕；该板块显示期间，左上角提示临时下移到其下方，关闭后自动复位。
