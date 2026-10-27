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

## 1.0.6

### English

- **New** MilesSoul gameplay support: this addon now uses the **same local Scout gem detection and settlement source as the main mod's client mode**. With a host running RollOfTheScout 1.0.9+, a client that has this addon enabled no longer needs the host's separate remote-client petrify heuristic.
    - Root cause of the previous rapid-use issue: the host inferred gem use from delayed remote petrify copies and later wrote an absolute reduction back. That write could overwrite newer in-flight increases and reduce petrify too much or clear it.
    - The host now sends the effective MilesSoul configuration to this addon; local action Prefix / Postfix hooks settle hunger, petrify and extra stamina directly on the owner client, so original local synchronization carries the result back. Targeted usage reports are used only for the host's 【Hindrance of the Souls】 count, never to apply the effects a second time.
    - Photon event 197 kinds 7 / 8 / 9 / 10 provide the hello, configuration, usage report and configuration ACK. The host skips its old effect path only after the ACK, so handshake-window uses remain on the host fallback. This gameplay channel is independent of class-display network sync.
- **Fixed** MilesSoul 【Rule Zero】: the +30 extra stamina is granted only after the petrify from feeding 【Scout's Perseverance】 to a downed player has returned to its pre-use target. The previous order could let the later petrify / stamina-cap update clamp away the freshly granted stamina.
- **Changed** This addon is no longer strictly receive-only: only the MilesSoul feature sends targeted capability / usage-count messages to the host. Class display remains receive-only and local to each client's screen. The addon still disables itself when the main mod is installed, because the main mod provides the same shared implementation.
- **Optimised** MilesSoul handshake on the client: an unchanged state is no longer re-parsed each time it arrives, the configuration ACK is throttled to 8 s (under the host's 12 s timeout), the scene check is cached, and hello is only sent in gameplay. The host reply is now hello-driven instead of a duplicate 2 s broadcast.
- **Fixed** The top-left notification box could be only one line tall on some clients, so a two-line bilingual notice was clipped to "English lower half + Chinese upper half".
    - Root cause: the IMGUI fallback (used when the native TMP overlay is unavailable) drew each queue entry at a fixed single-line height, while one entry may contain an explicit newline or wrap on a narrow screen; middle-left alignment then clipped the taller text symmetrically. The box height had the same defect.
    - Entries are now measured with `GUIStyle.CalcHeight`, the box height is the sum of the real heights, word wrapping is enabled and the fallback text is no longer bold (matching the native overlay).

### 中文

- **新增** 残魂玩法支持：本附属模组现在与主模组客机模式**共用同一份童军宝石本机检测与结算源码**。主机使用 RollOfTheScout v1.0.9+ 时，启用本附属模组的客机不再依赖主机原先的客机石化启发式。
    - 旧高频使用问题的根因：主机根据延迟同步的客机石化副本推测宝石使用，再进行绝对减量写回；该写入可能覆盖较新的在途增量，导致石化额外降低乃至清空。
    - 主机现在向本附属模组同步实际生效的残魂配置；本地动作 Prefix / Postfix 直接在属主客户端结算饥饿、石化与额外体力，再由原版同步回传结果。定向使用报告仅供主机累计【众魂阻挠】次数，不会重复施加效果。
    - Photon 事件 197 的 Kind 7 / 8 / 9 / 10 分别承载 Hello、配置、使用报告与配置 ACK；主机收到 ACK 后才跳过旧效果路径，握手期间的使用仍由主机兜底。该玩法通道不受「职业显示-网络同步」开关影响。
- **修复** 残魂【零号守则】：喂毅力给昏迷玩家后，先确认石化回到使用前目标，再发放 +30 额外体力。旧顺序会让后续石化 / 体力上限更新把刚发放的额外体力 clamp 掉。
- **调整** 本附属模组不再是绝对意义上的纯接收模组：仅残魂功能会向主机定向发送能力 / 使用计数消息；职业显示仍为纯接收且只作用于本机屏幕。本机安装主模组时仍会自我停用，因为主模组已包含同一共用实现。
- **优化** 残魂握手的客机侧开销：内容未变的状态不再重复解析，配置 ACK 节流为每 8 秒一次（仍小于主机 12 秒超时），场景判定改为缓存，Hello 仅在关卡内发送；主机回复也改为由 Hello 触发，不再重复 2 秒群发。
- **修复** 部分客机上左上角提示窗口高度不足，中英双语两行提示各被裁掉一半。
    - 根因：原生 TMP 字体叠加层不可用时走 IMGUI 回退绘制，该路径把每条提示按固定单行高度绘制；一条提示本身可含换行（英文 + 中文），窄屏还会折行，配合垂直居中即被上下对称裁切，窗口背景高度同样偏小。
    - 现在逐条用 `GUIStyle.CalcHeight` 测量实际高度并累加窗口高度，启用自动折行，并取消回退文本加粗（与原生叠加层一致）。
