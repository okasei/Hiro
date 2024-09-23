using Hiro.Helpers.Plugin;
using Hiro.ModelViews;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hiro.Helpers.Database
{
    internal class DB_HIRO_PLUGINS
    {

        internal static void InitializeTable()
        {
            using (var connection = HDataBase.GetConnection())
            {
                connection.Open();
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS HIRO_PLUGINS (
                    ID TEXT PRIMARY KEY,
                    PackageName TEXT UNIQUE,
                    MD5 TEXT,
                    Icon TEXT,
                    Name TEXT,
                    Author TEXT,
                    Version INTEGER,
                    Description TEXT,
                    AutoLoad INTEGER,
                    IsolatedLoad INTEGER,
                    Path TEXT
                )";
                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        internal static void InitializePlugins()
        {
            using (var connection = HDataBase.GetConnection())
            {
                connection.Open();
                InitializeTable();
                string pluginsPath = HText.Path_Prepare(@"<current>\system\plugins\");
                HFile.CreateFolder(pluginsPath);
                ScanAndLoadPlugins(pluginsPath);
            }
        }
        // 扫描指定文件夹中的 .appli 文件，仅扫描第一层子目录
        internal static void ScanAndLoadPlugins(string folderPath)
        {
            // 获取第一层子目录
            var directories = Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly);

            foreach (var dir in directories)
            {
                // 仅扫描第一层子目录中的 .appli 文件
                var appliFiles = Directory.GetFiles(dir, "*.appli", SearchOption.TopDirectoryOnly);

                foreach (var filePath in appliFiles)
                {

                    // 检查是否存在相同的 PackageName
                    if (IsPluginExists(filePath))
                    {
                        HLogger.LogtoFile($"{filePath} already loaded, cannot update now.");
                        // 这里可以添加自定义处理逻辑
                        continue;
                    }

                    var plugin = HPluginManager.CreateOrLoad(filePath);
                    // 插入插件信息到数据库
                    InsertPluginToDatabase(plugin, filePath);
                    plugin.FirstRun();
                }
            }
        }

        // 判断 PackageName 是否已经存在于数据库中
        private static bool IsPluginExists(string filePath)
        {
            using (var connection = HDataBase.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM HIRO_PLUGINS WHERE Path = @Path";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Path", HText.Anti_Path_Prepare(filePath));
                    long count = (long)(command.ExecuteScalar() ?? 0);
                    return count > 0;
                }
            }
        }

        // 插入插件信息到数据库
        private static void InsertPluginToDatabase(HiroPlugin plugin, string filePath)
        {
            using (var connection = HDataBase.GetConnection())
            {
                connection.Open();
                string insertQuery = @"
                INSERT INTO HIRO_PLUGINS (ID, PackageName, MD5, Icon, Name, Author, Version, Description, AutoLoad, IsolatedLoad, Path)
                VALUES (@ID, @PackageName, @MD5, @Icon, @Name, @Author, @Version, @Description, @AutoLoad, @IsolatedLoad, @Path)";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", Guid.NewGuid().ToString("N")); // 生成不带 '-' 的 UUID
                    command.Parameters.AddWithValue("@PackageName", plugin.PackageName);
                    command.Parameters.AddWithValue("@MD5", HFile.GetMD5(filePath).ToUpper().Replace("-",string.Empty));
                    command.Parameters.AddWithValue("@Icon", plugin.Icon);
                    command.Parameters.AddWithValue("@Name", plugin.Name);
                    command.Parameters.AddWithValue("@Author", plugin.Author);
                    command.Parameters.AddWithValue("@Version", plugin.Version);
                    command.Parameters.AddWithValue("@Description", plugin.Description);
                    command.Parameters.AddWithValue("@AutoLoad", plugin.AutoLoad ? 1 : 0);
                    command.Parameters.AddWithValue("@IsolatedLoad", plugin.IsolateRun ? 1 : 0);
                    command.Parameters.AddWithValue("@Path", HText.Anti_Path_Prepare(filePath)); // 插件文件的路径

                    command.ExecuteNonQuery();
                }
            }
        }


        internal static string? GetPluginInfo(string key, string value, string target)
        {
            using (var connection = HDataBase.GetConnection())
            {
                connection.Open();

                // 构建 SQL 查询，根据指定的列名和值查找目标列
                string query = $@"
                SELECT {target} 
                FROM HIRO_PLUGINS 
                WHERE {key} = @Value 
                LIMIT 1;
            ";

                using (var command = new SqliteCommand(query, connection))
                {
                    // 添加参数以防止 SQL 注入
                    command.Parameters.AddWithValue("@Value", value);
                    // 执行查询并获取结果
                    var result = command.ExecuteScalar();

                    // 如果查询有结果，返回第一条记录，否则返回 null
                    return result?.ToString();
                }
            }
        }

    }
}
