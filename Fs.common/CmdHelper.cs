using System.Diagnostics;

namespace Fs.common
{
    public class CmdHelper
    {
        public static string RunCmd(string cmd)
        {

            string str = "";
            cmd = cmd.Trim().TrimEnd('&') + "&exit";
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.StandardInput.WriteLine(cmd);
                process.StandardInput.AutoFlush = true;
                str = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
            }
            return str;

        }
    }
}