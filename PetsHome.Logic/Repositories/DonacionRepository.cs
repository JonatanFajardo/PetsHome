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
    public class DonacionRepository : IGenericRepository<tbDonaciones>
    {
        public async Task<IEnumerable<PR_Refugio_Donaciones_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Donaciones_List]";
            return await DbApp.Select<PR_Refugio_Donaciones_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_Donaciones_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Donaciones_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@dona_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Donaciones_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_Donaciones_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Donaciones_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@dona_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Donaciones_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbDonaciones entity)
        {
            entity.dona_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Donaciones_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@dona_TipoDonacion", entity.dona_TipoDonacion, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_NombreDonante", entity.dona_NombreDonante, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_TelefonoDonante", entity.dona_TelefonoDonante, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_EmailDonante", entity.dona_EmailDonante, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_MontoMonetario", entity.dona_MontoMonetario, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@dona_DescripcionArticulos", entity.dona_DescripcionArticulos, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_ValorEstimado", entity.dona_ValorEstimado, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@dona_FechaDonacion", entity.dona_FechaDonacion, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@dona_Estado", entity.dona_Estado, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_Observaciones", entity.dona_Observaciones, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@dona_UsuarioCrea", entity.dona_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbDonaciones entity)
        {
            entity.dona_UsuarioModifica = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Donaciones_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@dona_Id", entity.dona_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@dona_TipoDonacion", entity.dona_TipoDonacion, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_NombreDonante", entity.dona_NombreDonante, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_TelefonoDonante", entity.dona_TelefonoDonante, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_EmailDonante", entity.dona_EmailDonante, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_MontoMonetario", entity.dona_MontoMonetario, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@dona_DescripcionArticulos", entity.dona_DescripcionArticulos, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_ValorEstimado", entity.dona_ValorEstimado, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@dona_FechaDonacion", entity.dona_FechaDonacion, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@dona_Estado", entity.dona_Estado, DbType.String, ParameterDirection.Input);
            parameter.Add("@dona_Observaciones", entity.dona_Observaciones, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@dona_UsuarioModifica", entity.dona_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Donaciones_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@dona_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}