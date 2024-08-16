using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Hiro.Helpers
{
    internal class HClass
    {

        #region 命令项目定义
        public class Cmditem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            private int p;
            private int i;
            private string na;
            private string co;
            private string hk;

            public int Page
            {
                get { return p; }
                set
                {
                    p = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Page)));
                }
            }
            public int Id
            {
                get { return i; }
                set
                {
                    i = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
                }
            }
            public string Name
            {
                get { return na; }
                set
                {
                    na = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
            public string Command
            {
                get { return co; }
                set
                {
                    co = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Command)));
                }
            }

            public string HotKey
            {
                get { return hk; }
                set
                {
                    hk = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HotKey)));
                }
            }
            public Cmditem()
            {
                Page = -1;
                Id = -1;
                Name = string.Empty;
                Command = string.Empty;
                HotKey = string.Empty;
                p = -1;
                Id = -1;
                na = string.Empty;
                co = string.Empty;
                hk = string.Empty;
            }
            public Cmditem(int a, int b, string c, string d, string e)
            {
                Page = a;
                Id = b;
                Name = c;
                Command = d;
                HotKey = e;
                p = a;
                Id = b;
                na = c;
                co = d;
                hk = e;
            }
        }
        #endregion

        #region 日程项目定义
        public class Scheduleitem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            private int i;
            private string na;
            private string ti;
            private string co;
            public double re;
            public string Time
            {
                get { return ti; }
                set
                {
                    ti = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Time)));
                }
            }
            public int Id
            {
                get { return i; }
                set
                {
                    i = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
                }
            }
            public string Name
            {
                get { return na; }
                set
                {
                    na = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
            public string Command
            {
                get { return co; }
                set
                {
                    co = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Command)));
                }
            }

            public Scheduleitem()
            {
                Time = "19000101000000";
                Id = -1;
                Name = string.Empty;
                Command = string.Empty;
                re = -2.0;
                ti = "19000101000000";
                i = -1;
                na = string.Empty;
                co = string.Empty;
            }
            public Scheduleitem(int b, string a, string c, string d, double e)
            {
                Name = a;
                Id = b;
                Time = c;
                Command = d;
                re = e;
                na = a;
                i = b;
                ti = c;
                co = d;
            }
        }
        #endregion

        #region 通知窗口项目定义
        public class Hiro_AlarmWin
        {
            public Hiro_Alarm win;
            public int id;
            public Hiro_AlarmWin(Hiro_Alarm a, int b)
            {
                win = a;
                id = b;
            }
            public Hiro_AlarmWin(int a, Hiro_Alarm b)
            {
                win = b;
                id = a;
            }
        }
        #endregion

        #region 图标项目定义
        public class Hiro_Icon
        {
            public string Location = string.Empty;
            public BitmapImage? Image = null;
            public Hiro_Icon()
            {

            }
        }
        #endregion

        #region 通知项目定义
        public class Hiro_Notice
        {
            public string? title;
            public string msg;
            public int time;
            public Action? act;
            public Hiro_Icon? icon;
            public Hiro_Notice(string ms = "NULL", int ti = 1, string? tit = null, Action? ac = null, Hiro_Icon? icon = null)
            {
                msg = ms;
                time = ti;
                title = tit;
                act = ac;
                this.icon = icon;
            }
        }
        #endregion

        #region 语言项目定义
        public class Language : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            private string na;
            private string la;
            public string Name
            {
                get { return na; }
                set
                {
                    na = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }
            public string Langname
            {
                get { return la; }
                set
                {
                    la = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Langname)));
                }
            }

            public Language()
            {
                Name = "null";
                Langname = "null";
                na = "null";
                la = "null";
            }

            public Language(string Name, string LangName)
            {
                this.Name = Name;
                Langname = LangName;
                na = Name;
                la = LangName;
            }
        }
        #endregion

        #region 通信标准定义
        public class HiroApp
        {
            public int state = -1;
            public string? appID = null;
            public string? appPackage = null;
            public string? appName = null;
            public string? msg = null;
            public HiroApp(string? appID = null, string? appPackage = null, string? appName = null, string? msg = null, int state = 0)
            {
                this.appID = appID;
                this.appPackage = appPackage;
                this.appName = appName;
                this.msg = msg;
                this.state = state;
            }
            public bool CheckIntegrity()
            {
                if (appID == null || appPackage == null || appName == null || msg == null)
                    return true;
                else
                    return false;
            }
            public void Reset()
            {
                appID = null;
                appPackage = null;
                appName = null;
                msg = null;
                state = 0;
            }
            public override string ToString()
            {
                var ret = "ID: ";
                var re = appID ?? "null";
                ret = ret + re + ", Package: ";
                re = appPackage ?? "null";
                ret = ret + re + ", Name: ";
                re = appName ?? "null";
                ret = ret + re + ", Msg: ";
                re = msg ?? "null";
                ret += re;
                return ret;
            }
        }

        #endregion

        #region 热键项目定义
        public class HotKey
        {
            private int _keyid = -1;
            private int _itemid = -1;

            public int KeyID
            {
                get => _keyid; set => _keyid = value;
            }
            public int ItemID
            {
                get => _itemid; set => _itemid = value;
            }
        }
        #endregion
    }
}
