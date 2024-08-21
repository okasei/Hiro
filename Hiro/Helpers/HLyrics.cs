using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Hiro.Helpers
{
    internal class HLrycis
    {
        /// <summary>
        /// 歌曲
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 艺术家
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// 专辑
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// 歌词作者
        /// </summary>
        public string LrcBy { get; set; }
        /// <summary>
        /// 偏移量
        /// </summary>
        public string Offset { get; set; }

        /// <summary>
        /// 歌词
        /// </summary>
        public Dictionary<double, string> LrcWord = new Dictionary<double, string>();

        /// <summary>
        /// 获得歌词信息
        /// </summary>
        /// <param name="LrcPath">歌词路径</param>
        /// <returns>返回歌词信息(Lrc实例)</returns>
        public static HLrycis InitLrcFromFile(string LrcPath)
        {
            HLrycis lrc = new HLrycis();
            Dictionary<double, string> dicword = new Dictionary<double, string>();
            using (FileStream fs = new FileStream(LrcPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                string line;
                using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("[ti:"))
                        {
                            lrc.Title = SplitInfo(line);
                        }
                        else if (line.StartsWith("[ar:"))
                        {
                            lrc.Artist = SplitInfo(line);
                        }
                        else if (line.StartsWith("[al:"))
                        {
                            lrc.Album = SplitInfo(line);
                        }
                        else if (line.StartsWith("[by:"))
                        {
                            lrc.LrcBy = SplitInfo(line);
                        }
                        else if (line.StartsWith("[offset:"))
                        {
                            lrc.Offset = SplitInfo(line);
                        }
                        else
                        {
                            try
                            {
                                Regex regexword = new Regex(@".*\](.*)");
                                Match mcw = regexword.Match(line);
                                string word = mcw.Groups[1].Value;
                                if (HSet.Read_DCIni("LyricsBlank", "false").Equals("false") && word.Trim().Equals(string.Empty))
                                    continue;
                                Regex regextime = new Regex(@"\[([0-9.:]*)\]", RegexOptions.Compiled);
                                MatchCollection mct = regextime.Matches(line);
                                foreach (Match item in mct)
                                {
                                    double time = TimeSpan.Parse("00:" + item.Groups[1].Value).TotalSeconds;
                                    dicword.Add(time, word);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            lrc.LrcWord = dicword.OrderBy(t => t.Key).ToDictionary(t => t.Key, p => p.Value);
            return lrc;
        }
        /// <summary>
        /// 获得歌词信息
        /// </summary>
        /// <param name="LrcPath">歌词路径</param>
        /// <returns>返回歌词信息(Lrc实例)</returns>
        public static HLrycis InitLrc(string Lryics)
        {
            HLrycis lrc = new HLrycis();
            Dictionary<double, string> dicword = new Dictionary<double, string>();
            var _lrcs = Lryics.Split(new string[] { Environment.NewLine, "\r\n", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < _lrcs.Length; i++)
            {
                var line = _lrcs[i];
                if (line.Trim().Length == 0)
                {
                    continue;
                }
                else if (line.StartsWith("[ti:"))
                {
                    lrc.Title = SplitInfo(line);
                }
                else if (line.StartsWith("[ar:"))
                {
                    lrc.Artist = SplitInfo(line);
                }
                else if (line.StartsWith("[al:"))
                {
                    lrc.Album = SplitInfo(line);
                }
                else if (line.StartsWith("[by:"))
                {
                    lrc.LrcBy = SplitInfo(line);
                }
                else if (line.StartsWith("[offset:"))
                {
                    lrc.Offset = SplitInfo(line);
                }
                else
                {
                    try
                    {
                        Regex regexword = new Regex(@".*\](.*)");
                        Match mcw = regexword.Match(line);
                        string word = mcw.Groups[1].Value;
                        if (HSet.Read_DCIni("LyricsBlank", "false").Equals("false") && word.Trim().Equals(string.Empty))
                            continue;
                        Regex regextime = new Regex(@"\[([0-9.:]*)\]", RegexOptions.Compiled);
                        MatchCollection mct = regextime.Matches(line);
                        foreach (Match item in mct)
                        {
                            double time = TimeSpan.Parse("00:" + item.Groups[1].Value).TotalSeconds;
                            dicword.Add(time, word);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            lrc.LrcWord = dicword.OrderBy(t => t.Key).ToDictionary(t => t.Key, p => p.Value);
            return lrc;
        }
        /// <summary>
        /// 获取三项Lyrics, 其中编号为 0 的是已经经过的歌词的最近一个, 编号为 1, 2 的是即将经过的最近的歌词
        /// </summary>
        /// <param name="baseTime">基准时间</param>
        /// <returns></returns>
        public string[] GetLyrics(double baseTime, out double nextTime)
        {
            var ret = new string[5] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
            nextTime = -1;
            bool _inted = false;
            for (int i = 0; i < LrcWord.Keys.Count; i++)
            {
                if (App.dflag)
                    HLogger.LogtoFile($"LrcWord.Key.Count = {LrcWord.Keys.Count}, i = {i}, Key = {LrcWord.Keys.ElementAt(i)}");
                var _key = LrcWord.Keys.ElementAt(i);
                if (_key > baseTime && !_inted)
                {
                    ret[0] = i > 0 ? LrcWord[LrcWord.Keys.ElementAt(i - 1)] : string.Empty;
                    ret[1] = LrcWord[LrcWord.Keys.ElementAt(i)];
                    ret[2] = i < LrcWord.Keys.Count - 1 ? LrcWord[LrcWord.Keys.ElementAt(i + 1)] : string.Empty;
                    ret[3] = i < LrcWord.Keys.Count - 2 ? LrcWord[LrcWord.Keys.ElementAt(i + 2)] : string.Empty;
                    ret[4] = i < LrcWord.Keys.Count - 3 ? LrcWord[LrcWord.Keys.ElementAt(i + 3)] : string.Empty;
                    nextTime = LrcWord.Keys.ElementAt(i);
                    return ret;
                }
            }
            return ret;
        }

        /// <summary>
        /// 处理信息(私有方法)
        /// </summary>
        /// <param name="line"></param>
        /// <returns>返回基础信息</returns>
        static string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
        }
    }
}
