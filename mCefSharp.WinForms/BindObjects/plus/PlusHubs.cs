using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace mCefSharp.WinForms.BindObjects.plus
{
    public class PlusHubs
    {
        private static BrowserForm _browserForm;
        public PlusHubs(BrowserForm form)
        {
            _browserForm = form;
        }
        public PlusHubs()
        {
        }

        //sqlite数据库
        public Sqlite sqlite => new Sqlite();
        //其他
        public Others others => new Others(_browserForm);

        public class ret
        {
            public bool state { get; set; } = true;
            public object data { get; set; } = null;
            public string msg { get; set; } = "success";
        }
    }
}
