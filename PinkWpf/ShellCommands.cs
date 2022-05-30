using PinkWpf.Input;
using System;
using System.Diagnostics;

namespace PinkWpf
{
    public static class ShellCommands
    {
        public static Command StartCommand { get; } = new Command((obj) => Start(obj?.ToString()));

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
