using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.DataAccess.Extensions
{
    public class DbApp
    {
        public static async Task<IEnumerable<T>> Select<T>(string sqlQuery)
        {
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                try
                {
                    var result = await db.QueryAsync<T>(sqlQuery, commandType: CommandType.StoredProcedure);
                    db.Close();
                    db.Dispose();
                    return result;
                }
                catch (Exception error)
                {
                    //answer.ErrorGeneral = error.Message;
                    //answer.ErrorDetails = error.ToString();
                    db.Close();
                    db.Dispose();
                    return null;
                }
            }
        }

        public static async Task<T> Select<T>(string sqlQuery, DynamicParameters parameters)
        {
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                try
                {
                    var result = await db.QueryFirstOrDefaultAsync<T>(sqlQuery, parameters, commandType: CommandType.StoredProcedure);
                    db.Close();
                    db.Dispose();
                    return result;
                }
                catch (Exception error)
                {
                    //answer.ErrorGeneral = error.Message;
                    //answer.ErrorDetails = error.ToString();
                    db.Close();
                    db.Dispose();
                    return default(T);
                }
            }
        }

        public static async Task<bool> Update(string sqlQuery, DynamicParameters parameters)
        {
            bool resultSql = true;
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        await db.QueryAsync(sqlQuery, parameters, transaction, commandType: CommandType.StoredProcedure);
                        resultSql = false;
                        transaction.Commit();
                        db.Close();
                        db.Dispose();
                        return resultSql;
                    }
                    catch (Exception error)
                    {
                        transaction.Rollback();
                        db.Close();
                        db.Dispose();
                        return resultSql;
                    }
                }
            }
        }
        public static async Task<bool> Insert(string sqlQuery, DynamicParameters parameters)
        {
            bool resultSql = true;
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        await db.QueryAsync(sqlQuery, parameters, transaction, commandType: CommandType.StoredProcedure);
                        resultSql = false;
                        transaction.Commit();
                        db.Close();
                        db.Dispose();
                        return resultSql;
                    }
                    catch (Exception error)
                    {
                        transaction.Rollback();
                        db.Close();
                        db.Dispose();
                        return resultSql;
                    }
                }
            }
        }


        public static async Task<T> Find<T>(string sqlQuery, DynamicParameters parameters)
        {
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                try
                {
                    var result = await db.QueryFirstOrDefaultAsync<T>(sqlQuery, parameters, commandType: CommandType.StoredProcedure);
                    db.Close();
                    db.Dispose();
                    return result;
                }
                catch (Exception error)
                {
                    //answer.ErrorGeneral = error.Message;
                    //answer.ErrorDetails = error.ToString();
                    db.Close();
                    db.Dispose();
                    return default(T);
                }
            }
        }

        public static async Task<T> Detail<T>(string sqlQuery, DynamicParameters parameters)
        {

            return default(T);

        }

        public static async Task<Boolean> Delete(string sqlQuery, DynamicParameters parameters)
        {

            return true;

        }

    }
}
