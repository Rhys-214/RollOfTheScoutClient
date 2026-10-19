using System;
using System.Collections.Generic;

namespace RollOfTheScoutClient
{
    /// <summary>
    /// 附属模组配置文件的中英双语国际化（按游戏语言择一），机制与主模组 <c>RollOfTheScout</c> 完全一致。
    ///
    /// · 游戏语言为中文（简体 / 繁体）→ <see cref="UseEnglish"/> = false：Section / Key / Description 全部使用中文原文；
    /// · 其它语言 → <see cref="UseEnglish"/> = true：Section 使用英文分类名、Key 使用小驼峰（camelCase）、Description 使用英文。
    /// 翻译表以「中文 Section | 中文 Key」为索引；未收录的条目自动回退为中文原文。
    /// </summary>
    internal static class ConfigLocalization
    {
        /// <summary>是否使用英文配置（由 <see cref="DetectLanguage"/> 依据游戏语言设置）。</summary>
        internal static bool UseEnglish { get; private set; }

        /// <summary>中文 Section → 英文 Section。</summary>
        private static readonly Dictionary<string, string> Sections = new Dictionary<string, string>
        {
            { "通用", "General" },
        };

        /// <summary>「中文 Section | 中文 Key」→ 英文 Key / 英文 Description。</summary>
        private static readonly Dictionary<string, (string Key, string Desc)> Entries =
            new Dictionary<string, (string Key, string Desc)>
        {
            { "通用|职业显示-按键", ("classDisplayKey", "Key that shows the current class assignment (broadcast by the host) on the client's own screen. Enter a Unity KeyCode name, e.g. Tab, F1, LeftAlt, BackQuote, Mouse2.") },
            { "通用|职业显示-模式", ("classDisplayMode", "Display mode of the class panel. HoldToShow = show while the key is held and hide immediately on release; PressToShow = show for 5 seconds after a single press, then auto-hide; ToggleOnPress = each press toggles between show and hide.") },
            { "通用|头顶职业前缀", ("overheadClassPrefix", "Whether to rewrite the overhead player name as '[Class] Nickname', e.g. Alice -> [Explorer] Alice. The '[Class]' part is colored with that player's skin color; the nickname keeps its vanilla color. Class data comes from the host's broadcast; this switch only affects names seen on the local screen. Default true.") },
        };

        /// <summary>
        /// 依据游戏语言决定用中文还是英文配置。
        /// 优先读游戏的 <c>LocalizedText.CURRENT_LANGUAGE</c>；若其仍为默认 English（说明尚未套用玩家设置），
        /// 再退回读系统语言 <c>Application.systemLanguage</c>。
        /// </summary>
        internal static void DetectLanguage()
        {
            bool chinese;
            try
            {
                var current = LocalizedText.CURRENT_LANGUAGE;
                if (current == LocalizedText.Language.SimplifiedChinese || current == LocalizedText.Language.TraditionalChinese)
                {
                    chinese = true;
                }
                else if (current != LocalizedText.Language.English)
                {
                    chinese = false;   // 游戏内已显式设为其它语言 → 视为非中文
                }
                else
                {
                    chinese = IsSystemChinese();   // 仍是默认值 English → 以系统语言为准
                }
            }
            catch
            {
                chinese = IsSystemChinese();
            }

            UseEnglish = !chinese;
        }

        private static bool IsSystemChinese()
        {
            var sys = UnityEngine.Application.systemLanguage;
            return sys == UnityEngine.SystemLanguage.ChineseSimplified
                || sys == UnityEngine.SystemLanguage.ChineseTraditional;
        }

        /// <summary>中文 Section → 当前语言的 Section（英文语言下未收录则不翻译）。</summary>
        internal static string Section(string zhSection)
        {
            return UseEnglish && Sections.TryGetValue(zhSection, out string en) ? en : zhSection;
        }

        /// <summary>中文 Key → 当前语言的 Key（英文语言下未收录则回退中文）。</summary>
        internal static string Key(string zhSection, string zhKey)
        {
            return UseEnglish && Entries.TryGetValue(Index(zhSection, zhKey), out var e) ? e.Key : zhKey;
        }

        /// <summary>中文 Description → 当前语言的 Description（英文语言下未收录则回退中文）。</summary>
        internal static string Desc(string zhSection, string zhKey, string zhDescription)
        {
            return UseEnglish && Entries.TryGetValue(Index(zhSection, zhKey), out var e) ? e.Desc : zhDescription;
        }

        private static string Index(string zhSection, string zhKey) => zhSection + "|" + zhKey;
    }
}
