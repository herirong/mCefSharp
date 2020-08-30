// Copyright © 2010-2016 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp;
using System;
using System.Windows.Forms;
namespace mCefSharp.WinForms.Handlers
{
    internal class MenuHandler : IContextMenuHandler
    {
        private const int ShowDevTools = 26501;
        private const int CloseDevTools = 26502;
        private const int Close = 26503;
        private const int Lock = 26504;

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            //To disable the menu then call clear
            model.Clear();

            //Removing existing menu item
            //bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option

            //Add new custom menu items
            //model.AddItem((CefMenuCommand)ShowDevTools, "调试");
            //model.AddItem((CefMenuCommand)Lock, "锁定");
           // model.AddItem((CefMenuCommand)Close, "退出");
        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if ((int)commandId == ShowDevTools)
            {
                browser.ShowDevTools();
            }
            if ((int)commandId == Close)
            {
                browserControl.ExecuteScriptAsync("cefsharp.CloseAPP();");
                //var frm = (Globals.GetInstance()["MainForm"] as Form);
                //frm.Invoke(new Action(() =>
                //{
                //    frm.Close();
                //}));
            }
            if ((int)commandId == Lock)
            {
                browserControl.ExecuteScriptAsync("cefsharp.Lock();");
            }
            return false;//true表示拦截，默认false
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;//true表示拦截，默认false
        }
    }
}