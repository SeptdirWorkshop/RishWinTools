using RishWinTools.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RishWinTools.Servers
{
    public class ServerObject
    {
        public string Host;
        public string Hostname;
        public string User;
        public string KeyName;
        public string PublicKey;
        public Dictionary<string, string> Additions = new Dictionary<string, string>();
        public KeyObject Key = new KeyObject();

        public ServerObject(string? host = null)
        {           
            if (host == null)
            {
                return;
            }
            host = host.Replace("Host", "").Trim();

            if (!Directory.Exists(Program.SSHFolderPath) || !File.Exists(ServersManager.ConfigPath))
            {
                return;
            }

            bool find = false;
            string[] lines = File.ReadAllLines(ServersManager.ConfigPath);

            Additions = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                if (find && line.StartsWith("Host ", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                if (!find && line.StartsWith("Host " + host, StringComparison.OrdinalIgnoreCase))
                {
                    Host = line.Replace("Host", "").Trim();
                    find = true;
                    continue;
                }

                if (!find)
                {
                    continue;
                }

                string clean = line.Trim();
                if (String.IsNullOrWhiteSpace(clean))
                {
                    continue;
                }

                if (clean.StartsWith("Hostname", StringComparison.OrdinalIgnoreCase))
                {
                    Hostname = clean.Replace("Hostname", "").Trim();
                    continue;
                }

                if (clean.StartsWith("User", StringComparison.OrdinalIgnoreCase))
                {
                    User = clean.Replace("User", "").Trim();
                    continue;
                }

                if (clean.StartsWith("IdentityFile", StringComparison.OrdinalIgnoreCase))
                {
                    KeyName = clean.Replace("IdentityFile", "").Trim();
                }

                int index = clean.IndexOf(' ');
                string key = "";
                string value = "";
                if (index > -1)
                {

                    key = clean.Substring(0, index);
                    value = clean.Substring(index + 1);
                }
                else
                {
                    key = clean;
                }
                Additions[key] = value;
            }

            if (KeyName == null)
            {
                return;
            }

            Key = new KeyObject(KeyName);
            if (Key.KeyExist)
            {
                KeyName = Key.Filename;
                PublicKey = Key.PublicKey;
            }
        }

        public string ToConfigFileSting()
        {
            List<string> result = new List<string>();

            result.Add("Host " + Host);
            result.Add("    Hostname " + Hostname);
            result.Add("    User " + User);

            foreach (var item in Additions)
            {
                string key = item.Key;
                string value = item.Value;
                if (key != "IdentityFile")
                {
                    result.Add("    " + key + " " + value);
                }
            }

            string identityFile = (Additions.ContainsKey("IdentityFile")) ? Additions["IdentityFile"] : "";
            if (Key.KeyExist)
            {
                identityFile = "~/.ssh/" + Key.Filename;
            }
            if (!string.IsNullOrEmpty(identityFile))
            {
                result.Add("    IdentityFile " + identityFile);
            }

            return string.Join("\n", result);
        }
    }
}
