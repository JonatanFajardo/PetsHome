using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class EventoRepository
    {
        public async Task<IEnumerable<PR_Refugio_Eventos_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Eventos_List]";
            return await DbApp.Select<PR_Refugio_Eventos_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_Eventos_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Eventos_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@eve_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Eventos_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_Eventos_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Eventos_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@eve_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Eventos_DetailResult>(sqlQuery, parameter);
        }

        public async Task<RequestResult> AddAsync(tbEventos entity)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Eventos_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@eve_Descripcion", entity.eve_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@eve_HoraInicio", entity.eve_HoraInicio, DbType.Time, ParameterDirection.Input);
            parameter.Add("@eve_HoraFinal", entity.eve_HoraFinal, DbType.Time, ParameterDirection.Input);
            parameter.Add("@eve_Fecha", entity.eve_Fecha, DbType.Date, ParameterDirection.Input);
            parameter.Add("@eve_UsuarioCrea", entity.eve_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> EditAsync(tbEventos entity)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Eventos_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@eve_Id", entity.eve_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@eve_Descripcion", entity.eve_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@eve_HoraInicio", entity.eve_HoraInicio, DbType.Time, ParameterDirection.Input);
            parameter.Add("@eve_HoraFinal", entity.eve_HoraFinal, DbType.Time, ParameterDirection.Input);
            parameter.Add("@eve_Fecha", entity.eve_Fecha, DbType.Date, ParameterDirection.Input);
            parameter.Add("@eve_UsuarioModifica", entity.eve_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Eventos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@eve_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<IEnumerable<PR_Refugio_EventoVoluntarios_ListResult>> ListVoluntariosAsync(int eveId)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_EventoVoluntarios_List]";
            var parameter = new DynamicParameters();
            parameter.Add("@eve_Id", eveId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Refugio_EventoVoluntarios_ListResult>(sqlQuery, parameter);
        }

        public async Task<RequestResult> CambiarEstadoVoluntarioAsync(int evevolId, string estado)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_EventoVoluntarios_CambiarEstado]";
            var parameter = new DynamicParameters();
            parameter.Add("@evevol_Id",    evevolId, DbType.Int32,  ParameterDirection.Input);
            parameter.Add("@evevol_Estado", estado,  DbType.String, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<IEnumerable<PR_Refugio_Voluntarios_DisponiblesResult>> VoluntariosDisponiblesAsync(int eveId)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Voluntarios_Disponibles]";
            var parameter = new DynamicParameters();
            parameter.Add("@eve_Id", eveId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Refugio_Voluntarios_DisponiblesResult>(sqlQuery, parameter);
        }

        public async Task<RequestResult> AsignarVoluntarioAsync(int eveId, int volId)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_EventoVoluntarios_Asignar]";
            var parameter = new DynamicParameters();
            parameter.Add("@eve_Id", eveId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@vol_Id", volId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }
    }
}