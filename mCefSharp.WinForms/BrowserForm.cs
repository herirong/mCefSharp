// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using mCefSharp.WinForms.Controls;
using CefSharp.WinForms;
using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using CefSharp;
using mCefSharp.WinForms.Handlers;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace mCefSharp.WinForms
{
    public partial class BrowserForm : Form
    {
        private readonly ChromiumWebBrowser browser;
        private static bool debug;
        private static bool fullScreen;
        public static Form mForm;
        public Process yjzsProcess = null;
        private static bool _formClosingState;
        private static bool MainClosingState;
        public BrowserForm()
        {
            Process.GetCurrentProcess().EnableRaisingEvents = true;
            System.Windows.Forms.Application.ThreadExit += main_Exited;
            mForm = this;
            debug = Convert.ToBoolean(System.Configuration.ConfigurationSettings.AppSettings["debug"]);
            InitializeComponent();

            Text = "银达汇智自助终端V3.03";
            WindowState = FormWindowState.Maximized;

            browser = new ChromiumWebBrowser("localfolder://cefsharp/index.html");
            toolStripContainer.ContentPanel.Controls.Add(browser);

            browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            // browser.LoadingStateChanged += OnLoadingStateChanged;
            // browser.ConsoleMessage += OnBrowserConsoleMessage;
            // browser.StatusMessage += OnBrowserStatusMessage;
            // browser.TitleChanged += OnBrowserTitleChanged;
            // browser.AddressChanged += OnBrowserAddressChanged;
            browser.KeyboardHandler = new KeyboardHandler();
            browser.LifeSpanHandler = new LifeSpanHandler();
            browser.MenuHandler = new MenuHandler();
            //var url= ConfigurationSettings.AppSettings["URL"];

            // url = ShowDialog("请输入web地址", "温馨提示");
            // var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // config.AppSettings.Settings["URL"].Value = url;
            // config.Save(ConfigurationSaveMode.Modified);

            //ConfigurationManager.RefreshSection("appSettings");
            //if (!string.IsNullOrEmpty(url)) LoadUrl(url);

            var HardwareAssistantPath = tools.AppSetting.GetAppSettings("HardwareAssistantPath");
            if (string.IsNullOrEmpty(HardwareAssistantPath))
            {
                ShowSelectFileDialog();
            }
            else
            {
                if (!IsHardwareAssistantPathAndStart(HardwareAssistantPath))
                {
                    ShowSelectFileDialog();
                }
            }

            registCustomObject();

        }
        public string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                ControlBox = false,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            textBox.Text = ConfigurationSettings.AppSettings["URL"];
            Button confirmation = new Button() { Text = "确定", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void registCustomObject()
        {
            browser.JavascriptObjectRepository.Register("plus", new BindObjects.plus.PlusHubs(this), isAsync: false, options: BindingOptions.DefaultBinder);
        }

        private void OnIsBrowserInitializedChanged(object sender, EventArgs e)
        {
            var b = ((ChromiumWebBrowser)sender);

            this.InvokeOnUiThreadIfRequired(() => b.Focus());
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput(string.Format("Line: {0}, Source: {1}, Message: {2}", args.Line, args.Source, args.Message));
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);

            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = args.Address);
        }

        private void SetCanGoBack(bool canGoBack)
        {
            this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }

        private void SetIsLoading(bool isLoading)
        {
            goButton.Text = isLoading ?
                "Stop" :
                "Go";
            goButton.Image = isLoading ?
                Properties.Resources.nav_plain_red :
                Properties.Resources.nav_plain_green;

            HandleToolStripLayout();
        }

        public void DisplayOutput(string output)
        {
            this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }

        private void HandleToolStripLayout()
        {
            var width = toolStrip1.Width;
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item != urlTextBox)
                {
                    width -= item.Width - item.Margin.Horizontal;
                }
            }
            urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            browser.Dispose();
            Cef.Shutdown();
            Close();
        }

        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            browser.Back();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            browser.Forward();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            LoadUrl(urlTextBox.Text);
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                browser.Load(url);
            }
        }

        private void ShowDevToolsMenuItemClick(object sender, EventArgs e)
        {
            browser.ShowDevTools();
        }

        private void FormClosingEvent(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason.ToString() == "UserClosing")
            {

                if (new FrmInputPwd().ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _formClosingState = true;
                    if (yjzsProcess != null && !yjzsProcess.HasExited) yjzsProcess.Kill();

                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {


            }
        }

        private void fullScreen_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switchScreem();
        }

        private void BrowserForm_KeyDown(object sender, KeyEventArgs e)
        {
        }

        public void switchScreem()
        {
            if (fullScreen)
            {
                ExitFullScreen();
            }
            else
            {
                setFullScreen();
            }
        }

        public void setFullScreen()
        {
            try
            {
                this.menuStrip1.Hide();
                //this.toolStrip1.Hide();
                this.TopMost = true;

                this.SetVisibleCore(false);
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.SetVisibleCore(true);
                fullScreen = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("setFullScreen:" + ex.Message);
            }
        }
        public void ExitFullScreen()
        {
            try
            {
                this.menuStrip1.Show();
                //this.toolStrip1.Show();
                this.TopMost = false;

                this.FormBorderStyle = FormBorderStyle.Fixed3D;
                this.WindowState = FormWindowState.Maximized;
                fullScreen = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("setFullScreen:" + ex.Message);
            }
        }

        private void 助手路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = ShowSelectFileDialog(isMust: false);
        }

        public string ShowSelectFileDialog(string text = "请选手助手路径", bool isMust = true)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.Description = text;
            var dialog = path.ShowDialog(this);
            if (dialog == System.Windows.Forms.DialogResult.OK)
            {

                var result = IsHardwareAssistantPathAndStart(path.SelectedPath);
                if (!result) return ShowSelectFileDialog("路径错误！请重新选择硬件助手路径", isMust);
                tools.AppSetting.UpdateAppSettings("HardwareAssistantPath", path.SelectedPath);

                return path.SelectedPath;
            }

            if (!isMust) return null;
            ExitMenuItemClick(null, new EventArgs());
            return null;
        }

        public bool IsHardwareAssistantPathAndStart(string path)
        {
            var newpath = path + "\\银达汇智硬件助手.exe";
            var isExist = File.Exists(newpath);
            if (isExist)
            {
                if (yjzsProcess != null && !yjzsProcess.HasExited) yjzsProcess.Kill();

                var _iniFile = new tools.iniFile(path + "\\AnnSrv.ini");
                _iniFile.IniWriteValue("WebSocket", "AutoStart", "1");
                _iniFile.IniWriteValue("Reader", "Enable", "1");
                yjzsProcess = System.Diagnostics.Process.Start(newpath);
                yjzsProcess.EnableRaisingEvents = true;
                yjzsProcess.Exited += new EventHandler(yjzs_Exited);
            }
            StringBuilder temp = new StringBuilder(255);
            return isExist;
        }
        void yjzs_Exited(object sender, EventArgs e)
        {
            if (!_formClosingState)
            {
                yjzsProcess.Start();
            }
        }
        void main_Exited(object sender, EventArgs e)
        {
            if (!_formClosingState)
            {
                System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\银达汇智终端.exe");
            }
        }
    }
}
