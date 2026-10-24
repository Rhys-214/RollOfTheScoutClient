namespace RollOfTheScoutClient
{
    /// <summary>
    /// 职业数值 → 「英文职业名 · 中文职业名」（如 <c>Qu · 区</c>）。
    ///
    /// 与主模组 <c>RollOfTheScout.ClassLedger.DescribeBilingual</c> 一一对应；
    /// 主机广播来的名称也已是同一格式（Kind 3），本表只作「尚未收到广播」时的本地兜底。
    /// 这里独立维护一份（而非引用主模组），是因为两模组必须能各自独立安装。
    /// </summary>
    public static class ClassNames
    {
        public static string Describe(int code)
        {
            switch (code)
            {
                case 1: return "Explorer · 先驱";
                case 2: return "Leader · 领袖";
                case 3: return "Logistician · 后勤";
                case 4: return "Doctor · 医生";
                case 5: return "Assassin · 刺客";
                case 6: return "Pandora's Child · 潘多拉之子";
                case 7: return "Necromancer · 死灵法师";
                case 8: return "M · M";
                case 9: return "Savage · 野人";
                case 10: return "Scout · 童军";
                case 11: return "StandardBearer · 旗手";
                case 12: return "Dancer · 舞王";
                case 13: return "Daihao · 待皓";
                case 14: return "Saint · 圣人";
                case 15: return "Mage · 法师";
                case 16: return "Challenger · 挑战者";
                case 17: return "MasterChef · 特级厨师";
                case 18: return "Cook · 炊事员";
                case 19: return "Pacifist · 和平主义者";
                case 20: return "Hamster · 仓鼠";
                case 21: return "Firefighter · 救火队员";
                case 22: return "Narcissist · 自恋狂";
                case 23: return "LoneWolf · 独狼";
                case 24: return "Gunner · 炮手";
                case 25: return "Band · 乐队成员";
                case 26: return "MilesSoul · 迈尔斯残魂";
                case 27: return "Qu · 区";
                case 28: return "Astronaut · 太空人";
                case 29: return "Langwang · 榔王";
                case 30: return "Programmer · 程序员";
                default: return "None · 无职业";
            }
        }
    }
}
