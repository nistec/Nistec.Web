using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Xml;
using System.Web.UI;
using System.Diagnostics;


namespace Nistec.Web
{

    public class IOhelper
    {

        /// <summary>
        /// Returns the names of files in a specified directories that match the specified patterns using LINQ
        /// </summary>
        /// <param name="srcDirs">The directories to seach</param>
        /// <param name="searchPatterns">the list of search patterns</param>
        /// <param name="searchOption"></param>
        /// <returns>The list of files that match the specified pattern</returns>
        public static string[] GetFiles(string[] srcDirs,
             string[] searchPatterns,
             SearchOption searchOption = SearchOption.AllDirectories)
        {
            var r = from dir in srcDirs
                    from searchPattern in searchPatterns
                    from f in Directory.GetFiles(dir, searchPattern, searchOption)
                    select f;

            return r.ToArray();
        }



        //string account_folder = IOhelper.GetAccountFolder("files");
        //string uploadingFolder = ResolveUrl("~/Uploads/" + account_folder + "/files");

        //public static string GetUploadFolder(Page p, string folder)
        //{
        //    //SetAccount();
        //    string account_folder = "0";
        //    account_folder = Sessions.GetAccount(p);
        //    string uploadingFolder = p.ResolveUrl("~/Uploads/" + account_folder + "/" + folder);

        //    if (!IsDirectoryExists(uploadingFolder))
        //    {
        //        CreateAccount("~/Uploads/" + account_folder + "/", "images", "flash", "media", "files");
        //    }

        //    return uploadingFolder;
        //}


        //public static string GetAccountFolder(string dirPath)
        //{

        //    //SetAccount();
        //    string accountFolder = "0";
        //    accountFolder = Sessions.GetAccount();

        //    if (!Directory.Exists(dirPath))
        //    {
        //        CreateAccount("~/uploads/" + accountFolder + "/", "images", "flash", "media", "files");
        //    }

        //    return accountFolder;
        //}
       
        public static bool IsDirectoryExists(string virtualPath)
        {
           
            string dirPath = MapPath(virtualPath, false);
            return Directory.Exists(dirPath);
        }

        //Get's list of DirectoryNames for the given path
        public static string[] GetDirectoryNames(string dirPath)
        {
            string[] dlist = null;
            dirPath = MapPath(dirPath, false);
            if (Directory.Exists(dirPath))
            {
                dlist = Directory.GetDirectories(dirPath);
                if (dlist != null && dlist.Length > 0)
                {
                    int len = dlist.Length;
                    for (int i = 0; i < len; i++)
                    {
                        dlist[i] = stripRoot(dirPath, dlist[i]);
                    }
                }
            }
            return dlist;
        }

        //Get's list of FileNames for the given path
        public static string[] GetFileNames(string dirPath)
        {
            string[] flist = null;
            dirPath = MapPath(dirPath, false);
            if (Directory.Exists(dirPath))
            {
                flist = Directory.GetFiles(dirPath);
                if (flist != null && flist.Length > 0)
                {
                    int len = flist.Length;
                    for (int i = 0; i < len; i++)
                    {
                        flist[i] = stripRoot(dirPath, flist[i]);
                    }
                }
            }
            return flist;
        }

        //Get's File Size
        public static long GetFileSize(string filePath)
        {
            filePath = MapPath(filePath, false);
            if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length;
            }
            return 0;
        }

        public static void DeleteFile(string dirPath, string fileName)
        {
            //This method is called only if the dirPath is included in DeletePaths.
            //You can add custom logic here.
            dirPath = MapPath(dirPath, true);
            if (File.Exists(dirPath + fileName))
            {
                File.Delete(dirPath + fileName);
            }
        }

