using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SeqkitTool
{
    public class SeqkitHelper
    {
        /// <summary>
        /// 获取当前用户的文件夹路径
        /// </summary>
        public static string UserProfile = Environment.GetEnvironmentVariable("USERPROFILE");
        /// <summary>
        /// 匹配测序.gz文件
        /// </summary>
        /// <param name="iDNA">匹配：ATCAGTACGGTGCACCACCATGAA</param>
        /// <param name="filePath">.gz文件物理路径</param>
        /// <param name="txtPath">匹配后的txt文件物理路径</param>
        /// <returns></returns>
        public static string MatcheAsTxt(string iDNA, string filePath, string txtPath)
        {
            var command = $@"seqkit grep -s -i -p ""{iDNA}"" ""{filePath}"">""{txtPath}""";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c " + command;
            startInfo.WorkingDirectory = UserProfile;//当前用户的文件夹路径
            startInfo.UseShellExecute = false;   //是否使用操作系统shell启动 
            startInfo.CreateNoWindow = true;   //是否在新窗口中启动该进程的值 (不显示程序窗口)
            startInfo.RedirectStandardInput = true;  // 接受来自调用程序的输入信息 
            startInfo.RedirectStandardOutput = true;  // 由调用程序获取输出信息
            startInfo.RedirectStandardError = true;  //重定向标准错误输出

            string output = "";
            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    output = process.StandardOutput.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return output;
        }
        /// <summary>
        /// 源文件
        /// </summary>
        /// <param name="filePath">.gz文件物理路径</param>
        /// <param name="txtPath">匹配后的txt文件物理路径</param>
        /// <returns></returns>
        public static string OriginalAsTxt(string filePath, string txtPath)
        {
            var command = $@"seqkit sort -l ""{filePath}"">""{txtPath}""";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c " + command;
            startInfo.WorkingDirectory = UserProfile;//当前用户的文件夹路径
            startInfo.UseShellExecute = false;   //是否使用操作系统shell启动 
            startInfo.CreateNoWindow = true;   //是否在新窗口中启动该进程的值 (不显示程序窗口)
            startInfo.RedirectStandardInput = true;  // 接受来自调用程序的输入信息 
            startInfo.RedirectStandardOutput = true;  // 由调用程序获取输出信息
            startInfo.RedirectStandardError = true;  //重定向标准错误输出

            string output = "";
            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    output = process.StandardOutput.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return output;
        }

        /// <summary>
        /// 获取文件夹下.gz文件
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static List<ResultItemVM> GetFileList(string folderPath)
        {
            var list = new List<ResultItemVM>();
            int index = 0;
            string[] files = Directory.GetFiles(folderPath, "*");
            foreach (string filePath in files)
            {
                var model = new ResultItemVM()
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    FolderPath = folderPath,
                    FolderName = new DirectoryInfo(folderPath).Name,
                    Index = index,
                    OriginalTxtPath = folderPath + $"/original{index}.txt",
                    MatchedTxtPath = folderPath + $"/matched{index}.txt",
                };
                if (model.FileName.Contains(".gz"))
                {
                    list.Add(model);
                    index++;
                }
            }
            return list;
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="txtPath">txt文件物理路径</param>
        /// <returns></returns>
        public static int GetCount(string txtPath)
        {
            if (File.Exists(txtPath))
            {
                var text = File.ReadAllText(txtPath, Encoding.UTF8);
                return new Regex("runid=", RegexOptions.Compiled).Matches(text).Count;
            }
            return 0;
        }
        /// <summary>
        /// 测序匹配计数
        /// </summary>
        /// <param name="iDNA">匹配：ATCAGTACGGTGCACCACCATGAA</param>
        /// <param name="filePath">.gz文件物理路径</param>
        /// <returns></returns>
        public static int GetMatcheCount(string iDNA, string filePath)
        {
            var command = $@"seqkit grep -s -i -p ""{iDNA}"" ""{filePath}"" -C";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c " + command;
            startInfo.WorkingDirectory = UserProfile;//当前用户的文件夹路径
            startInfo.UseShellExecute = false;   //是否使用操作系统shell启动 
            startInfo.CreateNoWindow = true;   //是否在新窗口中启动该进程的值 (不显示程序窗口)
            startInfo.RedirectStandardInput = true;  // 接受来自调用程序的输入信息 
            startInfo.RedirectStandardOutput = true;  // 由调用程序获取输出信息
            startInfo.RedirectStandardError = true;  //重定向标准错误输出

            string output = "";
            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    output = process.StandardOutput.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            int count = 0;
            int.TryParse(output.Replace("\n", string.Empty), out count);
            return count;
        }
        /// <summary>
        /// 测序链数量
        /// </summary>
        /// <param name="filePath">.gz文件物理路径</param>
        /// <returns></returns>
        public static int GetOriginalCount(string filePath)
        {
            var command = $@"seqkit grep -i -r -p ^[ATCG]?$ ""{filePath}"" -C";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c " + command;
            startInfo.WorkingDirectory = UserProfile;//当前用户的文件夹路径
            startInfo.UseShellExecute = false;   //是否使用操作系统shell启动 
            startInfo.CreateNoWindow = true;   //是否在新窗口中启动该进程的值 (不显示程序窗口)
            startInfo.RedirectStandardInput = true;  // 接受来自调用程序的输入信息 
            startInfo.RedirectStandardOutput = true;  // 由调用程序获取输出信息
            startInfo.RedirectStandardError = true;  //重定向标准错误输出

            string output = "";
            try
            {
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    output = process.StandardOutput.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            int count = 0;
            int.TryParse(output.Replace("\n", string.Empty), out count);
            return count;
        }
    }


    public class ResultFolderVM
    {
        public ResultFolderVM()
        {
            this.SubFolderList = new List<ResultFolderVM>();
            this.FileList = new List<ResultItemVM>();
        }
        public int FolderLevel { get; set; }
        public string FolderName { get; set; }
        public string FolderPath { get; set; }
        public List<ResultFolderVM> SubFolderList { get; set; }
        public List<ResultItemVM> FileList { get; set; }
    }


    public class ResultItemVM
    {
        public int Index { get; set; }
        public string FolderName { get; set; }
        public string FolderPath { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string OriginalTxtPath { get; set; }
        public string MatchedTxtPath { get; set; }
        public int OriginalCount { get; set; }
        public int MatchedCount { get; set; }
    }
}
