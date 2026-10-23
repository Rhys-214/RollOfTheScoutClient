using BepInEx.Configuration;

namespace RollOfTheScoutClient
{
    /// <summary>
    /// 附属模组的少量配置（只影响「客机自己屏幕上」的显示，与主机模组互不影响）。
    ///
    /// 中英双语：按游戏语言决定 Section / Key / Description 用中文还是英文（见 <see cref="ConfigLocalization"/>），
    /// 机制与主模组 <c>RollOfTheScout</c> 一致。
    /// 配置文件固定为插件自身的 <c>com.peakmodding.RollOfTheScoutClient.cfg</c>（内容语言随游戏语言），
    /// 必须如此才能被 PEAKLib.ModConfig 枚举到 —— 它只读取各插件的 plugin.Config。
    /// </summary>
    public static class ClientConfig
    {
        public static ConfigEntry<string> ClassListKey { get; private set; }
        public static ConfigEntry<string> ClassListMode { get; private set; }
        public static ConfigEntry<bool> NamePrefix { get; private set; }

        public static void Initialize(ConfigFile config)
        {
            // ⚠ 绑定到 BepInEx 传入的插件配置文件本身：PEAKLib.ModConfig 只读取各插件的 plugin.Config。
            ConfigLocalization.DetectLanguage();

            ClassListKey = BindL(config, "通用", "职业显示-按键", "Tab",
                "按哪个键在【客机自己的屏幕】上显示「当前职业分配情况」（由主机广播而来）。\n" +
                "填写 Unity 的 KeyCode 名，例如 Tab、F1、LeftAlt、BackQuote、Mouse2。");
            // 该项的「值」也随语言变化：英文配置用 HoldToShow / PressToShow / ToggleOnPress，
            // 中文配置沿用 按住显示 / 按下显示 / 按下切换；代码对中英两种写法都识别。
            ClassListMode = BindL(config, "通用", "职业显示-模式",
                ConfigLocalization.UseEnglish ? "HoldToShow" : "按住显示",
                "职业分配面板的显示方式：\n" +
                "按住显示 = 按住该键时显示，松开立即隐藏；\n" +
                "按下显示 = 按一下显示 5 秒后自动隐藏；\n" +
                "按下切换 = 每按一次在「显示 / 隐藏」之间切换。");
            NamePrefix = BindL(config, "通用", "头顶职业前缀", true,
                "是否把「头顶玩家名称」改写成「【职业】昵称」，例如「张三」→「【先驱】张三」。\n" +
                "「【职业】」用该玩家进入游戏时的皮肤色着色；「昵称」维持原版颜色不变。\n" +
                "职业数据由主机广播而来；本开关只影响【本机屏幕】上看到的名字。\n" +
                "默认 true。");

            // 首次运行（或新增键）时立即落盘：确保 {GUID}.cfg 一定生成，ModConfig 才有文件可读。
            config.Save();
        }

        /// <summary>
        /// 本地化版 <c>config.Bind</c>：按当前语言翻译 Section / Key / Description 后再绑定。
        /// 中文语言下三者均为传入的中文原文；英文语言下改用英文分类名 / 小驼峰键 / 英文说明
        /// （翻译表见 <see cref="ConfigLocalization"/>；未收录的条目自动回退为中文原文）。
        /// </summary>
        private static ConfigEntry<T> BindL<T>(ConfigFile config, string zhSection, string zhKey,
            T defaultValue, string zhDescription)
        {
            return config.Bind(
                ConfigLocalization.Section(zhSection),
                ConfigLocalization.Key(zhSection, zhKey),
                defaultValue,
                ConfigLocalization.Desc(zhSection, zhKey, zhDescription));
        }

        /// <summary>
        /// 把配置里的按键名解析为 KeyCode；无法解析时退回 Tab。
        ///
        /// 结果按「配置字符串」缓存：本方法由每帧的输入读取调用，若每次都走
        /// <see cref="System.Enum.TryParse{T}"/> 会带来无谓的解析开销与 GC 抖动；
        /// 只有用户修改 cfg 并重载后（字符串变化）才重新解析。
        /// </summary>
        public static UnityEngine.KeyCode ParseKey()
        {
            string text = ClassListKey != null ? ClassListKey.Value : null;
            if (string.Equals(text, _cachedKeyRaw)) return _cachedKey;

            _cachedKeyRaw = text;
            _cachedKey = !string.IsNullOrEmpty(text)
                && System.Enum.TryParse(text.Trim(), true, out UnityEngine.KeyCode key)
                    ? key
                    : UnityEngine.KeyCode.Tab;
            return _cachedKey;
        }

        /// <summary>
        /// 显示模式分类：1 = 按住显示、2 = 按下切换、0 = 按下显示若干秒。
        /// 分类顺序与旧实现一致（先判「按住」、再判「切换」）；同样按配置字符串缓存，
        /// 避免每帧对模式串做两次 <c>Contains</c>。
        /// 中文配置值为 按住显示 / 按下显示 / 按下切换，英文配置值为 HoldToShow / PressToShow / ToggleOnPress，两种写法都识别。
        /// </summary>
        public static int ClassListModeKind()
        {
            string mode = ClassListMode != null ? (ClassListMode.Value ?? string.Empty) : string.Empty;
            if (string.Equals(mode, _cachedModeRaw)) return _cachedModeKind;

            _cachedModeRaw = mode;
            string lower = mode.ToLowerInvariant();
            _cachedModeKind = (mode.Contains("按住") || lower.Contains("hold")) ? 1
                : (mode.Contains("切换") || lower.Contains("toggle")) ? 2 : 0;
            return _cachedModeKind;
        }

        private static string _cachedKeyRaw;
        private static UnityEngine.KeyCode _cachedKey = UnityEngine.KeyCode.Tab;
        private static string _cachedModeRaw;
        private static int _cachedModeKind;
    }
}
