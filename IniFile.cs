using System.Runtime.InteropServices;
using System.Text;

namespace RHServerManager
{
    public class IniFile
    {
        private readonly string _iniFilePath;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public IniFile(string iniFileName)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _iniFilePath = Path.Combine(appDirectory, iniFileName);

            if (!File.Exists(_iniFilePath))
            {
                File.Create(_iniFilePath).Close();
            }
        }

        public string ReadValue(string key, string defaultValue = "")
        {
            StringBuilder sb = new(255);
            _ = GetPrivateProfileString("Option", key, defaultValue, sb, 255, _iniFilePath);
            return sb.ToString();
        }

        public void WriteValue(string key, string value)
        {
            WritePrivateProfileString("Option", key, value, _iniFilePath);
        }
    }

}