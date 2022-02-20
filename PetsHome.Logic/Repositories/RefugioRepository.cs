using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class RefugioRepository : IGenericRepository<tbRefugios>
    {
        public async Task<IEnumerable<PR_Refugio_Refugios_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Refugios_List]";
            return await DbApp.Select<PR_Refugio_Refugios_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_Refugios_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Refugios_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@refg", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Refugios_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_Refugios_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Refugios_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@refg", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Refugios_DetailResult>(sqlQuery, parameter);
        }


        public async Task<Boolean> AddAsync(tbRefugios entity)
        {
            entity.refg_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Refugios_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@refg_Nombre", entity.refg_Nombre, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Ubicacion", entity.refg_Ubicacion, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_RTN", entity.refg_RTN, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Telefono", entity.refg_Telefono, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Correo", entity.refg_Correo, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_InformacionAdicional", entity.refg_InformacionAdicional, DbType.String, ParameterDirection.Input);
            parameter.Add("@depto_Id", entity.depto_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@mpio_Id", entity.mpio_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@refg_UsuarioCrea", entity.refg_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbRefugios entity)
        {
            entity.refg_UsuarioModifica = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Refugios_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@refg_Nombre", entity.refg_Nombre, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Ubicacion", entity.refg_Ubicacion, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_RTN", entity.refg_RTN, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Telefono", entity.refg_Telefono, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Correo", entity.refg_Correo, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_InformacionAdicional", entity.refg_InformacionAdicional, DbType.String, ParameterDirection.Input);
            parameter.Add("@depto_Id", entity.depto_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@mpio_Id", entity.mpio_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@refg_UsuarioModifica", entity.refg_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Refugios_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@refg", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}
