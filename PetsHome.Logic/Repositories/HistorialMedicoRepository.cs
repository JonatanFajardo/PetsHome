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
    public class HistorialMedicoRepository : IGenericRepository<tbHistorialMedico>
    {
        public async Task<IEnumerable<PR_Refugio_HistorialMedico_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_List]";
            return await DbApp.Select<PR_Refugio_HistorialMedico_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_HistorialMedico_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@medic_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_HistorialMedico_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_HistorialMedico_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@medic_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_HistorialMedico_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbHistorialMedico entity)
        {
            entity.medic_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_Esterilizacion", entity.medic_Esterilizacion, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@medic_Comportamiento", entity.medic_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_SaludCuidado", entity.medic_SaludCuidado, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_InformacionAdicional", entity.medic_InformacionAdicional, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_UsuarioCrea", entity.medic_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbHistorialMedico entity)
        {
            entity.medic_UsuarioModifica = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@medic_Id", entity.medic_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_Esterilizacion", entity.medic_Esterilizacion, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@medic_Comportamiento", entity.medic_Id, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_SaludCuidado", entity.medic_SaludCuidado, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_InformacionAdicional", entity.medic_InformacionAdicional, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_UsuarioModifica", entity.medic_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@medic_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}