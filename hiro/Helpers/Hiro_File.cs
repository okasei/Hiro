using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using static hiro.Helpers.Hiro_Class;

namespace hiro.Helpers
{
    internal class Hiro_File
    {
        internal static void MoveFile(string? path, List<string> parameter)
        {
            if (!File.Exists(parameter[0]))
            {
                try
                {
                    CreateFolder(parameter[1]);
                    Directory.Move(parameter[0], parameter[1]);
                }
                catch (Exception ex)
                {
                    App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("failed"), 2, Hiro_Utils.Get_Translate("file")));
                    Hiro_Utils.LogError(ex, $"Hiro.Exception.IO.Move{Environment.NewLine}Path: {path}");
                }
                return;
            }
            try
            {
                CreateFolder(parameter[1]);
                File.Move(parameter[0], parameter[1]);
            }
            catch (Exception ex)
            {
                App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("failed"), 2, Hiro_Utils.Get_Translate("file")));
                Hiro_Utils.LogError(ex, $"Hiro.Exception.IO.Move{Environment.NewLine}Path: {path}");
            }
        }

        internal static void CopyFile(string? path, List<string> parameter)
        {
            if (!File.Exists(parameter[0]))
            {
                if (Directory.Exists(parameter[0]))
                    try
                    {
                        CopyDirectory(parameter[0], parameter[1]);
                    }
                    catch (Exception ex)
                    {
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("failed"), 2, Hiro_Utils.Get_Translate("file")));
                        Hiro_Utils.LogError(ex, $"Hiro.Exception.IO.Copy{Environment.NewLine}Path: {path}");
                    }
                else
                {
                    App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("syntax"), 2, Hiro_Utils.Get_Translate("file")));
                    Hiro_Utils.LogError(new FileNotFoundException(), $"Hiro.Exception.IO.Copy{Environment.NewLine}Path: {path}");
                }
                return;
            }
            try
            {
                CreateFolder(parameter[1]);
                File.Copy(parameter[0], parameter[1]);
            }
            catch (Exception ex)
            {
                App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("failed"), 2, Hiro_Utils.Get_Translate("file")));
                Hiro_Utils.LogError(ex, $"Hiro.Exception.IO.Copy{Environment.NewLine}Path: {path}");
            }
        }

        internal static void DeleteFile(string? path, List<string> parameter)
        {
            if (!File.Exists(parameter[0]))
            {
                if (Directory.Exists(parameter[0]))
                    try
                    {
                        Directory.Delete(parameter[0], true);
                    }
                    catch (Exception ex)
                    {
                        App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("failed"), 2, Hiro_Utils.Get_Translate("file")));
                        Hiro_Utils.LogError(ex, $"Hiro.Exception.IO.Delete{Environment.NewLine}Path: {path}");
                    }
                else
                    Hiro_Utils.LogtoFile($"[WARNING]Hiro.Warning.IO.Delete: Warning at {path} | Details: {Hiro_Utils.Get_Translate("filenotexist")}");
                return;
            }
            try
            {
                File.Delete(parameter[0]);
            }
            catch (Exception ex)
            {
                App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("failed"), 2, Hiro_Utils.Get_Translate("file")));
                Hiro_Utils.LogError(ex, $"Hiro.Exception.IO.Delete{Environment.NewLine}Path: {path}");
            }
        }
        private static void CopyDirectory(string srcdir, string desdir)
        {
            if (srcdir.EndsWith("\\"))
                srcdir = srcdir[0..^1];
            if (desdir.ToLower().StartsWith(srcdir.ToLower()))
            {
                App.Notify(new Hiro_Notice(Hiro_Utils.Get_Translate("syntax"), 2, Hiro_Utils.Get_Translate("file")));
                return;
            }
            string desfolderdir = desdir;
            if (!desfolderdir.EndsWith("\\"))
            {
                desfolderdir += "\\";
            }
            CreateFolder(desfolderdir);
            string[] filenames = Directory.GetFileSystemEntries(srcdir);
            foreach (string file in filenames)
            {
                string newdest = desfolderdir + file.Replace(srcdir, "");
                CreateFolder(newdest);
                if (Directory.Exists(file))
                    CopyDirectory(file, newdest);
                else
                    File.Copy(file, newdest);
            }
        }

        internal static void Zip(string? source, List<string> parameter)
        {
            CreateFolder(parameter[1]);
            System.IO.Compression.ZipFile.CreateFromDirectory(parameter[0], parameter[1]);
            BackgroundWorker bw = new();
            if (parameter.Count > 2)
            {
                var para = parameter[2].ToLower();
                if (para.IndexOf("s") != -1)
                    Hiro_Utils.RunExe(parameter[1]);
                if (para.IndexOf("d") != -1)
                    Hiro_Utils.RunExe($"Delete({parameter[0]})");
            }
            if (parameter.Count > 3)
            {
                string cmd = parameter[3];
                for (var i = 4; i < parameter.Count; i++)
                {
                    cmd += "," + parameter[i];
                }
                Hiro_Utils.RunExe(cmd, source);
            }
        }

        internal static void Unzip(string? source, List<string> parameter)
        {
            CreateFolder(parameter[1]);
            System.IO.Compression.ZipFile.ExtractToDirectory(parameter[0], parameter[1]);
            if (parameter.Count > 2)
            {
                var para = parameter[2].ToLower();
                if (para.IndexOf("s") != -1)
                    Hiro_Utils.RunExe(parameter[1]);
                if (para.IndexOf("d") != -1)
                    Hiro_Utils.RunExe($"Delete({parameter[0]})");
            }
            if (parameter.Count > 3)
            {
                string cmd = parameter[3];
                for (var i = 4; i < parameter.Count; i++)
                {
                    cmd += "," + parameter[i];
                }
                Hiro_Utils.RunExe(cmd, source);
            }
        }

        internal static bool? isVideo(string? path)
        {
            var videoExt = "*.3g2;*.3gp;*.3gp2;*.3gpp;*.amv;*.asf;*.avi;*.bik;*.bin;*.crf;*.dav;*.divx;*.drc;*.dv;*.dvr-ms;*.evo;*.f4v;*.flv;*.gvi;*.gxf;*.m1v;*.m2v;*.m2t;*.m2ts;*.m4v;*.mkv;*.mov;*.mp2;*.mp2v;*.mp4;*.mp4v;*.mpe;*.mpeg;*.mpeg1;*.mpeg2;*.mpeg4;*.mpg;*.mpv2;*.mts;*.mtv;*.mxf;*.mxg;*.nsv;*.nuv;*.ogm;*.ogv;*.ogx;*.ps;*.rec;*.rm;*.rmvb;*.rpl;*.thp;*.tod;*.tp;*.ts;*.tts;*.txd;*.vob;*.vro;*.webm;*.wm;*.wmv;*.wtv;*.xesc,";
            if (path == null || !File.Exists(path))
                return null;
            var pExt = Path.GetExtension(path).ToLower();
            return videoExt.Contains($"*{pExt};");
        }



        #region 新建完全限定路径文件夹
        internal static bool CreateFolder(string path)
        {
            int pos = path.IndexOf("\\") + 1;
            string vpath;
            DirectoryInfo? di;
            try
            {
                while (pos > 0)
                {
                    vpath = path[..pos];
                    pos = path.IndexOf("\\", pos) + 1;
                    di = new DirectoryInfo(vpath);
                    if (!di.Exists)
                        di.Create();
                }
            }
            catch (Exception ex)
            {
                Hiro_Utils.LogError(ex, $"Hiro.Exception.Directory.Create");
                return false;
            }
            return true;

        }
        #endregion
    }
}
