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
    public class IngresoRepository : IGenericRepository<tbIngresos>
    {
        public async Task<IEnumerable<PR_Rescate_Ingresos_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_Ingresos_List]";
            return await DbApp.Select<PR_Rescate_Ingresos_ListResult>(sqlQuery);
        }

        public async Task<PR_Rescate_Ingresos_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_Ingresos_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@ingr_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Rescate_Ingresos_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Rescate_Ingresos_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_Ingresos_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@ingr_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Rescate_Ingresos_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbIngresos entity)
        {
            entity.ingr_UsuarioCrea = 1;
            const string sqlQuery = "[Rescate].[PR_Rescate_Ingresos_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@repa_Id", entity.repa_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@ingr_FechaIngreso", entity.ingr_FechaIngreso, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@ingr_LugarRescate", entity.ingr_LugarRescate, DbType.String, ParameterDirection.Input);
            parameter.Add("@ingr_CondicionInicial", entity.ingr_CondicionInicial, DbType.String, ParameterDirection.Input);
            parameter.Add("@ingr_PersonaRescatista", entity.ingr_PersonaRescatista, DbType.String, ParameterDirection.Input);
            parameter.Add("@ingr_MedioTransporte", entity.ingr_MedioTransporte, DbType.String, ParameterDirection.Input);
            parameter.Add("@ingr_Observaciones", entity.ingr_Observaciones, DbType.String, ParameterDirection.Input);
            parameter.Add("@ingr_EsEmergencia", entity.ingr_EsEmergencia, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@ingr_UsuarioCrea", entity.ingr_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbIngresos entity)
        {
            entity.ingr_UsuarioModifica = 1;
            const string sqlQuery = "[Rescate].[PR_Rescate_Ingresos_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@ingr_Id", entity.ingr_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@repa_Id", entity.repa_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@ingr_FechaIngreso", entity.ingr_FechaIngreso, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@ingr_LugarRescate", entity.ingr_LugarRescate, DbType.String, ParameterDirection.Input);
            parameter.Add("@ingr_CondicionInicial", entity.ingr_CondicionInicial, DbType.String, ParameterDirection.Input);
            parameter.Add("@ingr_PersonaRescatista", entity.ingr_PersonaRescatista, DbType.String, ParameterDirection.Input);
            parameter.Add("@ingr_MedioTransporte", entity.ingr_MedioTransporte, DbType.String, ParameterDirection.Input);
            parameter.Add("@ingr_Observaciones", entity.ingr_Observaciones, DbType.String, ParameterDirection.Input);
            parameter.Add("@ingr_EsEmergencia", entity.ingr_EsEmergencia, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@ingr_UsuarioModifica", entity.ingr_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_Ingresos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@ingr_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        public async Task<IEnumerable<tbIngresos>> DropdownAsync()
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_Ingresos_List]";
            return await DbApp.Select<tbIngresos>(sqlQuery);
        }
    }
}
