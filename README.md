# RollOfTheScoutClient · 童子军的命运之骰：客机显示

**English** ｜ **中文**（见下方）

---

## English

> The **client display and MilesSoul sync addon** for **RollOfTheScout**. It draws the host-broadcast class panel, toasts and overhead **【Class】Nickname** prefix on each client's own screen. 
> For MilesSoul, it also detects local Scout gem actions and settles their effects with the host-synchronized configuration.

### Highlights

- **Shows class info on the client's own screen**: the class-assignment panel (key-triggered), effect notifications, and the overhead `【Class】Nickname` prefix.
- **Reliable MilesSoul gem effects**: uses the same local action detection and settlement as the main mod's client mode, avoiding host-side heuristic and stale-petrify writebacks. Only this feature exchanges targeted handshake / configuration / usage-count messages (Photon custom event `197`).
- **Needs the host to run the main mod** (`RollOfTheScout`). Class display requires class-display network sync; MilesSoul gameplay sync is independent of that display option and requires the new host version.
- **Self-disables** when the main mod is already installed on the same machine (the main mod provides both features), so installing both is harmless.
- **Bilingual configuration**, selected automatically by the game language.
- Also adds a compatibility layer so the overhead prefix still shows up in third-party name-tag mods (BetterPlayerDistance / Sense of Direction).

### Requirements

- **PEAK** (Steam)
- **BepInEx** for PEAK (`BepInExPack_PEAK`)
- On the host: the main mod **`RollOfTheScout`**

### Installation (on clients)

1. Install BepInEx for PEAK.
2. Put the file `com.peakmodding.RollOfTheScoutClient.dll` into `PEAK/BepInEx/plugins/`.
3. Done — nothing else to set up.

> Players who already run the main mod do **not** need this addon; it disables itself automatically.

### Configuration

Config files are generated automatically on first run(`BepInEx/config/com.peakmodding.RollOfTheScoutClient.cfg`).
Options: the class-display key, the display mode (`HoldToShow` / `PressToShow` / `ToggleOnPress`) and the overhead class prefix.

### Building from source

Requirements: **.NET SDK**, **PEAK**, **BepInEx 5**.

```bash
# 1) Point the build at your local paths
#    copy Config.Build.user.props.example  ->  Config.Build.user.props
#    then fill in PeakRoot / BepInExCore

# 2) Build (Release)
dotnet build Mod/RollOfTheScoutClient/RollOfTheScoutClient.csproj -c Release
```

Output: `Mod/RollOfTheScoutClient/bin/Release/com.peakmodding.RollOfTheScoutClient.dll`.

### Repository layout

```
.
├── Directory.Build.props            # repo-level MSBuild config (collects local paths)
├── Config.Build.user.props.example  # template for your local paths (copy to Config.Build.user.props)
├── README.md
├── LICENSE
└── Mod/
    ├── RollOfTheScoutClient/        # the client display addon
    └── Shared/                      # source shared with the main mod
```

### Known constraints

- Class text is per-client local UI; a client without this addon (or the main mod) sees the class **effects** but no class **text**.
- If the host does not run the main mod, there is no broadcast and nothing is displayed.
- The addon draws only on the local screen; it cannot change what other players see. MilesSoul usage reports are used by the host only for Scoutmaster-spawn counts, not to repeat the local status effects.

### License

Licensed under the **[GNU General Public License v3.0 (GPLv3)](LICENSE)**, the same license as the main mod.

- ✅ You may use, modify and redistribute this mod — **including commercially**.
- ✅ Derivative works must be released under the GPLv3 too (**share-alike**: ship the source).
- ⚠️ Provided **without any warranty**.

### Credits

