using RishWinTools.Keys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RishWinTools.Servers
{
    public static class ServersManager
    {
        public static string ConfigPath = System.IO.Path.Combine(Program.SSHFolderPath, "config");
        public static string ConfigBacPath = System.IO.Path.Combine(Program.SSHFolderPath, "config.rwtbac");
        private static Dictionary<string, ServerObject>? Servers;

        public static Dictionary<string, ServerObject> GetServers(bool forse = false)
        {
            if (Servers == null || forse)
            {
                Servers = new Dictionary<string, ServerObject>();

                if (Directory.Exists(Program.SSHFolderPath) && File.Exists(ConfigPath))
                {

                    string[] lines = File.ReadAllLines(ConfigPath);

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Host ", StringComparison.OrdinalIgnoreCase))
                        {
                            ServerObject server = new ServerObject(line.Trim());
                            if (server.Host != null)
                            {
                                Servers[server.Host] = server;
                            }
                        }
                    }

                    SortedDictionary<string, ServerObject> sortedServers = new SortedDictionary<string, ServerObject>(Servers);
                    Servers = sortedServers.ToDictionary(entry => entry.Key, entry => entry.Value);
                }
            }

            return Servers;
        }

        public static bool CreateServer(string host, string hostname, string user, string key)
        {
            GetServers(true);
            if (Servers == null)
            {
                return false;
            }

            if (Servers.ContainsKey(host))
            {
                MessageBox.Show("Сервер уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            ServerObject server = new ServerObject(host);
            server.Host = host;
            server.Hostname = hostname;
            server.User = user;
            server.Key = new KeyObject(key);
            server.Additions["Compression"] = "yes";
            server.Additions["AddKeysToAgent"] = "yes";
            SetServer(server);

            if (PullServersToFile())
            {
                MessageBox.Show("Сервер создан", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool RemoveServer(string host)
        {
            GetServers(true);
            if (Servers == null)
            {
                MessageBox.Show("Сервер удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            ServerObject server = new ServerObject(host);
            if (server.Host == null)
            {
                MessageBox.Show("Сервер не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (Servers.ContainsKey(server.Host))
            {
                MessageBox.Show("Сервер не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            Servers.Remove(server.Host);
            if (PullServersToFile())
            {
                MessageBox.Show("Сервер удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            else
            {
                RemoveKnownHosts(false, true);

                return false;
            }
        }

        public static void SetServer(ServerObject server)
        {
            if (server.Host == null)
            {
                return;
            }

            if (Servers == null)
            {
                Servers = new Dictionary<string, ServerObject>();
            }

            Servers[server.Host] = server;
        }

        public static bool PullServersToFile()
        {
            bool backup = false;
            try
            {
                if (Servers == null)
                {
                    GetServers();
                }

                List<string> result = new List<string>();
                if (Servers != null)
                {
                    SortedDictionary<string, ServerObject> sortedServers = new SortedDictionary<string, ServerObject>(Servers);
                    foreach (var item in sortedServers)
                    {
                        ServerObject server = item.Value as ServerObject;
                        if (server != null)
                        {
                            result.Add(server.ToConfigFileSting());
                        }
                    }
                }

                string content = string.Join("\n", result);

                if (!File.Exists(ConfigPath))
                {
                    File.Create(ConfigPath).Close();
                }

                if (File.Exists(ConfigBacPath))
                {
                    File.Delete(ConfigBacPath);
                }

                File.Copy(ConfigPath, ConfigBacPath);
                backup = true;

                File.WriteAllText(ConfigPath, content);

                return true;
            }
            catch (Exception ex)
            {
                int backupRestore = 0;
                if (backup)
                {
                    try
                    {
                        File.Copy(ConfigBacPath, ConfigPath, true);
                        backupRestore = 1;
                    }
                    catch
                    {
                        backupRestore = -1;
                    }
                }

                string message = $"Произошла ошибка при обновлении файлов конфигурации: {ex.Message}";
                if (backupRestore != 0)
                {
                    message += "\n";
                    message += (backupRestore == 1) ? "Бэкап востановлен" : "Бэкап не востановлен";
                }
                MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }

        public static void ConnectToServer(string host, string? user = null)
        {
            string sshCommand = $"ssh {host}";
            if (!string.IsNullOrEmpty(user))
            {
                sshCommand = $"ssh {user}@{host}";
            }

            string? workingDirectory = Environment.GetEnvironmentVariable("USERPROFILE");
            string commandToShow = $"echo '{workingDirectory}^>{sshCommand}'";

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/K \"{commandToShow} && {sshCommand}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WorkingDirectory = workingDirectory
                };

                Process process = new Process { StartInfo = psi };
                process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске PowerShell: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static bool RemoveKnownHosts(bool message = true, bool quiet = false)
        {
            string fileName = System.IO.Path.Combine(Program.SSHFolderPath, "known_hosts");
            if (!File.Exists(fileName))
            {
                return true;
            }
            try
            {
                File.Delete(fileName);
                if (message)
                {
                    MessageBox.Show("known_hosts удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (!quiet)
                {
                    MessageBox.Show("Ошибка при удалении known_hosts: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }
    }
}
