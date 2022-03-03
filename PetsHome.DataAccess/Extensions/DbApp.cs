using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.DataAccess.Extensions
{
    public class DbApp
    {
        public static async Task<IEnumerable<T>> Select<T>(string sqlQuery)
        {
            using (var database = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                try
                {
                    var result = await database.QueryAsync<T>(sqlQuery, commandType: CommandType.StoredProcedure);
                    if (result == null && result.Count() > 0)
                    {
                    }
                    database.Close();
                    database.Dispose();
                    return result;
                }
                catch (Exception error)
                {
                    //answer.ErrorGeneral = error.Message;
                    //answer.ErrorDetails = error.ToString();
                    database.Close();
                    database.Dispose();
                    return null;
                }
            }
        }

        public static async Task<T> Select<T>(string sqlQuery, DynamicParameters parameters)
        {
            using (var database = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                try
                {
                    var result = await database.QueryFirstOrDefaultAsync<T>(sqlQuery, parameters, commandType: CommandType.StoredProcedure);
                    database.Close();
                    database.Dispose();
                    return result;
                }
                catch (Exception error)
                {
                    //answer.ErrorGeneral = error.Message;
                    //answer.ErrorDetails = error.ToString();
                    database.Close();
                    database.Dispose();
                    return default(T);
                }
            }
        }

        public static IEnumerable<T> Dropdown<T>(string sqlQuery)
        {
            using (var database = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                try
                {
                    var result = database.Query<T>(sqlQuery, commandType: CommandType.StoredProcedure);
                    if (result == null && result.Count() > 0)
                    {
                    }
                    database.Close();
                    database.Dispose();
                    return result;
                }
                catch (Exception error)
                {
                    //answer.ErrorGeneral = error.Message;
                    //answer.ErrorDetails = error.ToString();
                    database.Close();
                    database.Dispose();
                    return null;
                }
            }
        }

        public static async Task<bool> Update(string sqlQuery, DynamicParameters parameters)
        {
            bool resultSql = true;
            using (var database = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                database.Open();
                //using (var transaction = database.BeginTransaction())
                //{
                //try
                //{
                var result = await database.QueryAsync(sqlQuery, parameters, commandType: CommandType.StoredProcedure);
                if (result.Count() != 0)
                {
                    return true;
                }

                resultSql = false;

                //transaction.Commit();
                database.Close();
                database.Dispose();
                return resultSql;
                //}
                //catch (Exception error)
                //{
                //    transaction.Rollback();
                //    database.Close();
                //    database.Dispose();
                //    return resultSql;
                //}
                //}
            }
        }
        public static async Task<bool> Insert(string sqlQuery, DynamicParameters parameters)
        {
            bool resultSql = true;
            using (var database = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                database.Open();
                var result = await database.QueryAsync(sqlQuery, parameters, commandType: CommandType.StoredProcedure);
                if (result.Count() !=0)
                {
                    
                }
                resultSql = false;
                database.Close();
                database.Dispose();
                return resultSql;
            }
        }


        public static async Task<T> Find<T>(string sqlQuery, DynamicParameters parameters)
        {
            using (var database = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var result = await database.QueryFirstOrDefaultAsync<T>(sqlQuery, parameters, commandType: CommandType.StoredProcedure);
                database.Close();
                database.Dispose();
                return result;
            }
        }

        public static async Task<T> Detail<T>(string sqlQuery, DynamicParameters parameters)
        {

            return default(T);

        }

        public static async Task<Boolean> Delete(string sqlQuery, DynamicParameters parameters)
        {
            bool resultSql = true;
            using (var database = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                database.Open();
                var result = await database.QueryAsync(sqlQuery, parameters, commandType: CommandType.StoredProcedure);
                if (result.Count() != 0)
                {

                }
                resultSql = false;
                database.Close();
                database.Dispose();
                return resultSql;
            }
        }

    }
}
