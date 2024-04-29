namespace RishWinTools
{
    internal static class Program
    {
        public static string SSHFolderPath { get; private set; }

        [STAThread]
        static void Main()
        {
            SSHFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());          
        }
    }
}