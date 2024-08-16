using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static Hiro.APIs.AHotKeys;

namespace Hiro.Helpers
{
    internal class HHotKeys
    {
        /// <summary>
        /// 序号从 0 开始
        /// </summary>
        internal static List<HClass.HotKey> hotkeys = new();
        internal static List<int> unused = new();
        #region 热键相关

        private static int FindFirstUnusedKeyID()
        {
            if (unused.Count > 0)
            {
                var ret = unused[0];
                unused.RemoveAt(0);
                return ret;
            }
            return hotkeys.Count;
        }

        public static int RegisterKey(uint modi, System.Windows.Input.Key id, int cid)
        {
            var vk = System.Windows.Input.KeyInterop.VirtualKeyFromKey(id);
            var kid = FindFirstUnusedKeyID();
            if (!RegisterHotKey(App.WND_Handle, kid, modi, (uint)vk))
            {
                string msg = "";
                IntPtr tempptr = IntPtr.Zero;
                int sa = Marshal.GetLastWin32Error();
                _ = FormatMessage(0x1300, ref tempptr, sa, 0, ref msg, 255, ref tempptr);
                Hiro_Utils.RunExe("notify(" + HText.Get_Translate("regfailed").Replace("%n", sa.ToString()) + ",2)");
                HLogger.LogError(new NotSupportedException(), $"Hiro.Exception.Hotkey.Register{Environment.NewLine}Extra: {sa} - {msg.Replace(Environment.NewLine, "")}");
            }
            hotkeys.Add(new()
            {
                KeyID = kid,
                ItemID = cid
            });
            return kid;
        }

        public static bool UnregisterKey(int id)
        {
            if (id < 0)
                return false;
            var key = hotkeys.Where(x => x.KeyID == id).FirstOrDefault();
            if (key == null)
                return false;
            bool a = UnregisterHotKey(App.WND_Handle, key.KeyID);
            unused.Add(key.KeyID);
            hotkeys.Remove(key);
            if (!a)
            {
                string msg = "";
                IntPtr tempptr = IntPtr.Zero;
                int sa = Marshal.GetLastWin32Error();
                _ = FormatMessage(0x1300, ref tempptr, sa, 0, ref msg, 255, ref tempptr);
                Hiro_Utils.RunExe("notify(" + HText.Get_Translate("unregfailed").Replace("%n", sa.ToString()) + ",2)");
                HLogger.LogError(new NotSupportedException(), $"Hiro.Exception.Hotkey.Unregister{Environment.NewLine}Extra: {sa} - {msg.Replace(Environment.NewLine, "")}");
            }
            else
                HLogger.LogtoFile("[REGISTER]Successfully unregistered.");
            return a;
        }

        public static int Index_Modifier(bool direction, int val)
        {
            //all provide!
            //alt - 1
            //shft - 2
            //ctrl - 4
            //win - 8
            //shit+alt - 3
            //ctrl +alt - 5
            //ctrl+shift = 6
            //win + alt = 9
            //win + shift = 10
            //win + ctrl = 12

            //ctrl+alt+shift = 7
            //win+alt+shift = 11
            //win+ctrl+alt = 13

            //win+ctrl+shift+alt = 15
            int[] mo = { 0, 1, 2, 4, 8, 3, 5, 6, 9, 10, 12, 7, 11, 13, 15 };
            if (direction)
            {
                if (val > -1 && val < mo.Length)
                    return mo[val];
            }
            else
            {
                for (int mos = 0; mos < mo.Length; mos++)
                {
                    if (mo[mos] == val)
                    {
                        return mos;
                    }
                }
            }
            return 0;
        }
        public static int Index_vKey(bool direction, int val)
        {
            int[] mo = { 0, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69,
                                34, 35, 36, 37, 38, 39, 40, 41, 42, 43,
                                90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101,
                                18, 13,
                                23, 24, 25, 26};
            if (direction)
            {
                if (val > -1 && val < mo.Length)
                    return mo[val];
            }
            else
            {
                for (int mos = 0; mos < mo.Length; mos++)
                {
                    if (mo[mos] == val)
                    {
                        return mos;
                    }
                }
            }
            return 0;
        }

        public static bool UnregisterIfExist(int itemID)
        {
            var key = hotkeys.Where(x => x.ItemID == itemID).FirstOrDefault();
            if (key == null)
                return true;
            ///如果存在, 需要将其它的大于它的项目向前移动一位
            for (int i = 0; i < hotkeys.Count; i++)
            {
                if (hotkeys[i].ItemID > itemID)
                    hotkeys[i].ItemID--;
            }
            return UnregisterKey(key.KeyID);
        }

        public static bool SwitchIfExist(int first, int second)
        {
            var key1 = hotkeys.Where(x => x.ItemID == first).FirstOrDefault();
            var key2 = hotkeys.Where(x => x.ItemID == second).FirstOrDefault();
            if (key2 == null && key2 == null)
                return true;
            ///只存在一个
            if (key1 == null)
            {
                ///key2 存在, 则把 key2 的 itemID 设置成 first
                key2.ItemID = first;
            }
            else if (key2 == null)
            {
                key1.ItemID = second;
            }
            else
            {
                key1.ItemID = second;
                key2.ItemID = first;
            }
            return true;
        }

        /// <summary>
        /// 根据 itemID, 寻找 keyID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int FindHotkeyByItemId(int id)
        {
            var key = hotkeys.Where(x => x.ItemID == id).FirstOrDefault();
            if (key != null)
                return key.KeyID;
            return -1;
        }

        /// <summary>
        /// 根据 keyID, 寻找 itemID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int FindHotkeyByKeyId(int id)
        {
            var key = hotkeys.Where(x => x.KeyID == id).FirstOrDefault();
            if (key != null)
                return key.ItemID;
            return -1;
        }
        #endregion
    }
}
