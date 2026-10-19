using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace RollOfTheScoutClient
{
    /// <summary>
    /// 附属模组入口：把「主机 RollOfTheScout 广播过来的职业显示」绘制到**客机自己的屏幕**上，
    /// 并给头顶玩家名称加上「【职业】」前缀。
    ///
    /// 开启 / 停用规则（关键需求）：
    /// · 只要本机安装了主模组 <c>com.peakmodding.RollOfTheScout</c>，本附属模组**完全不启用**
    ///   （主模组在客机上也会提供同样的显示功能）—— 因此主机（装了主模组）与「主客模组都装」的客机
    ///   都不会重复启用本模组，避免重复冲突与性能浪费；
    /// · 只有当本机**没有**主模组（典型：客机只装附属模组）时，本模组才工作；
    /// · 「客机只装主模组」时显示功能由主模组自己提供（见 RollOfTheScout 的客机接收模式）。
    /// </summary>
    [BepInPlugin(Guid, "RollOfTheScoutClient", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "com.peakmodding.RollOfTheScoutClient";

        /// <summary>主模组的程序集名 / DLL 文件名，用于「是否已安装主模组」的检测。</summary>
        private const string MainModAssemblyName = "com.peakmodding.RollOfTheScout";

        internal static ManualLogSource Log;

        /// <summary>本模组的 Harmony 实例（第三方兼容层也需要它来挂补丁）。</summary>
        internal static Harmony HarmonyInstance;

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;

            if (MainModInstalled())
            {
                Log.LogInfo(
                    "[RollOfTheScoutClient] 检测到本机已安装主模组 com.peakmodding.RollOfTheScout，" +
                    "本附属模组不启用（职业显示由主模组负责，避免重复冲突与性能浪费）。");
                return;
            }

            // 配置项按游戏语言本地化（见 ConfigLocalization）；绑定到插件自身的 Config，ModConfig 才能读到。
            ClientConfig.Initialize(Config);
            gameObject.AddComponent<ClassDisplayReceiver>();

            _harmony = new Harmony(Guid);
            HarmonyInstance = _harmony;
            var updateName = ResolveMethod(typeof(UIPlayerNames), "UpdateName", typeof(int));
            if (updateName == null)
            {
                Log.LogWarning("[RollOfTheScoutClient] 未找到 UIPlayerNames.UpdateName，头顶职业前缀将不可用（其余显示不受影响）。");
            }
            else
            {
                try
                {
                    _harmony.Patch(updateName, postfix: new HarmonyMethod(
                        AccessTools.Method(typeof(DisplayNamePatch), nameof(DisplayNamePatch.Postfix))));
                    Log.LogInfo("[RollOfTheScoutClient] 补丁成功：UIPlayerNames.UpdateName (头顶职业前缀)");
                }
                catch (System.Exception e)
                {
                    Log.LogError($"[RollOfTheScoutClient] 补丁失败：UIPlayerNames.UpdateName → {e.Message}");
                }
            }

            Log.LogInfo(
                "[RollOfTheScoutClient] 已加载。用于显示主机广播过来的职业分配情况、职业效果提示，以及头顶职业前缀。");

            // 第三方「头顶id显示」模组兼容层（BetterPlayerDistance / Sense of Direction）：
            // 这两个模组会隐藏原版名牌、改用自己绘制的标签，因此仅靠上面的原版名牌补丁在其标签上看不到前缀。
            // 兼容层以运行时反射挂补丁（软依赖，未安装则跳过），实际挂载由 ClassDisplayReceiver.Update 每帧驱动（限流重试）。
            PeakOverheadCompat.ThirdPartyOverheadNameCompat.Initialize(
                actor => ClientConfig.NamePrefix.Value && ClassDisplayReceiver.Instance != null
                    ? ClassDisplayReceiver.Instance.ClassNameOrEmptyOfActor(actor)
                    : string.Empty,
                msg => Log.LogWarning(msg));
        }

        private void OnDestroy()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchSelf();
                _harmony = null;
            }
        }

        /// <summary>
        /// 判断本机是否安装了主模组：优先检查 BepInEx 插件目录下是否存在主模组 DLL
        /// （与插件加载顺序无关、最可靠），退化到检查已加载的程序集。
        /// </summary>
        private static bool MainModInstalled()
        {
            try
            {
                string dir = Paths.PluginPath;
                if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                {
                    if (Directory.GetFiles(dir, MainModAssemblyName + ".dll", SearchOption.AllDirectories).Length > 0)
                        return true;
                }
            }
            catch { /* 目录不可访问等：忽略，走程序集兜底 */ }

            try
            {
                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (asm.GetName().Name == MainModAssemblyName) return true;
                }
            }
            catch { /* 忽略 */ }

            return false;
        }

        /// <summary>
        /// 按方法名宽松解析目标方法，优先返回「首个参数为 <paramref name="firstParam"/>」的重载。
        /// （与主模组 Plugin.cs 中的同名工具一致，用于应对签名微调。）
        /// </summary>
        private static MethodInfo ResolveMethod(System.Type type, string name, System.Type firstParam)
        {
            const BindingFlags flags =
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance | BindingFlags.Static;

            MethodInfo loose = null;
            foreach (var m in type.GetMethods(flags))
            {
                if (m.Name != name) continue;
                if (firstParam == null) return m;

                var ps = m.GetParameters();
                if (ps.Length >= 1 && ps[0].ParameterType == firstParam) return m;
                if (loose == null) loose = m;
            }
            return loose;
        }
    }
}
