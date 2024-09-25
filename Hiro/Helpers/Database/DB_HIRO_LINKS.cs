using Hiro.ModelViews;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Hiro.Helpers.Database
{
    internal class DB_HIRO_LINKS
    {
        private static void InitializeTable()
        {
            using (var connection = HDataBase.GetConnection())
            {
                connection.Open();

                // SQL 查询检查表是否存在
                string checkTableExists = @"
                SELECT name 
                FROM sqlite_master 
                WHERE type='table' 
                AND name='HIRO_LINKS';
            ";

                using (var command = new SqliteCommand(checkTableExists, connection))
                {
                    var result = command.ExecuteScalar();

                    // 如果表不存在，则创建表
                    if (result == null)
                    {
                        string createTableQuery = @"
                        CREATE TABLE HIRO_LINKS (
                            ID TEXT PRIMARY KEY,
                            Link TEXT,
                            App TEXT,
                            Params TEXT
                        );
                    ";

                        using (var createTableCommand = new SqliteCommand(createTableQuery, connection))
                        {
                            createTableCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        internal static bool? AddOrUpdateRecord(string id, string link, string app, string parameters)
        {
            try
            {
                InitializeTable();
                using (var connection = HDataBase.GetConnection())
                {
                    connection.Open();

                    // 使用 INSERT OR REPLACE 进行插入或更新
                    string insertOrUpdateQuery = @"
                INSERT OR REPLACE INTO HIRO_LINKS (ID, Link, App, Params)
                VALUES (@ID, @Link, @App, @Params);
            ";

                    using (var command = new SqliteCommand(insertOrUpdateQuery, connection))
                    {
                        // 添加参数
                        command.Parameters.AddWithValue("@ID", id);
                        command.Parameters.AddWithValue("@Link", link);
                        command.Parameters.AddWithValue("@App", app);
                        command.Parameters.AddWithValue("@Params", parameters);

                        // 执行命令
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                HLogger.LogError(ex, "Hiro.Exception.Database.Link.Update");
            }
            return false;
        }
        

        internal static List<HiroPluginC> GetPluginsByLink(string linkExt)
        {
            DB_HIRO_PLUGINS.InitializeTable();
            InitializeTable();
            var pluginInfos = new List<HiroPluginC>();

            using (var connection = HDataBase.GetConnection())
            {
                connection.Open();
                string query = @"
        SELECT p.Icon, p.Name, p.Author, p.Version, p.Description, p.Path
        FROM HIRO_LINKS l
        JOIN HIRO_PLUGINS p ON l.App = p.ID
        WHERE l.Link = @link;";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@link", linkExt); // 替换为你的链接值

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pluginInfo = new HiroPluginC
                            {
                                Icon = reader["Icon"].ToString(),
                                Name = reader["Name"].ToString(),
                                Author = reader["Author"].ToString(),
                                Version = reader.GetInt32(reader.GetOrdinal("Version")),
                                Description = reader["Description"].ToString(),
                                Path = reader["Path"].ToString()
                            };

                            pluginInfos.Add(pluginInfo);
                        }
                    }
                }
            }

            return pluginInfos;
        }
    }
}
