using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hiro.Helpers.Database
{
    internal class DB_HIRO_EXTENSIONS
    {
        static string? AnalyzeInput(string input)
        {
            // 正则表达式用于检测 URL 协议
            string urlPattern = @"^(?<scheme>[a-zA-Z][a-zA-Z\d+\-.]*):\/\/";

            var match = Regex.Match(input, urlPattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                // 如果是 URL，返回协议名加冒号
                return $"{match.Groups["scheme"].Value}:";
            }
            else
            {
                // 如果是文件路径，获取文件拓展名
                string extension = Path.GetExtension(input);
                return string.IsNullOrEmpty(extension) ? null : extension;
            }
        }
    }
}
