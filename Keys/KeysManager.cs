using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RishWinTools.Keys
{
    public static class KeysManager
    {
        private static Dictionary<string, KeyObject>? Keys;

        public static Dictionary<string, KeyObject> GetKeys(bool forse = false)
        {
            if (Keys == null || forse)
            {
                Keys = new Dictionary<string, KeyObject>();

                if (Directory.Exists(Program.SSHFolderPath))
                {
                    string[] files = Directory.GetFiles(Program.SSHFolderPath);
                    foreach (string file in files)
                    {
                        string content = File.ReadAllText(file);
                        if (content.Contains("-----BEGIN "))
                        {
                            KeyObject key = new KeyObject(file);
                            if (key != null && key.Filename != null && key.KeyExist)
                            {
                                Keys[key.Filename] = key;
                            }
                        }
                    }
                }
            }

            return Keys;
        }
        public static bool GenerateKey(string name, string comment = "", bool message = true)
        {
            string keyPath = Path.Combine(Program.SSHFolderPath, name);
            string pubPath = keyPath + ".pub";

            if (File.Exists(keyPath) || File.Exists(pubPath))
            {
                MessageBox.Show("Ключ уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Process process = new Process();
            process.StartInfo.FileName = "ssh-keygen";
            process.StartInfo.Arguments = $"-t ed25519 -C \"{comment}\" -f \"{keyPath}\" -N \"\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.Start();
            process.WaitForExit();
            string error = process.StandardError.ReadToEnd();

            if (process.ExitCode == 0)
            {
                if (message)
                {
                    MessageBox.Show("Ключ сгенирирован", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            }
            else
            {
                MessageBox.Show($"Ошибка генерерации ключа: {error}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool RemoveKeyFromAgent(string? name = null, bool message = true, bool quiet = false)
        {
            Process process = new Process();
            process.StartInfo.FileName = "ssh-add";
            if (name != null)
            {
                string keyPath = Path.Combine(Program.SSHFolderPath, name);
                process.StartInfo.Arguments = $"-d \"{keyPath}\"";
            }
            else
            {
                process.StartInfo.Arguments = "-D";
            }

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();
            string error = process.StandardError.ReadToEnd();

            if (process.ExitCode == 0)
            {
                if (message)
                {
                    if (name != null)
                    {
                        MessageBox.Show("Ключ удален из агента", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ключи удалены из агента", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                return true;
            }
            else
            {
                if (!quiet)
                {
                    MessageBox.Show($"Ошибка удаления ключа: {error}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }

        public static bool InsertKey(string name, string secret)
        {
            string keyPath = Path.Combine(Program.SSHFolderPath, name);
            string pubPath = keyPath + ".pub";

            if (File.Exists(keyPath) || File.Exists(pubPath))
            {
                MessageBox.Show("Ключ уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(secret))
            {
                MessageBox.Show("Пустой ключ не допускается", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!secret.EndsWith("\n"))
            {
                secret += "\n";
            }

            try
            {
                File.WriteAllText(keyPath, secret);
                MessageBox.Show("Ключ успешно сохранен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении ключа: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool CreatePubKey(string name, bool message = true, bool quiet = false)
        {
            string keyPath = Path.Combine(Program.SSHFolderPath, name);
            string pubPath = keyPath + ".pub";
            if (!File.Exists(keyPath))
            {
                MessageBox.Show("Ключ не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (File.Exists(pubPath))
            {
                MessageBox.Show("Публичный Ключ уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Process process = new Process();
            process.StartInfo.FileName = "ssh-keygen";
            process.StartInfo.Arguments = $"-y -f \"{keyPath}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            process.Start();
            string publicKey = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                try
                {
                    File.WriteAllText(pubPath, publicKey);
                    if (message)
                    {
                        MessageBox.Show("Публичный ключ успешно сохранен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении публичного ключа: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                if (!quiet)
                {
                    MessageBox.Show($"Ошибка генерации ключа: {error}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }
        }

        public static bool RemoveKey(string name)
        {
            string keyPath = Path.Combine(Program.SSHFolderPath, name);
            string pubPath = keyPath + ".pub";

            if (!File.Exists(keyPath) && File.Exists(pubPath))
            {
                MessageBox.Show("Ключ не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                if (File.Exists(keyPath))
                {
                    RemoveKeyFromAgent(name, false, true);
                    File.Delete(keyPath);
                }
                if (File.Exists(pubPath))
                {
                    File.Delete(pubPath);
                }

                MessageBox.Show("Ключ успешно удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления ключа: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
