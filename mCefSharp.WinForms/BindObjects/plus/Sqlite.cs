using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace mCefSharp.WinForms.BindObjects.plus
{
    public class Sqlite
    {
        private readonly sql.sqlite _sqliteHelper;
        public Sqlite()
        {
            _sqliteHelper = new sql.sqlite();
        }
        //open
        public object openDatabase(string name, string path)
        {
            var ret = new PlusHubs.ret();
            try
            {
                var _p = path.Split('/');
                path = _p[_p.Length - 1];
                path = @"D:\windor\" + path.Replace(".db", ".sqlite");
                //string fixedPath = @"D:\windor\sql.sqlite";
                _sqliteHelper.OpenDatabase(path);
                return ret;
            }
            catch (Exception e)
            {
                ret.state = false;
                ret.msg = e.Message;
                return ret;
            }
        }
        //executeSql
        public object executeSql(string name, string sql)
        {
            var ret = new PlusHubs.ret();
            try
            {
                _sqliteHelper.ExecuteSql(sql);
                return ret;
            }
            catch (Exception e)
            {
                ret.state = false;
                ret.msg = e.Message;
                return ret;
            }
        }
        //select sql
        public object selectSql(string name, string sql)
        {
            var ret = new PlusHubs.ret();
            try
            {
                var result = _sqliteHelper.SelectSql(sql);
                ret.data = JsonConvert.SerializeObject(result);
                return ret;
            }
            catch (Exception e)
            {
                ret.state = false;
                ret.msg = e.Message;
                return ret;
            }
        }
    }
}
