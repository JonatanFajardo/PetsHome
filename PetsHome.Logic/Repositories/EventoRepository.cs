using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System;
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


        public async Task<Boolean> AddAsync(tbEventos entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Boolean> EditAsync(tbEventos entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Eventos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@eve_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}
