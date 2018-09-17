﻿using Newtonsoft.Json.Linq;
using SpeakerMicAutoTestApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static SpeakerMicAutoTestApi.Platform;

namespace audio
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern void AllocConsole();

        static string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(exepath, path);
        }

        static void Main(string[] args)
        {
            JObject result = new JObject();

            try
            {
                var jsonconfig = GetFullPath("config.json");
                var wavpath = GetFullPath("WinmateAudioTest.wav");

                if (!File.Exists(jsonconfig))
                {
                    MessageBox.Show("config.json not founded");
                    return;
                }
                    
                result["result"] = "FAIL";
                dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));

                if ((bool)jobject.ShowWindow)
                    AllocConsole();

                var name = jobject.name.ToString();
                var Model = name.Substring(name.IndexOf("_") + 1);
                AudioTest api = new AudioTest(Model, true);
                api.WavFileName = wavpath;
                api.ExternalRecordThreshold = (double)jobject.ExternalRecordThreshold;
                api.InternalRecordThreshold = (double)jobject.InternalRecordThreshold;
                api.AudioJackRecordThreshold = (double)jobject.AudioJackRecordThreshold;
                var testresult = api.RunTest();

                Console.WriteLine("file name: {0}", api.WavFileName);
                Console.WriteLine("left: {0}", api.LeftIntensity);
                Console.WriteLine("right: {0}", api.RightIntensity);
                Console.WriteLine("internal: {0}", api.InternalIntensity);
                Console.WriteLine("internal left: {0}", api.InternalLeftIntensity);
                Console.WriteLine("internal right: {0}", api.InternalRightIntensity);
                Console.WriteLine("audiojack: {0}", api.AudioJackIntensity);
                Console.WriteLine("fan: {0}", api.FanIntensity);
                Console.WriteLine("left threshold: {0}", api.ExternalRecordThreshold);
                Console.WriteLine("right threshold: {0}", api.ExternalRecordThreshold);
                Console.WriteLine("internal threshold: {0}", api.InternalRecordThreshold);
                Console.WriteLine("audiojack threshold: {0}", api.AudioJackRecordThreshold);
                Console.WriteLine("fan threshold: {0}", api.FanRecordThreshold);
                Console.WriteLine("exception: {0}", api?.Exception);

                switch (testresult)
                {
                    case Result.FanRecordFail:
                        Console.WriteLine("FanRecordFail");
                        break;
                    case Result.InternalLeftMicFail:
                        Console.WriteLine("InternalLeftMicFail");
                        break;
                    case Result.InternalRightMicFail:
                        Console.WriteLine("InternalRightMicFail");
                        break;
                    case Result.LeftSpeakerFail:
                        Console.WriteLine("LeftSpeakerFail");
                        break;
                    case Result.RightSpeakerFail:
                        Console.WriteLine("RightSpeakerFail");
                        break;
                    case Result.Pass:
                        Console.WriteLine("Pass");
                        result["result"] = "PASS";
                        break;
                    case Result.ExceptionFail:
                        Console.WriteLine("ExceptionFail");
                        break;
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            File.Create(GetFullPath("completed"));
            Console.ReadLine();
        }
    }
}
