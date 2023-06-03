using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.DataAccess.Extensions
{
    /// <summary>
    /// Clase que contiene los métodos para la conexión a la base de datos.
    /// </summary>
    public class DbApp
    {
        /// <summary>
        /// Realiza una consulta SELECT a la base de datos.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto a retornar.</typeparam>
        /// <param name="sqlQuery">La consulta SQL.</param>
        /// <returns>Una colección de objetos del tipo especificado.</returns>
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
                catch (Exception)
                {
                    database.Close();
                    database.Dispose();
                    return null;
                }
            }
        }

        /// <summary>
        /// Realiza una consulta SELECT a la base de datos utilizando parámetros.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto a retornar.</typeparam>
        /// <param name="sqlQuery">La consulta SQL.</param>
        /// <param name="parameters">Los parámetros de la consulta.</param>
        /// <returns>Una colección de objetos del tipo especificado.</returns>
        public static async Task<IEnumerable<T>> SelectById<T>(string sqlQuery, DynamicParameters parameters)
        {
            using (var database = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                try
                {
                    var result = await database.QueryAsync<T>(sqlQuery, parameters, commandType: CommandType.StoredProcedure);
                    if (result == null && result.Count() > 0)
                    {
                    }
                    database.Close();
                    database.Dispose();
                    return result;
                }
                catch (Exception)
                {
                    database.Close();
                    database.Dispose();
                    return null;
                }
            }
        }

        /// <summary>
        /// Realiza una consulta SELECT a la base de datos y retorna el primer objeto que coincida con los parámetros especificados.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto a retornar.</typeparam>
        /// <param name="sqlQuery">La consulta SQL.</param>
        /// <param name="parameters">Los parámetros de la consulta.</param>
        /// <returns>El primer objeto que coincida con los parámetros especificados.</returns>
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
                catch (Exception)
                {
                    database.Close();
                    database.Dispose();
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Realiza una consulta SELECT a la base de datos y retorna una lista desplegable.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto a retornar.</typeparam>
        /// <param name="sqlQuery">La consulta SQL.</param>
        /// <returns>Una lista desplegable de objetos del tipo especificado.</returns>
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
                catch (Exception)
                {
                    database.Close();
                    database.Dispose();
                    return null;
                }
            }
        }

        /// <summary>
        /// Realiza una actualización en la base de datos.
        /// </summary>
        /// <param name="sqlQuery">La consulta SQL.</param>
        /// <param name="parameters">Los parámetros de la consulta.</param>
        /// <returns>True si la actualización se realizó correctamente, False en caso contrario.</returns>
        public static async Task<bool> Update(string sqlQuery, DynamicParameters parameters)
        {
            bool resultSql;
            using (var database = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                database.Open();
                var result = await database.QueryAsync(sqlQuery, parameters, commandType: CommandType.StoredProcedure);
                if (result.Count() != 0)
                {
                    return true;
                }

                resultSql = false;

                database.Close();
                database.Dispose();
                return resultSql;
            }
        }

        /// <summary>
        /// Realiza una inserción en la base de datos.
        /// </summary>
        /// <param name="sqlQuery">La consulta SQL.</param>
        /// <param name="parameters">Los parámetros de la consulta.</param>
        /// <returns>True si la inserción se realizó correctamente, False en caso contrario.</returns>
        public static async Task<bool> Insert(string sqlQuery, DynamicParameters parameters)
        {
            bool resultSql;
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

        /// <summary>
        /// Busca un objeto en la base de datos.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto a buscar.</typeparam>
        /// <param name="sqlQuery">La consulta SQL.</param>
        /// <param name="parameters">Los parámetros de la consulta.</param>
        /// <returns>El objeto que coincide con los parámetros especificados.</returns>
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

        /// <summary>
        /// Obtiene el detalle de un objeto en la base de datos.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto a obtener el detalle.</typeparam>
        /// <param name="sqlQuery">La consulta SQL.</param>
        /// <param name="parameters">Los parámetros de la consulta.</param>
        /// <returns>El detalle del objeto.</returns>
        public static async Task<T> Detail<T>(string sqlQuery, DynamicParameters parameters)
        {
            return default(T);
        }

        /// <summary>
        /// Elimina un objeto de la base de datos.
        /// </summary>
        /// <param name="sqlQuery">La consulta SQL.</param>
        /// <param name="parameters">Los parámetros de la consulta.</param>
        /// <returns>True si la eliminación se realizó correctamente, False en caso contrario.</returns>
        public static async Task<Boolean> Delete(string sqlQuery, DynamicParameters parameters)
        {
            bool resultSql;
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
