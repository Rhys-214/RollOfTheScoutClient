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