        public static void CreateAccount(string dirPath, params string[] newDirNames)
        {
            //This method is called only if the dirPath is included in ViewPaths.
            //You can add custom logic here.
            dirPath = MapPath(dirPath, true);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                foreach (string s in newDirNames)
                {
                    Directory.CreateDirectory(dirPath + s);
                }
            }
        }
        public static void CreateDirecories(string dirPath, params string[] newDirNames)
        {
            //This method is called only if the dirPath is included in ViewPaths.
            //You can add custom logic here.
            dirPath = MapPath(dirPath, true);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            foreach (string s in newDirNames)
            {
                Directory.CreateDirectory(dirPath + s);
            }

        }
        public static void CreateDirectory(string dirPath, string newDirName)
        {
            //This method is called only if the dirPath is included in ViewPaths.
            //You can add custom logic here.
            dirPath = MapPath(dirPath, true);
            if (!Directory.Exists(dirPath + newDirName))
            {
                Directory.CreateDirectory(dirPath + newDirName);
            }
        }

        public static void DeleteDirectory(string dirPath, string dirName)
        {
            //This method is called only if the dirPath is included in DeletePaths.
            //You can add custom logic here.
            dirPath = MapPath(dirPath, true);
            if (Directory.Exists(dirPath + dirName))
            {
                Directory.Delete(dirPath + dirName);
            }
        }

        public static bool CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting)
        {
            bool ret = false;
            try
            {
                SourcePath=MapPath(SourcePath, true);
                DestinationPath = MapPath(DestinationPath, true);
                //SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
                //DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                        Directory.CreateDirectory(DestinationPath);

                    foreach (string fls in Directory.GetFiles(SourcePath))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                    }
                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        if (CopyDirectory(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)
                            ret = false;
                    }
                }
                ret = true;
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;
        }  

        private static string MapPath(string url, bool addSlash_pm)
        {
            if (url == null) { return url; }
            if (HttpContext.Current != null)
            {
                if (addSlash_pm && !url.EndsWith("/"))
                {
                    url = url + "/";
                }
                if (url.StartsWith("~/"))
                {
                    url = url.Substring(1);
                    string root = HttpContext.Current.Request.ApplicationPath;
                    if (root != "/")
                    {
                        url = root + url;
                    }
                }
                if ((!url.StartsWith("file://") && !url.StartsWith("http://")) && (!url.StartsWith("https://") && (url.IndexOf(@"\") == -1)))
                {
                    url = HttpContext.Current.Server.MapPath(url);
                }
            }
            return url;
        }

        private static string stripRoot(string root, string path)
        {
            if (root == null || path == null || path.EndsWith("/") || path.EndsWith("\\")) { return path; }
            path = path.Replace(root, "");
            path = path.TrimStart(new char[] { '/', '\\' });
            return path;
        }

        public static void CreateContentDirectory(int accountId, string contentPath)
        {
            string accFolder = contentPath + accountId.ToString();
            System.IO.Directory.CreateDirectory(accFolder);
        }

        public static string GetContentPath(int accountId, string contentPath, string filename)
        {
            string accFolder = contentPath + accountId.ToString();
            return accFolder + "\\" + System.IO.Path.GetFileName(filename);
        }

        public static string GetContentSaveName(int accountId, string filename)
        {
            return accountId.ToString() + "/" + System.IO.Path.GetFileName(filename);
        }

        public static string SaveFile(int accountId, string uploadFilesPath, string contentPath, string filename)
        {
            CreateContentDirectory(accountId, contentPath);
            string nfilename = GetContentPath(accountId, contentPath, filename);
            //uf.Save(new FileStream(nfilename, true));

            string sourcefile = System.IO.Path.Combine(uploadFilesPath, System.IO.Path.GetFileName(filename));
            System.IO.File.Copy(sourcefile, nfilename, true);
            //System.IO.File.Delete(sourcefile);

            return accountId.ToString() + "/" + System.IO.Path.GetFileName(filename);
        }

        public static void RemoveFile(string uploadFilesPath, string filename)
        {
            string sourcefile = System.IO.Path.Combine(uploadFilesPath, System.IO.Path.GetFileName(filename));
            System.IO.File.Delete(sourcefile);
        }


        public static string RunProcessWithResults(string url, string args)
        {
            string response = null;

            ProcessStartInfo psi = new ProcessStartInfo(url);
            psi.RedirectStandardOutput = true;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            psi.Arguments = args;
            //Process proc;
            using (Process exeProcess = Process.Start(psi))
            {
                System.IO.StreamReader stream = exeProcess.StandardOutput;
                exeProcess.WaitForExit();// (2000);
                if (exeProcess.HasExited)
                {
                    string output = stream.ReadToEnd();
                    response = output;
                }
            }
            return response;
        }

        public static void RunProcess(string url, string args)
        {

            // For the example
            //string alarmFileName = SrvConfig.SystemAlarmUrl;
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = url;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = args;


            // Start the process with the info we specified.
            // Call WaitForExit and then the using statement will close.
            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
            }
        }

        public static void ReplaceTextInFile(string originalFile, string outputFile, string searchTerm, string replaceTerm)
        {
            string tempLineValue;
            using (FileStream inputStream = File.OpenRead(originalFile))
            {
                using (StreamReader inputReader = new StreamReader(inputStream))
                {
                    using (StreamWriter outputWriter = File.AppendText(outputFile))
                    {
                        while (null != (tempLineValue = inputReader.ReadLine()))
                        {
                            outputWriter.WriteLine(tempLineValue.Replace(searchTerm, replaceTerm));
                        }
                    }
                }
            }
        }
        public static void WriteTextInFile(string filename, string output)
        {
            FileInfo fi = new FileInfo(filename);
            using (TextWriter tw = new StreamWriter(fi.Open(FileMode.Truncate),System.Text.Encoding.UTF8))
            {
                tw.Write(output);
            }
        }
    }
}
