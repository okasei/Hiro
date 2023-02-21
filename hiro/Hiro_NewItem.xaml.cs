using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hiro
{
    /// <summary>
    /// Hiro_NewItem.xaml の相互作用ロジック
    /// </summary>
    public partial class Hiro_NewItem : Page
    {
        private Hiro_MainUI? Hiro_Main = null;
        internal int index = -1;
        public Hiro_NewItem(Hiro_MainUI? Parent)
        {
            InitializeComponent();
            Hiro_Main = Parent;
            Hiro_Initialize();
            Loaded += delegate
            {
                HiHiro();
            };
        }

        public void HiHiro()
        {
            var animation = !Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("0");
            Storyboard sb = new();
            if (Hiro_Utils.Read_Ini(App.dconfig, "Config", "Ani", "2").Equals("1"))
            {
                Hiro_Utils.AddPowerAnimation(1, ntn1, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, ntn2, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, ntn3, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, ntn4, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(1, ntn5, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(2, ntn6, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(2, ntn7, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(2, ntn8, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, ntn9, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, ntnx, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, ntnx1, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, ntnx2, sb, -50, null);
                Hiro_Utils.AddPowerAnimation(3, ntnx, sb, -50, null);
            }

            if (!animation)
                return;
            Hiro_Utils.AddPowerAnimation(0, this, sb, 50, null);
            sb.Begin();
        }

        private void Hiro_Initialize()
        {
            Load_Color();
            Load_Translate();
            Load_Position();
        }
        public void Load_ComboBox()
        {
            modibox.Items.Clear();
            keybox.Items.Clear();
            string[] ars = { "nomodi", "altkey", "shiftkey", "ctrlkey", "winkey", "sakey", "cakey", "cskey", "wakey", "wskey", "wckey", "csakey", "wsakey", "wcakey", "wcsakey" };
            foreach (var arss in ars)
            {
                modibox.Items.Add(new ComboBoxItem()
                {
                    Content = Hiro_Utils.Get_Translate(arss)
                });
            }
            keybox.Items.Add(new ComboBoxItem()
            {
                Content = Hiro_Utils.Get_Translate("novkey")
            });
            string[] crs = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            foreach (var crss in crs)
            {
                keybox.Items.Add(new ComboBoxItem()
                {
                    Content = Hiro_Utils.Get_Translate("charkey").Replace("%c", crss)
                });
            }
            for (int nuss = 0; nuss < 10; nuss++)
            {
                keybox.Items.Add(new ComboBoxItem()
                {
                    Content = Hiro_Utils.Get_Translate("numkey").Replace("%n", nuss.ToString())
                });
            }
            for (int nuss = 1; nuss < 13; nuss++)
            {
                keybox.Items.Add(new ComboBoxItem()
                {
                    Content = Hiro_Utils.Get_Translate("fnkey").Replace("%f", "F" + nuss.ToString())
                });
            }
            string[] extras = { "space", "esc", "leftkey", "upkey", "rightkey", "downkey" };
            foreach(var ext in extras)
            {
                keybox.Items.Add(new ComboBoxItem()
                {
                    Content = Hiro_Utils.Get_Translate(ext)
                });
            }
            foreach (var obj in modibox.Items)
            {
                if (obj is ComboBoxItem mi)
                    Hiro_Utils.Set_Control_Location(mi, "moditem", location: false);
            }
            foreach (var obj in keybox.Items)
            {
                if (obj is ComboBoxItem mi)
                    Hiro_Utils.Set_Control_Location(mi, "vkeyitem", location: false);
            }
        }

        public void Load_Color()
        {
            Resources["AppFore"] = new SolidColorBrush(App.AppForeColor);
            Resources["AppAccent"] = new SolidColorBrush(Hiro_Utils.Color_Transparent(App.AppAccentColor, App.trval));
        }

        public void Load_Translate()
        {
            ntn1.Content = Hiro_Utils.Get_Translate("filename");
            ntn2.Content = Hiro_Utils.Get_Translate("hide");
            ntn3.Content = Hiro_Utils.Get_Translate("unformat");
            ntn4.Content = Hiro_Utils.Get_Translate("ban");
            ntn5.Content = Hiro_Utils.Get_Translate("select");
            ntn6.Content = Hiro_Utils.Get_Translate("quot");
            ntn7.Content = Hiro_Utils.Get_Translate("explorer");
            ntn8.Content = Hiro_Utils.Get_Translate("openfile");
            klabel.Content = Hiro_Utils.Get_Translate("HotKey");
            ntn9.Content = Hiro_Utils.Get_Translate("reset");
            ntnx.Content = Hiro_Utils.Get_Translate("clear");
            ntnx1.Content = Hiro_Utils.Get_Translate("ok");
            ntnx2.Content = Hiro_Utils.Get_Translate("cancel");
            glabel.Content = Hiro_Utils.Get_Translate("itemname");
            glabel2.Content = Hiro_Utils.Get_Translate("cmd");
        }

        public void Load_Position()
        {
            Hiro_Utils.Set_Control_Location(ntn1, "filename", right: true);
            Hiro_Utils.Set_Control_Location(ntn2, "hide", right: true);
            Hiro_Utils.Set_Control_Location(ntn3, "unformat", right: true);
            Hiro_Utils.Set_Control_Location(ntn4, "ban", right: true);
            Hiro_Utils.Set_Control_Location(ntn5, "select", right: true);
            Hiro_Utils.Set_Control_Location(ntn6, "quot", right: true);
            Hiro_Utils.Set_Control_Location(ntn7, "explorer", right: true);
            Hiro_Utils.Set_Control_Location(ntn8, "openfile", right: true);
            Hiro_Utils.Set_Control_Location(ntn9, "reset", bottom: true);
            Hiro_Utils.Set_Control_Location(ntnx, "clear", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(ntnx1, "ok", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(ntnx2, "cancel", bottom: true, right: true);
            Hiro_Utils.Set_Control_Location(tb7, "nametb");
            Hiro_Utils.Set_Control_Location(tb8, "cmdtb");
            Hiro_Utils.Set_Control_Location(klabel, "hotkey");
            Hiro_Utils.Set_Control_Location(modibox, "modifier");
            Hiro_Utils.Set_Control_Location(modiname, "modifier");
            Hiro_Utils.Set_Control_Location(keybox, "vkey");
            Hiro_Utils.Set_Control_Location(keylabel, "vkey");
            Hiro_Utils.Set_Control_Location(glabel, "itemname");
            Hiro_Utils.Set_Control_Location(glabel2, "command");
            Hiro_Utils.Set_Control_Location(tb9, "detailtb");
        }

        private void Ntn1_Click(object sender, RoutedEventArgs e)
        {
            var val = tb8.Text;
            if (val.EndsWith(":\\"))
            {
                val = val[0..^2];
            }
            else if (val.EndsWith("\\"))
            {
                val = val[0..^1];
                val = val[(val.LastIndexOf("\\") + 1)..];
            }
            else
            {
                var poi = val.LastIndexOf('.');
                var slas = val.LastIndexOf("\\");
                if (poi != -1 && poi > slas)
                {
                    val = val[(val.LastIndexOf("\\") + 1)..];
                    val = val[..val.LastIndexOf(".")];
                }
                else
                {
                    val = val[(val.LastIndexOf("\\") + 1)..];
                }
            }
            tb7.Text = val;
        }

        private void Ntn2_Click(object sender, RoutedEventArgs e)
        {
            if (tb7.Text.IndexOf("[H]") + tb7.Text.IndexOf("[h]") > -2)
            {
                tb7.Text = tb7.Text.Replace("[h]", "").Replace("[H]", "");
            }
            else
            {
                tb7.Text = "[H]" + tb7.Text;
            }
        }

        private void Ntn3_Click(object sender, RoutedEventArgs e)
        {
            var val = tb7.SelectionStart;
            tb7.Text = string.Concat(tb7.Text.AsSpan(0, val), "[\\\\]", tb7.Text.AsSpan(val));
            tb7.SelectionStart = val + 4;
        }

        private void Ntn4_Click(object sender, RoutedEventArgs e)
        {
            if (tb7.Text.IndexOf("[N]") + tb7.Text.IndexOf("[n]") > -2)
            {
                tb7.Text = tb7.Text.Replace("[N]", "").Replace("[n]", "");
            }
            else
            {
                tb7.Text = "[N]" + tb7.Text;
            }
        }

        private void Ntn5_Click(object sender, RoutedEventArgs e)
        {
            if (tb7.Text.IndexOf("[S]") + tb7.Text.IndexOf("[s]") > -2)
            {
                tb7.Text = tb7.Text.Replace("[S]", "").Replace("[s]", "");
            }
            else
            {
                tb7.Text = "[S]" + tb7.Text;
            }
        }

        private void Ntn6_Click(object sender, RoutedEventArgs e)
        {
            var val = tb8.SelectionStart;
            var val2 = tb8.SelectionLength;
            var str = tb8.SelectedText;
            if (val2 > 0)
            {
                if (str.StartsWith("\"") && str.EndsWith("\""))
                {
                    tb8.Text = string.Concat(tb8.Text.AsSpan(0, val), str.AsSpan(1, str.Length - 2), tb8.Text.AsSpan(val + val2));
                    tb8.SelectionStart = val + 1;
                    tb8.Select(val, str.Length - 2);
                }
                else
                {
                    tb8.Text = tb8.Text[..val] + "\"" + tb8.Text.Substring(val, val2) + "\"" + tb8.Text[(val + val2)..];
                    tb8.SelectionStart = val + 1;
                    tb8.Select(val, str.Length + 2);
                }

            }
            else
            {
                tb8.Text = string.Concat(tb8.Text.AsSpan(0, val), "\"\"", tb8.Text.AsSpan(val));
                tb8.SelectionStart = val + 1;
            }
        }

        private void Ntn7_Click(object sender, RoutedEventArgs e)
        {
            if (tb8.Text.StartsWith("explorer "))
                tb8.Text = tb8.Text[9..];
            else
                tb8.Text = "explorer " + tb8.Text;
        }

        private void Ntn8_Click(object sender, RoutedEventArgs e)
        {
            var strFileName = "";
            Microsoft.Win32.OpenFileDialog ofd = new();
            ofd.Filter = Hiro_Utils.Get_Translate("allfiles") + "|*.*";
            ofd.ValidateNames = true; // 验证用户输入是否是一个有效的Windows文件名
            ofd.CheckFileExists = true; //验证路径的有效性
            ofd.CheckPathExists = true;//验证路径的有效性
            ofd.Title = Hiro_Utils.Get_Translate("openfile") + " - " + App.AppTitle;
            if (ofd.ShowDialog() == true) //用户点击确认按钮，发送确认消息
            {
                strFileName = ofd.FileName;//获取在文件对话框中选定的路径或者字符串

            }

            if (!System.IO.File.Exists(strFileName))
                return;
            if (strFileName.ToLower().EndsWith(".lnk"))
            {
                IWshRuntimeLibrary.WshShell shell = new();
                var lnkPath = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(strFileName);
                strFileName = lnkPath.TargetPath.Equals("") ? strFileName : lnkPath.TargetPath;
            }
            if (strFileName.IndexOf(" ") != -1)
                strFileName = "\"" + strFileName + "\"";
            strFileName = Hiro_Utils.Anti_Path_Prepare(strFileName);
            tb8.Text = strFileName;
            strFileName = strFileName[(strFileName.LastIndexOf("\\") + 1)..];
            if (strFileName.LastIndexOf(".") != -1)
                strFileName = strFileName[..strFileName.LastIndexOf(".")];
            if (tb7.Text == "")
                tb7.Text = strFileName;
        }

        private void Ntn9_Click(object sender, RoutedEventArgs e)
        {
            tb7.Text = App.cmditems[index].Name;
            tb8.Text = App.cmditems[index].Command;
        }

        private void Ntnx_Click(object sender, RoutedEventArgs e)
        {
            tb7.Text = "";
            tb8.Text = "";
        }

        private void Ntnx2_Click(object sender, RoutedEventArgs e)
        {
            tb7.Text = "";
            tb8.Text = "";
            if (Hiro_Main != null)
                Hiro_Main.Set_Label(Hiro_Main.itemx);
        }

        private void Ntnx1_Click(object sender, RoutedEventArgs e)
        {
            if (tb7.Text.Equals(string.Empty) || tb8.Text.Equals(string.Empty))
            {
                return;
            }
            var hk = Hiro_Utils.Index_Modifier(true, modibox.SelectedIndex).ToString() + "," + Hiro_Utils.Index_vKey(true, keybox.SelectedIndex).ToString();
            if (ntn9.Visibility == Visibility.Hidden)
            {
                var i = App.cmditems.Count + 1;
                var p = (i % 10 == 0) ? i / 10 : i / 10 + 1;
                App.cmditems.Add(new Cmditem(p, i, tb7.Text, tb8.Text, hk));
                Hiro_Utils.Write_Ini(App.dconfig, i.ToString(), "Title", tb7.Text);
                Hiro_Utils.Write_Ini(App.dconfig, i.ToString(), "Command", "(" + tb8.Text + ")");
                Hiro_Utils.Write_Ini(App.dconfig, i.ToString(), "HotKey", hk);
                if (modibox.SelectedIndex > 0 && keybox.SelectedIndex > 0)
                {
                    try
                    {
                        if (App.wnd != null)
                        {
                            Hiro_Utils.RegisterKey((uint)Hiro_Utils.Index_Modifier(true, modibox.SelectedIndex), (Key)Hiro_Utils.Index_vKey(true, keybox.SelectedIndex), i);
                        }
                    }
                    catch (Exception ex)
                    {
                        Hiro_Utils.LogError(ex, "Hiro.Exception.Hotkey.RegisterFunc");
                    }
                }
            }
            else
            {
                App.cmditems[index].Name = tb7.Text;
                App.cmditems[index].Command = tb8.Text;
                Hiro_Utils.Write_Ini(App.dconfig, (index + 1).ToString(), "Title", tb7.Text);
                Hiro_Utils.Write_Ini(App.dconfig, (index + 1).ToString(), "Command", "(" + tb8.Text + ")");
                Hiro_Utils.Write_Ini(App.dconfig, (index + 1).ToString(), "HotKey", hk);
                if (!App.cmditems[index].HotKey.Equals(hk))
                {
                    App.cmditems[index].HotKey = hk;
                    var keyid = Hiro_Utils.FindHotkeyById(index);
                    Hiro_Utils.UnregisterKey(keyid);
                    if (modibox.SelectedIndex > 0 && keybox.SelectedIndex > 0)
                    {
                        try
                        {
                            if (App.wnd != null)
                            {
                                Hiro_Utils.RegisterKey((uint)Hiro_Utils.Index_Modifier(true, modibox.SelectedIndex), (Key)Hiro_Utils.Index_vKey(true, keybox.SelectedIndex), index);
                            }
                        }
                        catch (Exception ex)
                        {
                            Hiro_Utils.LogError(ex, "Hiro.Exception.HotKey.RegisterFunc");
                        }
                    }
                }
            }
            tb7.Text = "";
            tb8.Text = "";
            App.Load_Menu();
            if (Hiro_Main != null)
                Hiro_Main.Set_Label(Hiro_Main.itemx);
        }
        private void Tb8_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data;
            var formats = data.GetData(DataFormats.FileDrop).GetType().ToString();
            if (formats.Equals("System.String[]"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as String[];
                if (info != null && info.Length > 0)
                {
                    tb8.Text = info[0];
                    if (tb7.Text == "")
                        Ntn1_Click(sender, e);
                }
            }
            else if (formats.Equals("System.String"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as String;
                if (info != null)
                {
                    tb8.Text = info;
                    if (tb7.Text == "")
                        Ntn1_Click(sender, e);
                }
            }
        }

        private void Tb7_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data;
            var formats = data.GetData(DataFormats.FileDrop).GetType().ToString();
            if (formats.Equals("System.String[]"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as String[];
                if (info != null && info.Length > 0)
                {
                    tb8.Text = info[0];
                    if (tb7.Text == "")
                        Ntn1_Click(sender, e);
                }
            }
            else if (formats.Equals("System.String"))
            {
                var info = e.Data.GetData(DataFormats.FileDrop) as String;
                if (info != null)
                {
                    tb8.Text = info;
                    if (tb7.Text == "")
                        Ntn1_Click(sender, e);
                }
            }
        }

        private void Tb7_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Tb8_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Tb8_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Tb7_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }


        private void Modibox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (modibox.SelectedIndex < 0)
            {
                modiname.Content = "";
                return;
            }
            if (modibox.Items.GetItemAt(modibox.SelectedIndex) is ComboBoxItem { Content: { } } cbi)
            {
                modiname.Content = cbi.Content.ToString();
            }
        }

        private void Keybox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (keybox.SelectedIndex < 0)
            {
                keylabel.Content = "";
                return;
            }
            if (keybox.Items.GetItemAt(keybox.SelectedIndex) is ComboBoxItem { Content: { } } cbi)
            {
                keylabel.Content = cbi.Content.ToString();
            }
        }

        private void Tb8_KeyDown(object sender, KeyEventArgs e)
        {
            InputMethod.SetPreferredImeState(tb8,
                tb8.Text.ToLower().StartsWith("key(") ? InputMethodState.Off : InputMethodState.On);

            if (!tb8.Text.ToLower().StartsWith("key(") || tb8.Text.EndsWith(")"))
                return;
            uint[] modi = { (uint)Key.LeftAlt, (uint)Key.RightAlt, 156, (uint)Key.LeftCtrl, (uint)Key.RightCtrl, (uint)Key.LeftShift, (uint)Key.RightShift, (uint)Key.LWin, (uint)Key.RWin };
            uint[] modint = { 1, 1, 1, 2, 2, 4, 4, 8, 8 };
            string[] allowed = { "", ",0,2,4,6,8,10,12,14,", ",0,1,4,8,5,9,12,13,", "", ",0,1,2,8,3,9,10,11,", "", "", "", ",0,1,2,4,3,5,6,7," };
            var uin = (uint)e.Key;
            bool ismodi = false;
            for (var mmi = 0; mmi < modi.Length; mmi++)
            {
                if (uin != modi[mmi])
                    continue;
                ismodi = true;
                uin = modint[mmi];
                break;
            }
            if (tb8.Text.IndexOf(",") == -1)
            {
                if (ismodi)
                    tb8.Text = tb8.Text[..4] + uin.ToString() + ",";
                else
                    tb8.Text = tb8.Text[..4] + "0," + uin.ToString() + ")";
            }
            else
            {
                if (!ismodi)
                    tb8.Text = tb8.Text[..(tb8.Text.IndexOf(",") + 1)] + uin.ToString() + ")";
                else
                {
                    uint modn = 0;
                    try
                    {
                        var ts = tb8.Text.Trim();
                        ts = ts.Substring(ts.IndexOf("(") + 1, ts.Length - ts.IndexOf("(") - 1);
                        ts = ts[0..^1];
                        modn = uint.Parse(ts);
                    }
                    catch (Exception ex)
                    {
                        Hiro_Utils.LogError(ex, "Hiro.Exception.Data.Parse");
                    }
                    if (allowed[uin].IndexOf("," + modn.ToString() + ",") != -1)
                    {
                        modn += uin;
                        tb8.Text = tb8.Text[..4] + modn.ToString() + ",";

                    }

                }
            }
            e.Handled = true;
        }

        private void Tb8_TextChanged(object sender, TextChangedEventArgs e)
        {
            tb9.Text = Hiro_Utils.Get_CMD_Translation(tb8.Text);
            tb9.Visibility = tb9.Text.Equals("") ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
