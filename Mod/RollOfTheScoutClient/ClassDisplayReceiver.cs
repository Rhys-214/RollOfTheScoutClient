using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace RollOfTheScoutClient
{
    /// <summary>
    /// 接收主机 <c>RollOfTheScout</c> 广播的「职业显示」事件，并在**客机自己的屏幕**上原样显示：
    ///   · 职业分配面板（右上角，按键触发，内容与主机屏幕一致）；
    ///   · 职业效果提示 Toast（左上角，8 秒后消失，最多 5 条）；
    ///   · 头顶职业前缀「【职业】昵称」（见 <see cref="OverheadNamePrefix"/>）。
    ///
    /// 停用规则：本模组与主模组 <c>RollOfTheScout</c> **不共存**（见 <see cref="Plugin"/> 的安装检测）——
    /// 只要本机装了主模组，本附属模组就完全不启用；因此本组件运行时即代表「本机没有主模组」。
    /// 另：本机若成为主机（MasterClient），则没有广播来源，清空数据、不显示。
    ///
    /// 数据来源：主机用 Photon 自定义事件（码 197，见主模组 <c>RollOfTheScout.ClassDisplayNet</c>）发送，
    /// 格式 <c>object[]{ ModId(string), Kind(byte), Data(string) }</c>。本模组只接收、从不发送。
    /// </summary>
    public sealed class ClassDisplayReceiver : MonoBehaviour, IOnEventCallback
    {
        /// <summary>供头顶名称补丁（<see cref="DisplayNamePatch"/>）访问。</summary>
        public static ClassDisplayReceiver Instance { get; private set; }

        /// <summary>与主模组约定的模组标识 / 事件码（必须与 <c>RollOfTheScout.ClassDisplayNet</c> 完全一致）。</summary>
        private const string ModId = "com.peakmodding.RollOfTheScout";
        private const byte EventCode = 197;
        private const byte KindClassSnapshot = 1;
        private const byte KindToast = 2;
        private const byte KindClassNames = 3;

        // ---- 接收到的职业分配情况（ActorNumber → ClassType 数值，保持主机侧顺序）----
        private readonly List<KeyValuePair<int, int>> _remoteClasses = new List<KeyValuePair<int, int>>();
        private readonly Dictionary<int, int> _remoteClassMap = new Dictionary<int, int>();
        private bool _hasRemoteSnapshot;

        // ---- 接收到的「职业数值 → 职业名」对照表（主机广播）----
        // 有它之后，本模组**不需要内置职业名表**：主模组新增职业时本模组无需更新即可正确显示，
        // 不会出现「新职业显示成无职业」的问题；本地 ClassNames 仅作为旧版主模组下的兜底。
        private readonly Dictionary<int, string> _classNameByCode = new Dictionary<int, string>();

        // ---- 接收到的职业效果提示（Toast），与主机同样的 8 秒 / 最多 5 条规则 ----
        private readonly List<string> _toasts = new List<string>();
        private readonly List<float> _toastExpiries = new List<float>();
        private const float ToastDuration = 8f;
        private const int ToastMaxCount = 5;

        // ---- 按键显示「当前职业分配情况」（右侧面板，仅本机屏幕可见）----
        private bool _classListHeld;
        private bool _classListToggled;
        private float _classListPressUntil;
        private const float ClassListPressDuration = 5f;

        private GUIStyle _toastStyle;
        private GUIStyle _classListStyle;
        private GUIStyle _classListTitleStyle;

        private void Awake()
        {
            Instance = this;
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            if (Instance == this) Instance = null;
        }

        /// <summary>本机是否是「当前主机」：此时没有广播来源，本模组不显示。</summary>
        private static bool IsHostNow()
        {
            return PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient;
        }

        private void Update()
        {
            // 第三方「头顶id显示」模组兼容层：限流重试挂补丁（不依赖是否在房间 / 关卡内）。
            PeakOverheadCompat.ThirdPartyOverheadNameCompat.Tick(Plugin.HarmonyInstance);

            // 本机成为主机（房主离开后房主迁移）：没有广播来源 → 清空并跳过。
            if (IsHostNow())
            {
                _remoteClasses.Clear();
                _remoteClassMap.Clear();
                _hasRemoteSnapshot = false;
                _toasts.Clear();
                _toastExpiries.Clear();
                PushNativeOverlay();
                return;
            }

            ExpireToasts();
            UpdateClassListInput();

            // 回到机场（大厅）时清空，避免残留上一局的分配情况；主机此时也不再广播。
            if (!IsInGameplayScene() && (_remoteClasses.Count > 0 || _toasts.Count > 0))
            {
                _remoteClasses.Clear();
                _remoteClassMap.Clear();
                _hasRemoteSnapshot = false;
                _toasts.Clear();
                _toastExpiries.Clear();
            }

            PushNativeOverlay();
        }

        /// <summary>
        /// 把「职业效果提示」与「职业分配情况」推送到原生字体叠加层（每帧在 Update 里调用）。
        /// 叠加层未就绪时什么都不做，由 <see cref="OnGUI"/> 回退绘制。
        /// </summary>
        private void PushNativeOverlay()
        {
            if (!PeakNativeUi.NativeOverlay.Ensure()) return;

            PeakNativeUi.NativeOverlay.SetToast(_toasts);

            bool visible = ClassListVisible() && _hasRemoteSnapshot && _remoteClasses.Count > 0;
            List<string> lines = visible ? BuildPanelLines() : null;
            if (lines != null && lines.Count == 0) { lines = null; visible = false; }

            PeakNativeUi.NativeOverlay.SetPanel(
                $"[职业分配] 按 {ClientConfig.ClassListKey.Value} 显示", lines, visible);
        }

        /// <summary>构造职业面板各行文本（与主机端同一套措辞）。</summary>
        private List<string> BuildPanelLines()
        {
            var lines = new List<string>(_remoteClasses.Count);

            // 按主机广播的 ActorNumber → 昵称（与主机的 PhotonView.Owner.NickName 完全一致）
            var room = PhotonNetwork.CurrentRoom;
            foreach (var kv in _remoteClasses)
            {
                var player = room != null ? room.GetPlayer(kv.Key) : null;
                string name = player != null && !string.IsNullOrEmpty(player.NickName)
                    ? player.NickName : $"#{kv.Key}";
                lines.Add($"{name}：{NameOf(kv.Value)}");
            }
            return lines;
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent == null || photonEvent.Code != EventCode) return;
            if (IsHostNow()) return;

            // 只接受来自主机（MasterClient）的事件，避免误信其它客户端。
            var master = PhotonNetwork.MasterClient;
            if (master == null || photonEvent.Sender != master.ActorNumber) return;

            try
            {
                if (!(photonEvent.CustomData is object[] arr) || arr.Length < 3) return;
                if (!(arr[0] is string id) || id != ModId) return;

                byte kind = Convert.ToByte(arr[1]);
                string data = arr[2] as string;
                if (string.IsNullOrEmpty(data)) return;

                if (kind == KindClassSnapshot) SetSnapshot(data);
                else if (kind == KindClassNames) SetClassNames(data);
                else if (kind == KindToast) AddToast(data);
            }
            catch (Exception e)
            {
                Plugin.Log.LogWarning($"[RollOfTheScoutClient] 处理职业显示事件时异常：{e.Message}");
            }
        }

        /// <summary>解析 <c>"actor=class;…"</c> 并重建职业分配情况。</summary>
        private void SetSnapshot(string data)
        {
            _remoteClasses.Clear();
            _remoteClassMap.Clear();

            foreach (var part in data.Split(';'))
            {
                if (string.IsNullOrEmpty(part)) continue;
                int eq = part.IndexOf('=');
                if (eq <= 0) continue;
                if (int.TryParse(part.Substring(0, eq), out int actor) &&
                    int.TryParse(part.Substring(eq + 1), out int cls))
                {
                    _remoteClasses.Add(new KeyValuePair<int, int>(actor, cls));
                    _remoteClassMap[actor] = cls;
                }
            }
            _hasRemoteSnapshot = _remoteClasses.Count > 0;
        }

        /// <summary>解析 <c>"code=名称;…"</c> 并更新「职业数值 → 职业名」对照表。</summary>
        private void SetClassNames(string data)
        {
            foreach (var part in data.Split(';'))
            {
                if (string.IsNullOrEmpty(part)) continue;
                int eq = part.IndexOf('=');
                if (eq <= 0 || eq + 1 >= part.Length) continue;
                if (int.TryParse(part.Substring(0, eq), out int code))
                    _classNameByCode[code] = part.Substring(eq + 1);
            }
        }

        /// <summary>
        /// 职业数值 → 职业名：**优先用主机广播来的名称**（因此主模组新增职业时本模组无需更新），
        /// 本地表 <see cref="ClassNames"/> 仅作为「旧版主模组不广播名称」时的兜底。
        /// </summary>
        public string NameOf(int code)
        {
            if (_classNameByCode.TryGetValue(code, out var name) && !string.IsNullOrEmpty(name)) return name;
            return ClassNames.Describe(code);
        }

        /// <summary>按 ActorNumber 取职业名（空字符串 = 无职业 / 未收到），供第三方头顶id显示模组兼容层使用。</summary>
        public string ClassNameOrEmptyOfActor(int actor)
        {
            int code = ClassCodeOfActor(actor);
            return code == 0 ? string.Empty : NameOf(code);
        }

        /// <summary>取「本机所见」的某角色职业数值（0 = 无职业 / 未收到）。</summary>
        public int ClassCodeOf(Character character)
        {
            var owner = character != null && character.photonView != null ? character.photonView.Owner : null;
            if (owner != null && _remoteClassMap.TryGetValue(owner.ActorNumber, out int code)) return code;
            return 0;
        }

        /// <summary>
        /// 按 <b>ActorNumber</b> 取职业数值（供第三方头顶id显示模组兼容层使用：
        /// 那些模组手里只有 ActorNumber，没有角色引用）。
        /// </summary>
        public int ClassCodeOfActor(int actor)
        {
            return _remoteClassMap.TryGetValue(actor, out int code) ? code : 0;
        }

        /// <summary>「头顶职业前缀」每帧回调（由 <see cref="DisplayNamePatch"/> 调用）。</summary>
        public void OnOverheadNameTick(UIPlayerNames ui, int index)
        {
            if (!ClientConfig.NamePrefix.Value) return;
            if (ui == null) return;

            var arr = ui.playerNameText;
            if (arr == null || index < 0 || index >= arr.Length) return;

            var pn = arr[index];
            if (pn == null) return;

            var interactible = pn.characterInteractable;
            var character = interactible != null ? interactible.character : null;
            if (character == null) return;

            OverheadNamePrefix.Apply(pn, character, OverheadClassNameOf);
        }

        /// <summary>命中「头顶职业前缀」用的职业名查询（空字符串 = 无职业 / 未收到）。</summary>
        private string OverheadClassNameOf(Character character)
        {
            int code = ClassCodeOf(character);
            return code == 0 ? string.Empty : NameOf(code);
        }

        private void AddToast(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            if (_toasts.Count >= ToastMaxCount)
            {
                _toasts.RemoveAt(0);
                _toastExpiries.RemoveAt(0);
            }
            _toasts.Add(text);
            _toastExpiries.Add(Time.time + ToastDuration);
        }

        private void ExpireToasts()
        {
            if (_toasts.Count == 0) return;
            float now = Time.time;
            for (int i = _toasts.Count - 1; i >= 0; i--)
            {
                if (now >= _toastExpiries[i])
                {
                    _toasts.RemoveAt(i);
                    _toastExpiries.RemoveAt(i);
                }
            }
        }

        /// <summary>当前是否处于游戏关卡内（客机侧读取原版 <c>GameHandler.IsInGameplayScene</c>）。</summary>
        private static bool IsInGameplayScene()
        {
            try { return GameHandler.IsInGameplayScene; }
            catch { return false; }
        }

        /// <summary>读取「职业显示」按键，更新面板可见状态（逻辑与主机模组一致）。</summary>
        private void UpdateClassListInput()
        {
            KeyCode key = ClientConfig.ParseKey();
            int kind = ClientConfig.ClassListModeKind();
            bool down = Input.GetKeyDown(key);
            bool held = Input.GetKey(key);

            if (kind == 1)                 // 按住显示
            {
                _classListHeld = held;
            }
            else if (kind == 2)            // 按下切换
            {
                if (down) _classListToggled = !_classListToggled;
            }
            else                           // 按下显示若干秒
            {
                if (down) _classListPressUntil = Time.time + ClassListPressDuration;
            }
        }

        private bool ClassListVisible()
        {
            int kind = ClientConfig.ClassListModeKind();
            if (kind == 1) return _classListHeld;      // 按住显示
            if (kind == 2) return _classListToggled;   // 按下切换
            return Time.time < _classListPressUntil;   // 按下显示若干秒
        }

        private void OnGUI()
        {
            // 优先用「原生字体叠加层」绘制（与游戏同款 TMP 字体）；
            // 字体尚未解析出来时，回退到旧的 IMGUI 自绘（Arial），保证功能不中断。
            if (PeakNativeUi.NativeOverlay.Ready) return;

            DrawToasts();
            DrawClassList();
        }

        private void DrawClassList()
        {
            if (IsHostNow()) return;
            if (!ClassListVisible()) return;
            if (!_hasRemoteSnapshot) return;

            var lines = new List<string>(_remoteClasses.Count + 1);
            lines.Add($"[职业分配] 按 {ClientConfig.ClassListKey.Value} 显示");
            lines.AddRange(BuildPanelLines());

            if (_classListStyle == null)
            {
                int size = Mathf.Max(16, Mathf.RoundToInt(Screen.height / 48f));
                _classListTitleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = size,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                };
                _classListTitleStyle.normal.textColor = new Color(1f, 0.9f, 0.4f);
                _classListStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = size,
                    alignment = TextAnchor.MiddleLeft,
                };
                _classListStyle.normal.textColor = Color.white;
            }

            float w = Screen.width * 0.32f;
            float lineH = _classListStyle.fontSize * 1.6f;
            float h = lineH * lines.Count + 12f;
            var box = new Rect(Screen.width - w - 12f, 12f, w, h);

            GUI.color = new Color(0f, 0f, 0f, 0.6f);
            GUI.DrawTexture(box, Texture2D.whiteTexture);
            GUI.color = Color.white;

            for (int i = 0; i < lines.Count; i++)
            {
                var line = new Rect(box.x + 10f, box.y + 6f + lineH * i, box.width - 20f, lineH);
                GUI.Label(line, lines[i], i == 0 ? _classListTitleStyle : _classListStyle);
            }
        }

        private void DrawToasts()
        {
            if (_toasts.Count == 0) return;

            if (_toastStyle == null)
            {
                _toastStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = Mathf.Max(16, Mathf.RoundToInt(Screen.height / 48f)),
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                };
                _toastStyle.normal.textColor = Color.white;
            }

            float w = Screen.width * 0.45f;
            float lineH = _toastStyle.fontSize * 1.6f;
            float h = lineH * _toasts.Count + 12f;
            var box = new Rect(12f, 12f, w, h);
            GUI.color = new Color(0f, 0f, 0f, 0.6f);
            GUI.DrawTexture(box, Texture2D.whiteTexture);
            GUI.color = Color.white;

            for (int i = 0; i < _toasts.Count; i++)
            {
                var line = new Rect(box.x + 10f, box.y + 6f + lineH * i, box.width - 20f, lineH);
                GUI.Label(line, _toasts[i], _toastStyle);
            }
        }
    }
}
