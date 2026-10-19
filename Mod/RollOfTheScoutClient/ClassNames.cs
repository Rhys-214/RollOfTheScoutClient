namespace RollOfTheScoutClient
{
    /// <summary>
    /// 职业数值 → 中文职业名（与主模组 <c>RollOfTheScout.ClassLedger.Describe</c> 一一对应）。
    ///
    /// 这里独立维护一份（而非引用主模组），是因为两模组必须能各自独立安装 —— 客机可能只装本附属模组。
    /// 若主模组新增 / 改名职业，需同步更新本表。
    /// </summary>
    public static class ClassNames
    {
        public static string Describe(int code)
        {
            switch (code)
            {
                case 1: return "先驱";
                case 2: return "领袖";
                case 3: return "后勤";
                case 4: return "医生";
                case 5: return "刺客";
                case 6: return "潘多拉之子";
                case 7: return "死灵法师";
                case 8: return "M";
                case 9: return "野人";
                case 10: return "童军";
                case 11: return "旗手";
                case 12: return "舞王";
                case 13: return "待皓";
                case 14: return "圣人";
                case 15: return "法师";
                case 16: return "挑战者";
                case 17: return "特级厨师";
                case 18: return "炊事员";
                case 19: return "和平主义者";
                case 20: return "仓鼠";
                case 21: return "救火队员";
                case 22: return "自恋狂";
                case 23: return "独狼";
                case 24: return "炮手";
                case 25: return "乐队成员";
                case 26: return "迈尔斯残魂";
                case 27: return "区";
                case 28: return "太空人";
                case 29: return "榔王";
                case 30: return "程序员";
                default: return "无职业";
            }
        }
    }
}
