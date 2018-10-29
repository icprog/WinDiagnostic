using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace barcode
{
    class SsiDllApi
    {
        #region SSIDLL DLL Declare For SE4500DL Barcode

        [DllImport("SSIdll.dll")]
        public static extern int SSIConnect(IntPtr hwnd, int baud, int port);

        [DllImport("SSIdll.dll")]
        public static extern int SSIDisconnect(IntPtr hwnd, int port);
        
        [DllImport("SSIdll.dll")]
        public static extern int TransmitVersion(int port);

        [DllImport("SSIdll.dll")]
        public static extern int PullTrigger(int port);

        [DllImport("SSIdll.dll")]
        public static extern int ReleaseTrigger(int port);

        [DllImport("SSIdll.dll")]
        public static extern int SetVersionBuffer(int port,byte[]data,long len);

        [DllImport("SSIdll.dll")]
        public static extern int SetDecodeBuffer(int port,byte[]data,long len);

        [DllImport("SSIdll.dll")]
        public static extern int SetParameters(byte[] Params, int ParamBytes, int port);

        #endregion

        public static UInt32 MAX_LEN = 4096;

        public static byte[] DecodeData = new byte[MAX_LEN];
        public static string DecodeBuffer = "";

        public SsiDllApi()
        {
            
        }

    }
}
