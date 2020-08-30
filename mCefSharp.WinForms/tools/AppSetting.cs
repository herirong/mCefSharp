using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mCefSharp.WinForms.tools
{
    public static class AppSetting
    {
        public static Configuration Config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
        public static AppSettingsSection appSettings = (AppSettingsSection)Config.GetSection("appSettings");
        ///<summary> 
        ///返回config文件中appSettings配置节的value项  
        ///</summary> 
        ///<param name="strKey"></param> 
        ///<param name="config">Configuration实例</param>
        ///<returns></returns> 
        public static string GetAppSettings(string strKey)
        {
           return appSettings.Settings[strKey]?.Value;
        }
        public static void UpdateAppSettings(string newKey, string newValue)
        {
            //删除name，然后添加新值
            appSettings.Settings.Remove(newKey);
            appSettings.Settings.Add(newKey, newValue);

            //保存配置文件
            Config.Save();
        }
    }
}
