using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using mCefSharp.WinForms.tools;
using NLog;

namespace mCefSharp.WinForms.BindObjects.plus
{
    public class Others
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static BrowserForm _browserForm;
        public Others(BrowserForm form)
        {
            _browserForm = form;
        }
        /// <summary>
        /// 获取设备唯一标识
        /// </summary>
        /// <returns></returns>
        public string getUUID()
        {
            return Zip.MD5encrypt(clsHardInfo.GetCpuID() + clsHardInfo.GetHardDiskID());
        }
        public void writeLog(string data)
        {
            Logger.Info(data);
        }
        public void SwitchFullscreen()
        {
            _browserForm.switchScreem();
        }
        public void setFullScreen()
        {
            _browserForm.setFullScreen();
        }
        /// <summary>
        /// 读文本文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string readTxtFile(string filePath)
        {
            try
            {
                //读文本文件
                FileStream fs = File.OpenRead(filePath);
                StreamReader sr = new StreamReader(fs);
                string ret = sr.ReadToEnd();
                sr.Close();
                fs.Close();
                return ret;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        /// <summary>
        /// 写文本文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string writeTxtFile(string filePath, string data)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                var di = fi.Directory;
                if (!di.Exists)
                    di.Create();
                //写记事本
                StreamWriter sw = new StreamWriter(filePath);
                sw.Write(data);
                sw.Flush();
                sw.Close();
                return "suc";
            }
            catch (Exception ex)
            {
                return "fail";
            }
        }
        public string getExeRootPath()
        {
            return Application.StartupPath.Replace("\\", "/");
        }

        public string uploadLog()
        {
            tools.file mfile=new file();
            mfile.UploadFile("D:\\windor\\Logs\\2020-08-12.log", "http://192.168.1.37:8005/api/SelfPay/UploadImages");
            return "1";
        }

        public string YDHZWebsiteValid(string url)
        {
            try
            {
                var result = tools.Request.HttpGet(url);
                if (result.StartsWith("<!DOCTYPE html>\n<html flag=\"YDHZ\"")|| result.StartsWith("<!DOCTYPE html><html flag=YDHZ")) return "";
                return "请输入银达汇智出品的网站";
            } 
            catch (Exception e)
            {

                return e.Message;
            }
        }
        //重启
        public void Reboot()
        {
            tools.cmd.RunCMD("shutdown.exe -r");
            Console.WriteLine("Reboot...");
        }
        //读取助手配置文件
        public string GetYjzSconfig(string section, string key)
        {

            var _ini = new iniFile(tools.AppSetting.GetAppSettings("HardwareAssistantPath") + "\\AnnSrv.ini");
            return _ini.IniReadValue(section, key);
        }
    }
}
