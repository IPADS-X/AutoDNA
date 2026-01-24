using CYAutoFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CYStandardProcedure
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

        public static int GetDNAMatcheCount(string filePath, string iDNA, string JianJi)
        {
            int count = 0;
            try
            {
                string pattern = "";
                char[] s = JianJi.ToArray();//识别碱基数量
                //string[] lines = File.ReadAllLines(filePath);
                var input = File.ReadAllText(filePath);
                pattern = iDNA + "{1,1}";
                for (int i = 0; i < s.Length; i++)
                {
                    pattern += s[i] + "{5,}";
                }
                count = Regex.Matches(input, pattern, RegexOptions.Multiline).Count;
                return count;
            }
            catch (Exception)
            {
                count = 0;
                return count;
            }
        }



        static List<string> lists = new List<string>();
        static string mostCommonString;
        /// <summary>
        /// 碱基识别推测
        /// </summary>
        /// <param name="number">碱基形成重复次数</param>
        /// <param name="iDNA">iDNA引物</param>
        /// <param name="filePaths">文件地址</param>
        /// <param name="mostJianJi">推测碱基值</param>
        /// <param name="quantity">推测的碱基值存在数量</param>
        public static void JianJiInfer(int number, string iDNA, string filePaths, out string mostJianJi, out int quantity)
        {
            try
            {
                mostJianJi = "";
                quantity = 0;
                StringBuilder sb = new StringBuilder();
                sb.Append(File.ReadAllText(filePaths));
                lists.Clear();
                bool b_infer = false;
                int num_infer = 0;//计数
                int index_infer = 0;//检索的字符
                string goout = "";//符合条件的字符
                string str_infer = sb.ToString();
                string[] parts = str_infer.Split(new string[] { iDNA }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 1; j < parts.Length; j++)
                {
                    for (int i = 0; i < parts[j].Length; i++)
                    {
                        if (parts[j][index_infer] == parts[j][i])
                        {
                            num_infer++;
                            if (num_infer >= number)
                            {
                                if (!b_infer)
                                {
                                    goout += parts[j][index_infer];
                                    b_infer = true;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (num_infer >= number)
                        {
                            index_infer = i;
                            b_infer = false;
                            num_infer = 1;
                        }
                        else
                        {
                            if (goout != "")
                            {
                                lists.Add(goout);
                            }
                            goout = "";
                            index_infer = 0;
                            num_infer = 0;
                            break;
                        }
                    }
                }
                mostCommonString = lists.GroupBy(x => x)
                                            .OrderByDescending(g => g.Count())
                                            .Select(g => g.Key)
                                            .First();
                quantity = lists.Count(x => x == mostCommonString);
                mostJianJi = mostCommonString;
            }
            catch (Exception ex)
            {
                mostJianJi = "";
                quantity = 0;
            }
        }


        /// <summary>
        /// 碱基识别推测(所有碱基对及对应值)
        /// </summary>
        /// <param name="number">碱基形成重复次数</param>
        /// <param name="iDNA">iDNA引物</param>
        /// <param name="filePaths">文件地址</param>
        /// <param name="mostJianJi">推测碱基值</param>
        /// <param name="quantity">推测的碱基值存在数量</param>
        public static Dictionary<string, int> SingleJianJiInfer(int number, string iDNA, string filePaths)
        {
            try
            {
                Dictionary<string, int> SingleJianJiDic = new Dictionary<string, int>();
                StringBuilder sb = new StringBuilder();
                sb.Append(File.ReadAllText(filePaths));
                lists.Clear();
                bool b_infer = false;
                int num_infer = 0;//计数
                int index_infer = 0;//检索的字符
                string goout = "";//符合条件的字符
                string str_infer = sb.ToString();
                string[] parts = str_infer.Split(new string[] { iDNA }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 1; j < parts.Length; j++)
                {
                    for (int i = 0; i < parts[j].Length; i++)
                    {
                        if (parts[j][index_infer] == parts[j][i])
                        {
                            num_infer++;
                            if (num_infer >= number)
                            {
                                if (!b_infer)
                                {
                                    goout += parts[j][index_infer];
                                    b_infer = true;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (num_infer >= number)
                        {
                            index_infer = i;
                            b_infer = false;
                            num_infer = 1;
                        }
                        else
                        {
                            if (goout != "")
                            {
                                lists.Add(goout);
                            }
                            goout = "";
                            index_infer = 0;
                            num_infer = 0;
                            break;
                        }
                    }
                }
                // 创建一个字典来存储元素及其出现次数
                Dictionary<string, int> elementCounts = new Dictionary<string, int>();
                // 遍历元素并统计出现次数
                foreach (var item in lists)
                {
                    if (elementCounts.ContainsKey(item))
                    {
                        // 如果元素已经存在于字典中，则增加出现次数
                        elementCounts[item]++;
                    }
                    else
                    {
                        // 否则，将元素添加到字典并初始化出现次数为 1
                        elementCounts.Add(item, 1);
                    }
                }
                var sortedElementCounts = elementCounts
                    .OrderByDescending(kvp => kvp.Value)
                    .ToList();
                SingleJianJiDic = sortedElementCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                return SingleJianJiDic;
            }
            catch (Exception ex)
            {
                Dictionary<string, int> SingleJianJiDic = new Dictionary<string, int>();
                return SingleJianJiDic;
            }
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
        public int DNAMatchedCount { get; set; }
        public string JianJiInfer { get; set; }
        public int JianJiInferCount { get; set; }
        public string JianJiInferResult { get; set; }

    }
}