- The **RollOfTheScout** main mod and the **Silver** mod (by fengsheng404) — the original design inspiration of main mod.
- [BepInEx](https://github.com/BepInEx/BepInEx) and [HarmonyX](https://github.com/BepInEx/HarmonyX).
- This mod was made with **Deepseek V4.1 Flash**.

### Disclaimer

This is an **unofficial third-party mod**, not affiliated with PEAK's developers or publishers. This repository contains **no** game assemblies, decompiled game code, or other copyrighted assets.

### A few words from the author

General contact: email **2043581350@qq.com**.

---

## 中文

> **RollOfTheScout（童子军的命运之骰）** 的**客机显示 / 残魂同步附属模组**。它在**客机自己的屏幕**上绘制职业分配面板、效果提示（Toast）与头顶「**【职业】昵称**」前缀
> 残魂时还会检测本机童军宝石动作，并按主机同步的配置结算效果。

### 特色

- **在客机自己屏幕上显示职业信息**：职业分配面板（按键触发）、职业效果提示、以及头顶「【职业】昵称」前缀。
- **可靠的残魂宝石效果**：与主模组客机模式共用本机动作检测与结算，避开主机启发式与旧石化值写回。仅该功能会通过 Photon 自定义事件 `197` 定向交换握手、配置与使用计数消息。
- **需要主机安装主模组**（`RollOfTheScout`）：职业显示需开启「职业显示-网络同步」；残魂玩法同步不受该显示开关影响，但需要新版主机。
- **自我停用**：若本机已安装主模组，本模组会自动停用（主模组本身已提供这两类功能），两者都装也不会冲突。
- **中英双语配置**，按游戏语言自动切换。
- 另含兼容层，使头顶前缀在第三方头顶 id 显示模组（BetterPlayerDistance / Sense of Direction）中也能正常显示。

### 环境要求

- **PEAK**（Steam）
- PEAK 的 **BepInEx**（`BepInExPack_PEAK`）
- 主机端：需安装主模组 **`RollOfTheScout`**

### 安装（客机端）

1. 为 PEAK 安装 BepInEx；
2. 将文件 `com.peakmodding.RollOfTheScoutClient.dll` 放入 `PEAK/BepInEx/plugins/`；
3. 完成 —— 无需其它设置。

> 已经装了主模组的玩家**不需要**本附属模组；它会自动停用。

### 配置

配置文件将在首次运行自动生成（`BepInEx/config/com.peakmodding.RollOfTheScoutClient.cfg`）
配置项：职业显示按键、显示模式（`HoldToShow` / `PressToShow` / `ToggleOnPress`）、头顶职业前缀。

### 从源码编译

前置：**.NET SDK**、**PEAK**、**BepInEx 5**。

```bash
# 1) 指向本机路径
#    复制 Config.Build.user.props.example → Config.Build.user.props
#    填写 PeakRoot / BepInExCore

# 2) 编译（Release）
dotnet build Mod/RollOfTheScoutClient/RollOfTheScoutClient.csproj -c Release
```

产物：`Mod/RollOfTheScoutClient/bin/Release/com.peakmodding.RollOfTheScoutClient.dll`。

### 仓库结构

```
.
├── Directory.Build.props            # 仓库级 MSBuild 配置（集中本机路径）
├── Config.Build.user.props.example  # 本机路径模板（复制为 Config.Build.user.props）
├── README.md
├── LICENSE
└── Mod/
    ├── RollOfTheScoutClient/        # 客机显示附属模组
    └── Shared/                      # 与主模组共用的源码
```

### 已知约束

- 职业文字属于各客户端本地 UI；未装本附属模组（也没装主模组）的客机只看得到职业**效果**，看不到职业**文字**。
- 若主机没有安装主模组，就没有广播来源，本模组不会显示任何内容。
- 职业文字只绘制在**本机屏幕**上，无法改变其他玩家看到的内容；残魂使用报告仅供主机累计【众魂阻挠】次数，不会重复施加本机已结算的状态效果。

### 许可证

本项目采用 **[GNU 通用公共许可证第 3 版（GPLv3）](LICENSE)**，与主模组一致。

- ✅ 可以自由使用、修改与再分发（**包括商业用途**）；
- ✅ 衍生作品必须以 **GPLv3** 发布（**相同方式共享**：需一并提供源码）；
- ⚠️ 本程序**不提供任何担保**。

### 致谢

- **RollOfTheScout** 主模组，以及 **Silver** 模组（作者 fengsheng404）—— 主模组最初的设计灵感来源。
- [BepInEx](https://github.com/BepInEx/BepInEx) 与 [HarmonyX](https://github.com/BepInEx/HarmonyX)。
- 本模组使用 **Deepseek V4.1 Flash** 制作。

### 免责声明

本模组为**第三方非官方作品**，与 PEAK 的开发 / 发行方**无任何关联**。本仓库**不包含**任何游戏程序集、游戏反编译源码或其它受版权保护的资源。

### 一些作者的话

- 作者通用联系方式：邮箱 **2043581350@qq.com**（当然，你也可以直接加QQ反馈意见与建议）
- 欢迎加入PEAK同好群喵（**QQ群：814891614**，模组更新将优先在本群内测）~
