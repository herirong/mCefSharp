using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using mCefSharp.WinForms.tools;

namespace mCefSharp.WinForms
{
    public partial class FrmInputPwd : Form
    {

        public FrmInputPwd()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text += (sender as Button).Text;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
                button13_Click(sender, e);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (!CheckPwd()) return;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private bool CheckPwd()
        {
            if (textBox1.Text != getSettingPwd())
            {
                this.textBox1.Text = "";
                ShowMsg("密码错误，请重新输入");
                textBox1.Focus();
                return false;
            }
            else
                return true;
        }
        public string getSettingPwd()
        {
            var myDate = DateTime.Now;
            //获取当前年
            var year = myDate.Year;
            //获取当前月
            var month = myDate.Month;
            //获取当前日
            var date = myDate.Day;
            //var h = myDate.getHours(); //获取当前小时数(0-23)
            //var m = myDate.getMinutes(); //获取当前分钟数(0-59)
            //var s = myDate.getSeconds();
            //var ampm='A';
           var newMonth = month < 10 ? ("0" + month) : month.ToString();   
           var newDate = date < 10 ? ('0' + date) : date;
            //h = h < 10 ? '0' + h : h;
            var now = year + "" + newMonth + "" + newDate + "" + "ydhz";
            var nowmd5 = Zip.MD5encrypt(now).ToUpper();

            nowmd5=Regex.Replace(nowmd5, "A", "10");
            nowmd5=Regex.Replace(nowmd5, "B", "11");
            nowmd5=Regex.Replace(nowmd5, "C", "12");
            nowmd5=Regex.Replace(nowmd5, "D", "13");
            nowmd5=Regex.Replace(nowmd5, "E", "14");
            nowmd5=Regex.Replace(nowmd5, "F", "15");


            var pwd = nowmd5.Substring(6, 6);
            return pwd;
        }

        private void ShowMsg(string msg)
        {
            label2.Text = msg;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ShowMsg("");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") return;
            textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 1);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void FrmInputPwd_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                ShowMsg("");
                textBox1.Focus();
                textBox1.Text = "";
            }
        }


        private void FrmInputPwd_Load(object sender, EventArgs e)
        {
            //
        }

        private void btnDebugTool_Click(object sender, EventArgs e)
        {

        }
    }
}
