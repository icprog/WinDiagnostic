using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.IO;

namespace rotation
{
    public class HotTabRegistry
    {
        #region DllImport
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        #endregion

        #region Definition
        private const string HotTabRegisterPath = @"SOFTWARE\HotTab";
        private string HotTabRegisterIniPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\hottab\\HottabRegIni.ini";
        private RegistryKey registryKey;
        private int mode = 0; // 0:register, 1:ini
        #endregion

        public HotTabRegistry()
        {
            string programSrcPatch = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\hottab\\";
            System.IO.Directory.CreateDirectory(programSrcPatch);

            try
            {
                registryKey = Registry.CurrentUser.CreateSubKey(HotTabRegisterPath); // for Win7
            }
            catch 
            {
                mode = 1;
            }
        }

        public bool RegistryRead(String name, ref String value)
        {

            if (mode == 1)
            {
                value = IniReadValue("SETTING", name);

                if (value == "")
                    return false;
                else
                    return true;
            }
            else
            {
                try
                {
                    value = registryKey.GetValue(name).ToString();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool RegistryWrite(String name, String value)
        {
            if (mode == 1)
            {
                IniWriteValue("SETTING", name, value);
                return true;
            }
            else
            {
                try
                {
                    registryKey.SetValue(name, value);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.HotTabRegisterIniPath);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.HotTabRegisterIniPath);
            return temp.ToString();
        }

        // Static Method Declare
        public static bool RegistryRead(String registerPath, String name, ref String value)
        {
            try
            {
                RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                RegistryKey registryKey = localMachineX64View.OpenSubKey(registerPath);

                value = registryKey.GetValue(name).ToString();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
