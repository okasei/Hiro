using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiro.Helpers
{
    internal class HDataBase
    {
        internal static SqliteConnection GetConnection()
        {
            var _cs = HText.Path_Prepare(@"<current>\users\<hiuser>\config\<hiuser>.hdb");
            HFile.CreateFolder(_cs);
            return new SqliteConnection(@$"Data Source={_cs};");
        }
    }
}
