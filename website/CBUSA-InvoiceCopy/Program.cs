using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using System.Threading;

namespace CBUSA_InvoiceCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            string SourcePath = ConfigurationManager.AppSettings["ResourceGroupFileCopySourcePath"].ToString(); // "\\\\10.10.224.52\\cbusalob";
            string DestinationPath = ConfigurationManager.AppSettings["ResourceGroupFileCopyDestinationPath"].ToString(); ;
            string Result = "";
            Result= CopyFilesToLocalStorage(SourcePath, DestinationPath);
            Console.WriteLine(Result);
            Thread.Sleep(36000);
        }
        private static string CopyFilesToLocalStorage(string SourcePath, string DestinationPath)
        {
            string Result = "";
            bool FileCopied = false;
            try
            {
                if (System.IO.Directory.Exists(SourcePath))
                {
                    string FileName = "";
                    string DestFile = "";
                    if (System.IO.Directory.Exists(SourcePath))
                    {
                        string[] SourceFileList = System.IO.Directory.GetFiles(SourcePath);
                        // Copy the files and overwrite destination files if they already exist.
                        foreach (string Item in SourceFileList)
                        {                            
                            // Use static Path methods to extract only the file name from the path.
                            FileName = System.IO.Path.GetFileName(Item);
                            DestFile = System.IO.Path.Combine(DestinationPath, FileName);
                            
                            if(File.Exists(DestFile))
                            {
                                if (!FilesAreEqual_Hash(Item, DestFile))
                                {
                                    System.IO.File.Copy(Item, DestFile, true);
                                    FileCopied = true;
                                }
                            }
                            else
                            {
                                System.IO.File.Copy(Item, DestFile, true);
                                FileCopied = true;
                            }
                        }
                        Result = FileCopied == true ? "Files Copied!" : "Connection opned : No files found!";
                    }
                    else
                    {
                        Result = "Source path does not exist!";
                    }
                }
            }
            catch(Exception ee)
            {
                Result = ee.Message.ToString();
            }            
            return Result;
        }
        static bool FilesAreEqual_Hash(string FirstFile, string SecondFile)
        {
            try
            {               
                if(CalculateMD5(FirstFile) != CalculateMD5(SecondFile))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch(Exception ee)
            {
                return false;
            }
            
        }
        static string CalculateMD5(string FileName)
        {
            using (var Md5 = MD5.Create())
            {
                using (var Stream = File.OpenRead(FileName))
                {
                    var DataHash = Md5.ComputeHash(Stream);
                    return BitConverter.ToString(DataHash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
