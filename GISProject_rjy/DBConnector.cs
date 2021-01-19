using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace GISProject_rjy
{
    class DBConnector
    {
        string connectionStringStr;
        string sqlStr;
        NpgsqlConnection Conn = new NpgsqlConnection();
        DataSet DS;
        bool ECode;
        string ErrString;

        public DBConnector()
        {
            //默认连接本机数据库mypostdb
            connectionStringStr = "User ID=postgres;Password=123456;Server=127.0.0.1;Port=5432;Database=mypostdb;";
        }

        /// <summary>
        /// 执行查询，返回数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>

        public DataSet GetRecordSet(string sql)
        {
            NpgsqlCommand sqlCmd = new NpgsqlCommand();
            sqlCmd.Connection = Conn;
            sqlCmd.CommandText = sql;
            try
            {
                NpgsqlDataAdapter adp = new NpgsqlDataAdapter(sqlCmd);
                DS = new DataSet();
                adp.Fill(DS);
            }
            catch (Exception e)
            {
                ErrString = e.Message;
                ECode = true;
                return null;
            }
            return DS;
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="Sqls"></param>
        public void ExecuteSQL(string Sqls)
        {
            NpgsqlCommand sqlCmd = new NpgsqlCommand();
            sqlCmd.Connection = Conn;
            sqlCmd.CommandText = Sqls;
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                ErrString = e.Message;
                ECode = true;
            }
        }

        /// <summary>
        /// 执行查询，返回DataReader
        /// </summary>
        /// <param name="Sqls"></param>
        /// <returns></returns>
        public NpgsqlDataReader DBDataReader(string Sqls)
        {
            NpgsqlCommand sqlCmd = new NpgsqlCommand();
            sqlCmd.Connection = Conn;
            sqlCmd.CommandText = Sqls;
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                return sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception e)
            {
                ErrString = e.Message;
                ECode = true;
                return null;
            }
        }

        /// <summary>
        /// 向数据库中写入海南各县的平均高程、最大高程、最小高程、平均风速、最大风速、最小风速
        /// </summary>
        /// <param name="avgDEM"></param>
        /// <param name="maxDEM"></param>
        /// <param name="minDEM"></param>
        /// <param name="avgSpeed"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="minSpeed"></param>
        public void InsertInfo(double avgDEM,double maxDEM,double minDEM,double avgSpeed,double maxSpeed,double minSpeed)
        {
            //稍后补上
        }


        public void DBClose()
        {
            try
            {
                Conn.Close();
            }
            catch (Exception e)
            {
                ErrString = e.Message;
                ECode = true;
            }
        }

        public bool ErrorCode()
        {
            return ECode;
        }
        public string ErrMessage()
        {
            return ErrString;
        }

    }
}
