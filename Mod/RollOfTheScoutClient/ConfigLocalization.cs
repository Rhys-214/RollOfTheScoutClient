using System;
using System.Collections.Generic;
using System.Text;

namespace RollOfTheScoutClient
{
    /// <summary>
    /// 附属模组配置文件名称的**固定中英双语**规则（与主模组完全一致，不再随游戏语言变化）。
    ///
    /// · Section：英文在前、中文在后（<c>General通用</c>）；
    /// · Key：英文驼峰在前、中文在后（<c>classDisplayKey职业显示-按键</c>）；
    /// · Description：英文一行、中文一行。
    /// <see cref="UseEnglish"/> 仍保留（供其它纯文本 UI 使用），但不再影响配置文件。
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
            { "通用|职业显示-模式", ("classDisplayMode", "Display mode of the class panel (values 1 / 2 / 3): 1 = show while the key is held and hide immediately on release; 2 = show for 5 seconds after a single press, then auto-hide; 3 = each press toggles between show and hide. Default 1. Legacy text values are still accepted.") },
            { "通用|头顶职业前缀", ("overheadClassPrefix", "Whether to rewrite the overhead player name as '[Class] Nickname', e.g. Alice -> [Explorer] Alice. '[Class]' is colored with that player's skin color; the nickname keeps its vanilla color. Class data comes from the host's broadcast; this switch only affects names seen on the local screen. Warning: this runs per frame and per player; if another overhead-name mod is installed (e.g. AlwaysDisplayPlayerName), both rewrite the text every frame and cause noticeable frame drops. Default false (off).") },
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

        /// <summary>中文 Section → **固定双语** Section：英文在前、中文在后（如 General通用）。</summary>
        internal static string Section(string zhSection)
        {
            if (Sections.TryGetValue(zhSection, out string en)
                && !string.IsNullOrEmpty(en)
                && !string.Equals(en, zhSection, StringComparison.Ordinal))
                return en + zhSection;
            return zhSection;
        }

        /// <summary>中文 Key → **固定双语** Key（英文按中文键的 '-' 分段、每段首字母大写，再加中文）。</summary>
        internal static string Key(string zhSection, string zhKey)
        {
            if (!Entries.TryGetValue(Index(zhSection, zhKey), out var e) || string.IsNullOrEmpty(e.Key)) return zhKey;
            if (string.Equals(e.Key, zhKey, StringComparison.OrdinalIgnoreCase)) return zhKey;
            // 英文在前、**中文键名完整保留在后**
            return BuildBilingualKey(zhKey, e.Key) + zhKey;
        }

        private static string BuildBilingualKey(string zhKey, string enKey)
        {
            int segCount = zhKey.Split('-').Length;
            var words = SplitCamelWords(enKey);
            if (segCount <= 1 || words.Count <= 1) return Capitalize(enKey);

            var segments = new List<string>(segCount);
            int per = (int)Math.Ceiling(words.Count / (float)segCount);
            int idx = 0;
            for (int s = 0; s < segCount && idx < words.Count; s++)
            {
                int take = Math.Min(per, words.Count - idx);
                segments.Add(Capitalize(string.Concat(words.GetRange(idx, take))));
                idx += take;
            }
            return string.Join("-", segments.ToArray());
        }

        private static string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpperInvariant(s[0]) + s.Substring(1);
        }

        private static List<string> SplitCamelWords(string s)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(s)) return list;

            int start = 0;
            for (int i = 1; i < s.Length; i++)
            {
                char c = s[i];
                char p = s[i - 1];
                bool boundary = char.IsUpper(c) && (char.IsLower(p) || char.IsDigit(p));
                if (!boundary && char.IsDigit(c) && !char.IsDigit(p)) boundary = true;
                if (!boundary && char.IsUpper(c) && char.IsUpper(p)
                    && i + 1 < s.Length && char.IsLower(s[i + 1])) boundary = true;
                if (!boundary) continue;

                list.Add(s.Substring(start, i - start));
                start = i;
            }
            list.Add(s.Substring(start));
            return list;
        }

        /// <summary>Description → **固定双语**：英文说明在前、中文说明在后（两行）。</summary>
        internal static string Desc(string zhSection, string zhKey, string zhDescription)
        {
            if (!Entries.TryGetValue(Index(zhSection, zhKey), out var e) || string.IsNullOrEmpty(e.Desc)) return zhDescription;
            return e.Desc + "\n" + zhDescription;
        }

        private static string Index(string zhSection, string zhKey) => zhSection + "|" + zhKey;
    }
}
