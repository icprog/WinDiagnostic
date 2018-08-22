using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace camera
{
    enum CameraList : int
    {
        Normal = 0,
        AG3820A11_S1_3ND,
        AG3820A11_S1_3ND_V_1_0_RT,
        AG3820A11_S1_3ND_V_1_0_RT_RIGHT,
        AG3820A11_S1_3ND_V_1_1_RT,
        AG3820A11_S1_3ND_V_1_1_RT_RIGHT,
        AV3850A22_SB_4B0,
        AV5050A22_V_1_0,
        DAP7_5M,
        DAP7_2084_0405
    }

    class FangtecCameraDLL
    {
        private static int mode = (int)CameraList.Normal;

        private static int CameraDeviceIdCompare(int iDeviceNum, string szDevicePatch)
        {
            int iRet = (int)CameraList.Normal;
            int iRetTmp = 0;
            bool bRet = false;
            string szStringTmp = "";
            string sFirstVersion = "";
            string sSecondVersion = "";
            int iDate = 0;


            Point pp = new Point(0, 0);
            pp = HotTabCamera.CheckMaxResolution(iDeviceNum);

            if (szDevicePatch.IndexOf("vid_058f&pid_3821") != -1)// 2M
            {

                iRetTmp = FangtecCamera5MDLL.ALC_Initialization();  // about 1.3sec

                bRet = ALC_GetSensorSettingVersion(out szStringTmp);

                if (szStringTmp != "")
                {
                    sFirstVersion = szStringTmp.Substring(0, 2);
                    sSecondVersion = szStringTmp.Substring(2, 6);

                    if (sFirstVersion == "6L")
                    {
                        iDate = Convert.ToInt32(sSecondVersion);
                        if (iDate >= 121120)
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_1_RT;
                        else
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND;

                    }
                    else if (sFirstVersion == "6R")
                    {
                        iDate = Convert.ToInt32(sSecondVersion);
                        if (iDate >= 121120)
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT;
                        else
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND;
                    }
                }

                // bRet = FangtecCamera5MDLL.ALC_UnInitialization();
            }
            else if ((szDevicePatch.IndexOf("vid_058f&pid_5650") != -1) && (pp.X == 2592) && (pp.Y == 1944))// 5M
            {
                iRetTmp = FangtecCamera5MDLL.ALC_Initialization();  // about 1.3sec

                bRet = ALC_GetSensorSettingVersion(out szStringTmp);

                iRet = (int)CameraList.AV3850A22_SB_4B0;

                // bRet = FangtecCamera5MDLL.ALC_UnInitialization();

            }
            else if (szDevicePatch.IndexOf("vid_058f&pid_5650") != -1)// 2M
            {

                iRetTmp = FangtecCamera5MDLL.ALC_Initialization();  // about 1.3sec

                bRet = ALC_GetSensorSettingVersion(out szStringTmp);

                if (szStringTmp != "")
                {
                    sFirstVersion = szStringTmp.Substring(0, 2);
                    sSecondVersion = szStringTmp.Substring(2, 6);

                    if (sFirstVersion == "6L")
                    {
                        iDate = Convert.ToInt32(sSecondVersion);
                        if (iDate >= 121120)
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_1_RT;
                        else
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_0_RT;

                    }
                    else if (sFirstVersion == "6R")
                    {
                        iDate = Convert.ToInt32(sSecondVersion);
                        if (iDate >= 121120)
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT;
                        else
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_0_RT_RIGHT;
                    }
                }

                // bRet = FangtecCamera5MDLL.ALC_UnInitialization();
            }
            else if (szDevicePatch.IndexOf("vid_058f&pid_3823") != -1)// 5M
            {
                iRet = (int)CameraDeviceIdList.AV5050A22_V_1_0;
            }
            else if (szDevicePatch.IndexOf("vid_1e4e&pid_0109") != -1)// 5M
            {
                iRet = (int)CameraDeviceIdList.DAP7_5M;
            }
            else if (szDevicePatch.IndexOf("vid_2084&pid_0405") != -1)// 5M
            {
                iRet = (int)CameraDeviceIdList.DAP7_2084_0405;
            }

            return iRet;
        }

        public static int ALC_Initialization(int iDeviceNum, string szDevicePatch)
        {
            mode = CameraDeviceIdCompare(iDeviceNum, szDevicePatch);
            switch (mode)
            {
                case (int)CameraList.AG3820A11_S1_3ND:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT_RIGHT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT:
                case (int)CameraList.AV3850A22_SB_4B0:
                case (int)CameraList.AV5050A22_V_1_0:
                    return FangtecCamera5MDLL.ALC_Initialization();
                default:
                    return 0;
            }
        }

        public static bool ALC_UnInitialization()
        {
            switch (mode)
            {
                case (int)CameraList.AG3820A11_S1_3ND:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT_RIGHT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT:
                case (int)CameraList.AV3850A22_SB_4B0:
                case (int)CameraList.AV5050A22_V_1_0:
                    return FangtecCamera5MDLL.ALC_UnInitialization();
                default:
                    return true;
            }
        }

        public static bool ALC_ProcReadFromISP(ushort waddr, ushort length, ref byte pValue)
        {
            switch (mode)
            {
                case (int)CameraList.AG3820A11_S1_3ND:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT_RIGHT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT:
                case (int)CameraList.AV3850A22_SB_4B0:
                case (int)CameraList.AV5050A22_V_1_0:
                    return FangtecCamera5MDLL.ALC_ProcReadFromISP(waddr, length, ref pValue);
                default:
                    return true;
            }
        }

        public static bool ALC_ProcWriteToISP(ushort waddr, ushort length, ref byte pValue)
        {
            switch (mode)
            {
                case (int)CameraList.AG3820A11_S1_3ND:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT_RIGHT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT:
                case (int)CameraList.AV3850A22_SB_4B0:
                case (int)CameraList.AV5050A22_V_1_0:
                    return FangtecCamera5MDLL.ALC_ProcWriteToISP(waddr, length, ref pValue);
                default:
                    return true;
            }
        }

        public static bool ALC_SetDeviceIdx(int index)
        {
            switch (mode)
            {
                case (int)CameraList.AG3820A11_S1_3ND:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT_RIGHT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT:
                case (int)CameraList.AV3850A22_SB_4B0:
                case (int)CameraList.AV5050A22_V_1_0:
                    return FangtecCamera5MDLL.ALC_SetDeviceIdx(index);
                default:
                    return true;
            }
        }

        public static bool ALC_GetFirmwareVersion(out string stpRomVersion)
        {
            bool bRet = false;
            byte[] pRomVersion = new byte[9];

            stpRomVersion = "";

            switch (mode)
            {
                case (int)CameraList.AG3820A11_S1_3ND:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT_RIGHT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT:
                case (int)CameraList.AV3850A22_SB_4B0:
                case (int)CameraList.AV5050A22_V_1_0:
                    bRet = FangtecCamera5MDLL.ALC_GetFirmwareVersion(pRomVersion);
                    stpRomVersion = System.Text.ASCIIEncoding.ASCII.GetString(pRomVersion, 0, GetByteLen(pRomVersion));
                    return bRet;
                default:
                    return bRet;
            }
        }

        public static bool ALC_GetSensorSettingVersion(out string stSettingVer)
        {
            bool bRet = false;
            byte[] SettingVer = new byte[11];

            stSettingVer = "";

            switch (mode)
            {
                case (int)CameraList.AG3820A11_S1_3ND:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT_RIGHT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT:
                case (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT:
                case (int)CameraList.AV3850A22_SB_4B0:
                case (int)CameraList.AV5050A22_V_1_0:
                default:
                    bRet = FangtecCamera5MDLL.ALC_GetSensorSettingVersion(SettingVer);
                    stSettingVer = System.Text.ASCIIEncoding.ASCII.GetString(SettingVer, 0, GetByteLen(SettingVer));
                    return bRet;
            }
        }

        private static int GetByteLen(byte[] array)
        {
            int iCount = 0;
            int iLen = 0;

            for (iCount = 0; iCount < array.Length; iCount++)
            {
                if (array[iCount] == 0)
                    return iLen;
                else
                    iLen++;

            }

            return iLen;
        }


        public static void Flash(byte ucOnOff)// 1:on, 0:off
        {
            byte ucData = 0;
            byte ucLedOn = 0x02;// 0x02:gpio 1, 0x40:gpio 6
            byte ucLedDirc = 0x22;
            byte ucLedLoc = 0x20;
            byte ucLen = 1;


            if ((mode == (int)CameraList.AG3820A11_S1_3ND) || (mode == (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT) || (mode == (int)CameraList.AG3820A11_S1_3ND_V_1_0_RT_RIGHT))
            {
                ucLedOn = 0x02;// gpio 1
            }
            else if ((mode == (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT) || (mode == (int)CameraList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT) || (mode == (int)CameraList.AV3850A22_SB_4B0))
            {
                ucLedOn = 0x40;// gpio 6
            }
            else
            {
                return;
            }

            ALC_ProcReadFromISP(ucLedDirc, ucLen, ref ucData);

            if (ucOnOff == 1)
            {
                ucData |= ucLedOn;
            }
            else
            {
                ucData &= (byte)(~ucLedOn);
            }

            ALC_ProcWriteToISP(ucLedDirc, ucLen, ref ucData);
            // Led on
            ALC_ProcReadFromISP(ucLedLoc, ucLen, ref ucData);

            if (ucOnOff == 1)
            {
                ucData |= ucLedOn;
            }
            else
            {
                ucData &= (byte)(~ucLedOn);
            }

            ALC_ProcWriteToISP(ucLedLoc, ucLen, ref ucData);

        }

    }

    class FangtecCamera5MDLL
    {
        #region ACamDll DLL Declare For 5M camera
        [DllImport("ACam5MDll.dll")]
        public static extern int ALC_Initialization();

        [DllImport("ACam5MDll.dll")]
        public static extern bool ALC_UnInitialization();

        [DllImport("ACam5MDll.dll")]
        public static extern bool ALC_ProcReadFromISP(ushort waddr, ushort length, ref byte pValue);

        [DllImport("ACam5MDll.dll")]
        public static extern bool ALC_ProcWriteToISP(ushort waddr, ushort length, ref byte pValue);

        [DllImport("ACam5MDll.dll")]
        public static extern bool ALC_GetFirmwareVersion(byte[] pRomVersion);

        [DllImport("ACam5MDll.dll")]
        public static extern bool ALC_GetSensorSettingVersion(byte[] SettingVer);

        [DllImport("ACam5MDll.dll")]
        public static extern bool ALC_SetDeviceIdx(int nSetIdx);
        #endregion

    }
}
