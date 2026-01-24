using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYStandardProcedure
{
    public class SQLiteHelper
    {
        /*************判断数据库中是否有数据表存在 SELECT count(*) from sqlite_master where type='table' and name='表名称'  ********************/

        private SQLiteConnection Sqlite;
        public SQLiteHelper(string DatabaseName)
        {
            Sqlite = new SQLiteConnection($"Data Source={DatabaseName };Version=3");
        }
        /// <summary>
        /// 返回查询结果中的第一行第一列的值
        /// </summary>
        /// <param name="sqlstr">sql查询语句</param>
        /// <returns>正常返回值，出现异常返回null</returns>
        public object ExecSQL(string sqlitestr)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(sqlitestr, Sqlite);
                if (Sqlite.State == ConnectionState.Closed)
                {
                    Sqlite.Open();
                }
                object num = cmd.ExecuteScalar().ToString();
                Sqlite.Close();
                return num;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 执行增删改操作
        /// </summary>
        /// <param name="sqlstr">sql执行语句</param>
        /// <returns>返回执行成功行数，执行失败返回-1</returns>
        public int ExecSQLResult(string sqlstr)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(sqlstr, Sqlite);
                if (Sqlite.State == System.Data.ConnectionState.Closed)
                {
                    Sqlite.Open();
                }
                int result = cmd.ExecuteNonQuery();
                Sqlite.Close();
                return result;
            }
            catch
            {
                return -1;
            }
        }




        /// <summary>
        /// 根据条件查询SQL数据
        /// </summary>
        /// <param name="sqlstr">sql语句</param>
        /// <returns>返回数据，查询失败返回null</returns>
        public DataSet GetDataSet(string sqlstr)
        {
            try
            {
                SQLiteDataAdapter sqlda = new SQLiteDataAdapter(sqlstr, Sqlite);
                DataSet ds = new DataSet();
                sqlda.Fill(ds);
                return ds;
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// 获取当前数据库状态
        /// </summary>
        public bool State
        {
            get
            {
                try
                {
                    Sqlite.Open();
                    if (Sqlite.State == ConnectionState.Open)
                    {
                        Sqlite.Close();
                        return true;
                    }
                    else
                    {
                        Sqlite.Close();
                        return false;
                    }

                }
                catch
                {
                    Sqlite.Close();
                    return false;
                }
            }
        }
    }
}
