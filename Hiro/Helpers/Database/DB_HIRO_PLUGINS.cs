using Hiro.ModelViews;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hiro.Helpers.Database
{
    internal class DB_HIRO_PLUGINS
    {
        internal static List<(string, string)>? InitializePlugins()
        {
            using (var connection = HDataBase.GetConnection())
            {
                connection.Open();
                bool tableExists = CheckIfTableExists(connection);
                List<(string FilePath, string Md5Value)> filesToUpdate = new List<(string FilePath, string Md5Value)>();

                if (!tableExists)
                {
                    CreateTable(connection);
                    string pluginsPath = HText.Path_Prepare(@"<current>\system\plugins\");
                    HFile.CreateFolder(pluginsPath);
                    InsertPlugins(connection, pluginsPath);
                    return null;
                }
                else
                {
                    string pluginsPath = HText.Path_Prepare(@"<current>\system\plugins\");
                    HFile.CreateFolder(pluginsPath);
                    CheckAndUpdatePlugins(connection, pluginsPath, filesToUpdate);
                    return filesToUpdate;
                }
            }
        }

        static bool CheckIfTableExists(SqliteConnection connection)
        {
            string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='HIRO_PLUGINS';";
            using (var command = new SqliteCommand(checkTableQuery, connection))
            {
                object? result = command.ExecuteScalar();
                return result != null;
            }
        }

        static void CreateTable(SqliteConnection connection)
        {
            string createTableQuery = @"
    CREATE TABLE IF NOT EXISTS HIRO_PLUGINS (
        ID TEXT PRIMARY KEY,
        Path TEXT UNIQUE,
        MD5 TEXT,
        Icon TEXT,
        Name TEXT,
        Author TEXT,
        Version TEXT,
        PackageName TEXT
    );";

            using (var command = new SqliteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        static void InsertPlugins(SqliteConnection connection, string pluginsPath)
        {
            foreach (var directory in Directory.GetDirectories(pluginsPath))
            {
                foreach (var filePath in Directory.GetFiles(directory, "*.appli"))
                {
                    string md5Value = HFile.GetMD5(filePath).Replace("-", string.Empty);
                    string relativePath = Path.GetRelativePath(HText.Path_Prepare(@"<current>\system\plugins\"), filePath);

                    // 生成不含 - 的 UUID
                    string id = Guid.NewGuid().ToString("N");

                    // 获取插件信息
                    var _appli = new HiroPlugin(filePath);
                    var _icon = _appli.Icon;
                    var _name = _appli.Name;
                    var _author = _appli.Author;
                    var _version = _appli.Version;
                    var _packageName = _appli.PackageName;

                    string insertQuery = @"
            INSERT OR REPLACE INTO HIRO_PLUGINS (ID, Path, MD5, Icon, Name, Author, Version, PackageName) 
            VALUES (@ID, @Path, @MD5, @Icon, @Name, @Author, @Version, @PackageName);";

                    using (var command = new SqliteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        command.Parameters.AddWithValue("@Path", relativePath);
                        command.Parameters.AddWithValue("@MD5", md5Value);
                        command.Parameters.AddWithValue("@Icon", _icon);
                        command.Parameters.AddWithValue("@Name", _name);
                        command.Parameters.AddWithValue("@Author", _author);
                        command.Parameters.AddWithValue("@Version", _version);
                        command.Parameters.AddWithValue("@PackageName", _packageName);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        static void CheckAndUpdatePlugins(SqliteConnection connection, string pluginsPath, List<(string FilePath, string Md5Value)> filesToUpdate)
        {
            string selectQuery = "SELECT Path, MD5 FROM HIRO_PLUGINS;";
            var existingFiles = new Dictionary<string, string>();

            using (var command = new SqliteCommand(selectQuery, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string pluginPath = reader.GetString(0);
                        string md5Value = reader.GetString(1);
                        existingFiles[pluginPath] = md5Value;
                    }
                }
            }

            foreach (var directory in Directory.GetDirectories(pluginsPath))
            {
                foreach (var filePath in Directory.GetFiles(directory, "*.appli"))
                {
                    string md5Value = HFile.GetMD5(filePath).Replace("-", string.Empty);
                    string relativePath = Path.GetRelativePath(HText.Path_Prepare(@"<current>\system\plugins\"), filePath);
                    if (existingFiles.TryGetValue(relativePath, out string? existingMd5Value))
                    {
                        if (!md5Value.Equals(existingMd5Value))
                        {
                            filesToUpdate.Add((relativePath, md5Value));
                        }
                    }
                    else
                    {
                        // 插入新记录时生成不含 - 的 UUID
                        string id = Guid.NewGuid().ToString("N");

                        // 获取插件信息
                        var _appli = new HiroPlugin(filePath);
                        var _icon = _appli.Icon;
                        var _name = _appli.Name;
                        var _author = _appli.Author;
                        var _version = _appli.Version;
                        var _packageName = _appli.PackageName;

                        string insertQuery = @"
                INSERT INTO HIRO_PLUGINS (ID, Path, MD5, Icon, Name, Author, Version, PackageName) 
                VALUES (@ID, @Path, @MD5, @Icon, @Name, @Author, @Version, @PackageName);";

                        using (var command = new SqliteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@ID", id);
                            command.Parameters.AddWithValue("@Path", relativePath);
                            command.Parameters.AddWithValue("@MD5", md5Value);
                            command.Parameters.AddWithValue("@Icon", _icon);
                            command.Parameters.AddWithValue("@Name", _name);
                            command.Parameters.AddWithValue("@Author", _author);
                            command.Parameters.AddWithValue("@Version", _version);
                            command.Parameters.AddWithValue("@PackageName", _packageName);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }


    }
}
