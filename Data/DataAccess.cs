using Dapper;
using Microsoft.Data.SqlClient;
using TugasAsinkron2.Interfaces;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace TugasAsinkron2.Data
{
    public class DataAccess : IDataAccess
    {
        //Connection string bisa disimpan di appsettings.json
        private string _conntectionString { get; set; }

        public DataAccess()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var config = builder.Build();
            _conntectionString = config.GetConnectionString("DefaultConnection");
        }


        //Kemudian kita akan membuat method yang dapat menerima input berupa SQL query
        //2 Method akan dibuat, satu untuk membaca query ke database dan menampilkan tabel
        //dan untuk menyimpan query / memodifikasi database
        public async Task<List<T>> Read <T, U>(string sql, U parameters)
        {
            using (IDbConnection con = new SqlConnection(_conntectionString))
            {
                var data = await con.QueryAsync<T>(sql, parameters);
                return data.ToList();
            }
        }

        public async Task Write<T>(string sql, T parameters)
        {
            using (IDbConnection con = new SqlConnection(_conntectionString))
            {
                await con.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<string> ExecuteTransaction<T>(List<KeyValuePair<string, T>> sqlDict)
        {
            using (IDbConnection connection = new SqlConnection(_conntectionString))
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                //Query-query dibawah ini akan dieksekusi didalam transaction

                try
                {
                    foreach(KeyValuePair<string, T> pair in sqlDict)
                    {
                        await connection.ExecuteAsync(pair.Key, pair.Value, transaction);
                    }
                }
                catch(Exception e)
                {
                    //Jika exception ditangkap, transaction akan di rollback sehingga seolah-olah tidak ada 
                    //perubahan pada database
                    transaction.Rollback();
                    return "Transaction Failed";
                }
                //Jika tidak ada exception yang ditangkap, transaction akan di commit dan 
                //query akan disimpan. 
                transaction.Commit();
                connection.Close();
            }
            return "Transaction Success";
        }

        public async Task<string> BulkInsert(DataTable data, string tableName)
        {
            string result = null;
            DataColumn col = data.Columns.Add("Id", Type.GetType("System.Int32"));
            col.SetOrdinal(0);
            using (SqlConnection con = new SqlConnection(_conntectionString))
            {
                con.Open();
                using (SqlBulkCopy copy = new SqlBulkCopy(con))
                {
                    copy.DestinationTableName = tableName;
                    try
                    {
                        await copy.WriteToServerAsync(data);
                        result = "Success";
                    }
                    catch (Exception e)
                    {
                        result = "Failed " + e.Message;
                    }
                }
                con.Close();
            }
            return result;
        }


    }
}
