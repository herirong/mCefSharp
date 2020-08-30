using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip;

namespace mCefSharp.WinForms.tools
{
    /// <summary>   
    /// Zip 压缩文件   
    /// </summary>   
    public class Zip
    {
        public Zip()
        {

        }
        #region 加压方法
        /// <summary>   
        /// 功能：压缩文件（暂时只压缩文件夹下一级目录中的文件，文件夹及其子级被忽略）   
        /// </summary>   
        /// <param name="dirPath">被压缩的文件夹夹路径</param>   
        /// <param name="zipFilePath">生成压缩文件的路径，为空则默认与被压缩文件夹同一级目录，名称为：文件夹名+.zip</param>   
        /// <param name="err">出错信息</param>   
        /// <returns>是否压缩成功</returns>   
        public static bool ZipFile(string dirPath, string zipFilePath, out string err)
        {
            err = "";
            if (dirPath == string.Empty)
            {
                err = "要压缩的文件夹不能为空！";
                return false;
            }
            if (!Directory.Exists(dirPath))
            {
                err = "要压缩的文件夹不存在！";
                return false;
            }
            //压缩文件名为空时使用文件夹名＋.zip   
            if (zipFilePath == string.Empty)
            {
                if (dirPath.EndsWith("\\"))
                {
                    dirPath = dirPath.Substring(0, dirPath.Length - 1);
                }
                zipFilePath = dirPath + ".zip";
            }

            try
            {
                string[] filenames = Directory.GetFiles(dirPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    s.SetLevel(9);
                    byte[] buffer = new byte[4096];
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            return true;
        }
        #endregion

        #region 解压
        /// <summary>   
        /// 功能：解压zip格式的文件。   
        /// </summary>   
        /// <param name="zipFilePath">压缩文件路径</param>   
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>   
        /// <param name="err">出错信息</param>   
        /// <returns>解压是否成功</returns>   
        public static bool UnZipFile(string zipFilePath, string unZipDir, out string err)
        {
            err = "";
            if (zipFilePath == string.Empty)
            {
                err = "压缩文件不能为空！";
                return false;
            }
            if (!File.Exists(zipFilePath))
            {
                err = "压缩文件不存在！";
                return false;
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹   
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith("\\"))
                unZipDir += "\\";
            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);

            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(unZipDir + directoryName);
                        }
                        if (!directoryName.EndsWith("\\"))
                            directoryName += "\\";
                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }//while   
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            return true;
        }//解压结束  
        #endregion

        #region 密码解压

        public static void UnZip(string directoryName, string ZipedFile, string password)
        {
            using (FileStream fileStreamIn = new FileStream(ZipedFile, FileMode.Open, FileAccess.Read))
            {
                using (ZipInputStream zipInStream = new ZipInputStream(fileStreamIn))
                {
                    zipInStream.Password = MD5encrypt(password);
                    ZipEntry entry = zipInStream.GetNextEntry();
                    // string filePath = directoryName + "\\" + entry.Name;
                    do
                    {
                        using (FileStream fileStreamOut = new FileStream(directoryName + @"\" + entry.Name, FileMode.Create, FileAccess.Write))
                        {

                            int size = 2048;
                            byte[] buffer = new byte[2048];
                            do
                            {
                                size = zipInStream.Read(buffer, 0, buffer.Length);
                                fileStreamOut.Write(buffer, 0, size);
                            } while (size > 0);
                        }
                    } while ((entry = zipInStream.GetNextEntry()) != null);
                }
            }
        }

        #endregion

        public static void ZipFile(string FileToZip, string ZipedFile, int CompressionLevel, int BlockSize)
        {
            if (!System.IO.File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("The specified file " + FileToZip + " could not be found. Zipping aborderd");
            }

            System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);
            ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
            ZipEntry ZipEntry = new ZipEntry("ZippedFile");
            ZipStream.PutNextEntry(ZipEntry);
            ZipStream.SetLevel(CompressionLevel);
            byte[] buffer = new byte[BlockSize];
            System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
            ZipStream.Write(buffer, 0, size);
            try
            {
                while (size < StreamToZip.Length)
                {
                    int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                    ZipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            ZipStream.Finish();
            ZipStream.Close();
            StreamToZip.Close();
        }
        /// <summary>
        /// 文件加密压缩
        /// </summary>
        /// <param name="FileToZip">需要压缩的文件路径</param>
        /// <param name="ZipedFile">压缩包路径（压缩包文件类型看自己需求）</param>
        /// <param name="password">加密密码</param>
        public static void ZipFileMain(string FileToZip, string ZipedFile, string password)
        {
            ZipOutputStream s = new ZipOutputStream(File.Create(ZipedFile));

            s.SetLevel(6); // 0 - store only to 9 - means best compression 
            s.Password = MD5encrypt(password); 
            //打开压缩文件 
            FileStream fs = File.OpenRead(FileToZip);

            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);

            Array arr = FileToZip.Split('\\');
            string le = arr.GetValue(arr.Length - 1).ToString();
            ZipEntry entry = new ZipEntry(le);
            entry.DateTime = DateTime.Now;
            entry.Size = fs.Length;
            fs.Close();
            s.PutNextEntry(entry);
            s.Write(buffer, 0, buffer.Length);
            s.Finish();
            s.Close();
        }
        
        #region "MD5加密"
        /// <summary>
        ///32位 MD5加密
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <returns></returns>
        public static string MD5encrypt(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            for (int i = 0; i < s.Length; i++)
            {
                pwd = pwd + s[i].ToString("X2");
            }
            return pwd;
        }
        #endregion

        #region 压缩字符串

        public static byte[] ZipString(string strSource, out string err)
        {
            err = "";
            byte[] byteReturn = null;
            if (string.IsNullOrEmpty(strSource))
            {
                return byteReturn;
            }
            byte[] byteSource = Encoding.Default.GetBytes(strSource);
            MemoryStream ms = new MemoryStream();
            GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(byteSource, 0, byteSource.Length);
            zip.Close();
            ms.Position = 0;
            byteReturn = new byte[ms.Length];
            ms.Read(byteReturn, 0, byteReturn.Length);
            ms.Close();
            return byteReturn;
        }

        public static string UnZipString(byte[] byteSource, out string err)
        {
            err = "";
            string strReturn = string.Empty;
            if (byteSource.Length == 0)
            {
                return strReturn;
            }
            byte[] byteReturn = new byte[1024];
            MemoryStream ms = new MemoryStream();
            ms.Write(byteSource, 0, byteSource.Length);
            ms.Position = 0;
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
            MemoryStream ms2 = new MemoryStream();
            while (true)
            {
                int bytesRead = zip.Read(byteReturn, 0, byteReturn.Length);
                if (bytesRead == 0)
                {
                    break;
                }
                ms2.Write(byteReturn, 0, bytesRead);
            }
            zip.Close();
            ms.Close();
            strReturn = Encoding.Default.GetString(ms2.ToArray(), 0, (int)ms2.Length);
            ms2.Close();
            return strReturn;
        }

        #endregion
    }
}