using System.Diagnostics;

namespace PinkWpf
{
    public static class Shell
    {
        public static void Start(string command)
        {
            if (command == null)
                return;

            Process proc = new Process();
            proc.StartInfo.FileName = "cmd";
            proc.StartInfo.Arguments = "/c start " + command;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
        }
    }
}
