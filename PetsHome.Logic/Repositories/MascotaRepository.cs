using Dapper;
using Microsoft.Data.SqlClient;
using PetsHome.Common.Entities;
using PetsHome.DataAccess;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class MascotaRepository : IGenericRepository<tbMascotas>
    {
        #region Consultas

        public async Task<IEnumerable<PR_Refugio_Mascotas_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Mascotas_List]";
            return await DbApp.Select<PR_Refugio_Mascotas_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_Mascotas_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Mascotas_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Mascotas_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_Mascotas_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Mascotas_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Mascotas_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbMascotas entity)
        {
            entity.masc_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Mascotas_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Imagen", entity.masc_Imagen, DbType.Binary, ParameterDirection.Input);
            parameter.Add("@masc_Nombre", entity.masc_Nombre, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_Id", entity.raza_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Edad", entity.masc_Edad, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Sexo", entity.masc_Sexo, DbType.String, ParameterDirection.Input);
            parameter.Add("@masc_Peso", entity.masc_Peso, DbType.Double, ParameterDirection.Input);
            parameter.Add("@masc_Color", entity.masc_Color, DbType.String, ParameterDirection.Input);
            parameter.Add("@masc_Historia", entity.masc_Historia, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@proc_Id", entity.proc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_UsuarioCrea", entity.masc_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbMascotas entity)
        {
            entity.masc_UsuarioModifica = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Mascotas_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Imagen", entity.masc_Imagen, DbType.Binary, ParameterDirection.Input);
            parameter.Add("@masc_Nombre", entity.masc_Nombre, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_Id", entity.raza_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Edad", entity.masc_Edad, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Sexo", entity.masc_Sexo, DbType.String, ParameterDirection.Input);
            parameter.Add("@masc_Peso", entity.masc_Peso, DbType.Double, ParameterDirection.Input);
            parameter.Add("@masc_Color", entity.masc_Color, DbType.String, ParameterDirection.Input);
            parameter.Add("@masc_Historia", entity.masc_Historia, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@proc_Id", entity.proc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_EsAdoptado", entity.masc_EsAdoptado, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_EsReservado", entity.masc_EsReservado, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_UsuarioModifica", entity.masc_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "PR_Refugio_Mascotas_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
        #endregion Consultas

        #region Dropdown
        public IEnumerable<PR_Refugio_Raza_DropdownResult> RazaDropdown()
        {
            const string query = "[Refugio].[PR_Refugio_Raza_Dropdown]";
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var result = db.Query<PR_Refugio_Raza_DropdownResult>(query, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        //public IEnumerable<PR_Refugio_Mascotas_DropdownResult> MascotasDropdown()
        //{
        //    const string query = "[Refugio].[PR_Refugio_Mascotas_Dropdown]";
        //    using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
        //    {
        //        var result = db.Query<PR_Refugio_Mascotas_DropdownResult>(query, commandType: CommandType.StoredProcedure);
        //        return result;
        //    }
        //}

        public IEnumerable<PR_Refugio_Procedencia_DropdownResult> ProcedenciaDropdown()
        {
            const string query = "[Refugio].[PR_Refugio_Procedencia_Dropdown]";
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var result = db.Query<PR_Refugio_Procedencia_DropdownResult>(query, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        #endregion Dropdown

    }
}
