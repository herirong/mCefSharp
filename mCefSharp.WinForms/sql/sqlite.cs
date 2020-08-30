using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mCefSharp.WinForms.sql
{
    public class sqlite
    {
        private SQLiteConnection cn;
        private String _path;
        public void OpenDatabase(string path)
        {
            if (cn != null)
                return;
            //string path1 = @"D:\windor\sql.sqlite";
            _path = path;
            cn = new SQLiteConnection("data source=" + path);
            cn.Open();
            cn.Close();
        }
        //Delete the database
        public void DeleteDB(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        //execute Sql
        public void ExecuteSql(string sql)
        {
            if (cn.State != System.Data.ConnectionState.Open)
            {
                cn.Open();
            }
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = cn;
            cmd.CommandText = sql;
            //cmd.CommandText = "CREATE TABLE IF NOT EXISTS t1(id varchar(4),score int)";
            cmd.ExecuteNonQuery();
            cn.Close();

        }
        //select Sql
        public List<Dictionary<string, object>> SelectSql(string sql)
        {
            if (cn.State != System.Data.ConnectionState.Open)
            {
                cn.Open();
            }
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = cn;
            cmd.CommandText = sql;
            DataTable dt = new DataTable();
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(dt);
            Dictionary<string, object> row;
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }
    }
}
