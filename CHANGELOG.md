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

## 1.0.3

### English

- Align the version numbers.

### 中文

- 对齐版本号。
