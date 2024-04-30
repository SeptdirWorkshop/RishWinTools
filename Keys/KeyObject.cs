using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RishWinTools.Keys
{
    public class KeyObject
    {
        public string? Filename;
        public string? Path;
        public string? PublicKey;
        public string? PublicKeyPath;
        public string? ServerCommand;
        public bool KeyExist = false;

        public KeyObject(string? fileName = null)
        {
            Path = "";
            if (fileName == null)
            {
                return;
            }

            Filename = System.IO.Path.GetFileNameWithoutExtension(fileName);
            Path = System.IO.Path.Combine(Program.SSHFolderPath, Filename);
            if (!File.Exists(Path))
            {
                return;
            }
            KeyExist = true;

            PublicKeyPath = System.IO.Path.Combine(Program.SSHFolderPath, Filename + ".pub");
            if (File.Exists(PublicKeyPath))
            {
                PublicKey = File.ReadAllText(PublicKeyPath).Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
            }
            else
            {
                PublicKey = "";
                PublicKeyPath = "";
            }

            if (PublicKey.Length > 0)
            {
                ServerCommand = $"echo \"{PublicKey}\" >> /root/.ssh/authorized_keys";
            }
            else
            {
                ServerCommand = "";
            }
        }
    }
}
