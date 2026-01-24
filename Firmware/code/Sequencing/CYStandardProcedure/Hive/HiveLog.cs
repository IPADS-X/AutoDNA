using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{
    internal static class HiveLog
    {

        private static string ErrorLog = @"E:\SWLog\Standard\NormalLog\Hive\HiveLog\MachineError";
        private static string StatusLog = @"E:\SWLog\Standard\NormalLog\Hive\HiveLog\MachineState";
        private static string AdminLog = @"E:\SWLog\Standard\NormalLog\Hive\HiveLog\Modification";

        private static object Code = new object();
        /// <summary>
        /// Hive报警上传记录
        /// </summary>
        /// <param name="strData"></param>
        public static void WriteHiveErrorLog(string strData)
        {
            Task.Factory.StartNew(new Action(() =>
            {
                lock (Code)
                {
                    try
                    {
                        StringBuilder strFile = new StringBuilder();
                        strFile.AppendFormat("{0}\\{1}\\", ErrorLog, DateTime.Now.ToString("yyyyMMdd"));
                        if (!Directory.Exists(strFile.ToString()))
                        {
                            Directory.CreateDirectory(strFile.ToString());
                        }
                        strFile.Append("hour_" + DateTime.Now.Hour.ToString() + ".txt");
                        using (StreamWriter swAppend = File.AppendText(strFile.ToString()))
                        {
                            StringBuilder str = new StringBuilder();
                            str.AppendFormat("[{0}][{1}]    [{2}]", DateTime.Now, DateTime.Now.Millisecond.ToString("d4"), strData);
                            swAppend.WriteLine(str.ToString());
                        }
                    }
                    catch { }
                }
            }));

        }


        private static object Code1 = new object();
        /// <summary>
        /// Hive机台状态上传记录
        /// </summary>
        /// <param name="strData"></param>
        public static void WriteHiveStatusLog(string strData)
        {
            Task.Factory.StartNew(new Action(() =>
            {
                lock (Code1)
                {
                    try
                    {
                        StringBuilder strFile = new StringBuilder();
                        strFile.AppendFormat("{0}\\{1}\\", StatusLog, DateTime.Now.ToString("yyyyMMdd"));
                        if (!Directory.Exists(strFile.ToString()))
                        {
                            Directory.CreateDirectory(strFile.ToString());
                        }
                        strFile.Append("hour_" + DateTime.Now.Hour.ToString() + ".txt");
                        using (StreamWriter swAppend = File.AppendText(strFile.ToString()))
                        {
                            StringBuilder str = new StringBuilder();
                            str.AppendFormat("[{0}][{1}]    [{2}]", DateTime.Now, DateTime.Now.Millisecond.ToString("d4"), strData);
                            swAppend.WriteLine(str.ToString());
                        }
                    }
                    catch { }
                }
            }));

        }


        private static object Code2 = new object();



        /// <summary>
        /// 机台手动切换Hive状态记录
        /// </summary>
        /// <param name="data"></param>
        public static void WriteModification(string data)
        {
            Task.Factory.StartNew(new Action(() =>
            {
                lock (Code1)
                {
                    try
                    {
                        StringBuilder strFile = new StringBuilder();
                        strFile.AppendFormat("{0}\\{1}\\", AdminLog, DateTime.Now.ToString("yyyyMMdd"));
                        if (!Directory.Exists(strFile.ToString()))
                        {
                            Directory.CreateDirectory(strFile.ToString());
                        }
                        strFile.Append("hour_" + DateTime.Now.Hour.ToString() + ".txt");
                        using (StreamWriter swAppend = File.AppendText(strFile.ToString()))
                        {
                            StringBuilder str = new StringBuilder();
                            str.AppendFormat("[{0}][{1}]    [{2}]", DateTime.Now, DateTime.Now.Millisecond.ToString("d4"), data);
                            swAppend.WriteLine(str.ToString());
                        }
                    }
                    catch { }
                }
            }));
        }
    }
}
