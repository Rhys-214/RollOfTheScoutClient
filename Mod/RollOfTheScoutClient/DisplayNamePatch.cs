namespace RollOfTheScoutClient
{
    /// <summary>
    /// 「头顶职业前缀」补丁：把头顶玩家名称改写成「【职业】昵称」。
    ///
    /// 挂 <c>UIPlayerNames.UpdateName(int index, Vector3 position, bool visible, int speakingAmplitude)</c>
    /// 的 Postfix —— 该方法由 <c>IsLookedAt.Update()</c> 对**场上每个其它玩家每帧**调用，是唯一
    /// 稳定、每帧都会经过、且能拿到「该名称对应角色」的挂点。
    ///
    /// ⚠ 纯本地 UI 改动：只影响**本机屏幕**上看到的名字；职业数据来自主机广播（见
    /// <see cref="ClassDisplayReceiver.ClassCodeOf"/>）。
    /// </summary>
    public static class DisplayNamePatch
    {
        public static void Postfix(UIPlayerNames __instance, int index)
        {
            var receiver = ClassDisplayReceiver.Instance;
            if (receiver == null) return;
            try
            {
                receiver.OnOverheadNameTick(__instance, index);
            }
            catch (System.Exception e)
            {
                Plugin.Log.LogError($"[RollOfTheScoutClient] 头顶职业前缀·异常：{e.Message}");
            }
        }
    }
}
