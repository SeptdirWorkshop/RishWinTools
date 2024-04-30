namespace RishWinTools
{
    internal static class Program
    {
        public static string SSHFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");

        [STAThread]
        static void Main()
        {          
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());          
        }
    }
}