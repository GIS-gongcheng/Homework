using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;
using System.Data.OleDb;

namespace GISProject_rjy
{
    class DBConnector
    {
        string connectionStringStr;
        string sqlStr;
        //DataSet DS;
        //bool ECode;
        //string ErrString;

        public DBConnector()
        {
            //默认连接本机数据库mypostdb
            connectionStringStr = "UserID=postgres;Password=123456;Server=127.0.0.1;Port=5432;Database=mypostdb;";
        }
        

        

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql"></param>
        public void ExecuteSQL(string sql)
        {
            NpgsqlCommand cmd = null;
            NpgsqlConnection cnn = null;
            try
            {
                cnn = new NpgsqlConnection(connectionStringStr);
                cnn.Open();

                cmd = new NpgsqlCommand(sql, cnn);
                cmd.ExecuteNonQuery();

            }
            catch (Exception error)
            {
                System.Console.WriteLine(error.Message);
            }
        }

        /// <summary>
        /// 执行查询，返回DataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public NpgsqlDataReader DBDataReader(string sql)
        {
            NpgsqlCommand cmd = null;
            NpgsqlConnection cnn = null;
            try
            {
                cnn = new NpgsqlConnection(connectionStringStr);
                cnn.Open();

                cmd = new NpgsqlCommand(sql, cnn);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);

            }
            catch (Exception error)
            {
                System.Console.WriteLine(error.Message);
                return null;
            }
        }



        /// <summary>
        /// 向数据库中添加新表高程统计数据，包括海南各县的code和平均高程、最大高程、最小高程
        /// </summary>
        public void AddDEMTable()
        {
            //首先向数据库里添加一个空表
            NpgsqlCommand cmd = null;
            NpgsqlConnection cnn = null;
            sqlStr = "create table DEMstatistics(" +
                "code character varying(6)," +
                "avgDEM numeric,maxDEM numeric,minDEM numeric); ";
            try
            {
                cnn = new NpgsqlConnection(connectionStringStr);
                cnn.Open();

                cmd = new NpgsqlCommand(sqlStr, cnn);
                cmd.ExecuteNonQuery();

            }
            catch (Exception error)
            {
                System.Console.WriteLine(error.Message);
            }
        }


        /// <summary>
        /// 向数据库中添加新表风速统计数据，包括海南各县的code和平均风速、最大风速、最小风速
        /// </summary>
        public void AddSpeedTable()
        {
            //首先向数据库里添加一个空表
            NpgsqlCommand cmd = null;
            NpgsqlConnection cnn = null;
            sqlStr = "create table WindSpeedStatistics(" +
                "code character varying(6)," +
                "avgSpeed numeric,maxSpeed numeric,minSpeed numeric); ";
            try
            {
                cnn = new NpgsqlConnection(connectionStringStr);
                cnn.Open();

                cmd = new NpgsqlCommand(sqlStr, cnn);
                cmd.ExecuteNonQuery();

            }
            catch (Exception error)
            {
                System.Console.WriteLine(error.Message);
            }
        }

        /// <summary>
        /// 将高程统计数据导入数据库中
        /// </summary>
        /// <param name="info"></param>
        public void InsertDEMInfo(List<float[]> info)
        {
            NpgsqlCommand cmd = null;
            NpgsqlConnection cnn = null;
            int i,j;
            for(i = 0;i < info.Count;i++)
            {
                sqlStr = "insert into DEMstatistics values ('";
                for(j=0;j<3;j++)
                {
                    sqlStr += info[i][j].ToString() + "','";
                }
                sqlStr += info[i][3].ToString() + "');";
                try
                {
                    cnn = new NpgsqlConnection(connectionStringStr);
                    cnn.Open();

                    cmd = new NpgsqlCommand(sqlStr, cnn);
                    cmd.ExecuteNonQuery();

                }
                catch (Exception error)
                {
                    System.Console.WriteLine(error.Message);
                }
            }
        }

        /// <summary>
        /// 将风速统计数据导入数据库中
        /// </summary>
        /// <param name="info"></param>
        public void InsertSpeedInfo(List<float[]> info)
        {
            NpgsqlCommand cmd = null;
            NpgsqlConnection cnn = null;
            int i, j;
            for (i = 0; i < info.Count; i++)
            {
                sqlStr = "insert into WindSpeedStatistics values ('";
                for (j = 0; j < 3; j++)
                {
                    sqlStr += info[i][j].ToString() + "','";
                }
                sqlStr += info[i][3].ToString() + "');";
                try
                {
                    cnn = new NpgsqlConnection(connectionStringStr);
                    cnn.Open();

                    cmd = new NpgsqlCommand(sqlStr, cnn);
                    cmd.ExecuteNonQuery();

                }
                catch (Exception error)
                {
                    System.Console.WriteLine(error.Message);
                }
            }
        }

        
    }
}
