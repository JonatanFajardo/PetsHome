using Dapper;
using Microsoft.Data.SqlClient;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class MunicipioRepository
    {


        public async Task<IEnumerable<PR_General_Municipios_ListResult>> ListAsync()
        {
            const string sqlQuery = "[General].[PR_General_Municipios_List]";
            var parameter = new DynamicParameters();
            return await DbApp.Select<PR_General_Municipios_ListResult>(sqlQuery);
        }

        public async Task<Boolean> AddAsync(tbMunicipios entity)
        {
            int result = 0;
            const string sqlQuery = "[General].[PR_General_Municipios_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@mpio_Codigo", entity.mpio_Codigo, DbType.String, ParameterDirection.Input);
            parameter.Add("@mpio_Descripcion", entity.mpio_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@depto_Id", entity.depto_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@mpio_UsuarioCrea", entity.mpio_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);

        }

        public async Task<PR_General_Municipios_FindResult> FindAsync(int id)
        {
            const string sqlQuery = @"[General].[PR_General_Municipios_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@depto_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<PR_General_Municipios_FindResult>(sqlQuery, parameter);
        }
        
        public async Task<PR_General_Municipios_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Municipios_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@depto_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_General_Municipios_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbMunicipios entity)
        {
            int result = 0;
            const string sqlQuery = "[General].[PR_General_Municipios_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@mpio_Id", entity.mpio_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@mpio_Codigo", entity.mpio_Codigo, DbType.String, ParameterDirection.Input);
            parameter.Add("@mpio_Descripcion", entity.mpio_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@mpio_UsuarioModifica", entity.mpio_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }



        public async Task<Boolean> RemoveAsync(int id)
        {
            int result = 0;
            const string sqlQuery = "[General].[PR_General_Municipios_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@mpio_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public IEnumerable<tbPersonas> ValidationPersonas(int mpio_Id)
        {
            //const string query = @"[General].[PR_General_tbMunicipios_DependencyPersonas]";
            //var parameters = new DynamicParameters();
            //parameters.Add("@mpio_Id", mpio_Id, DbType.Int32, ParameterDirection.Input);
            //using (var db = new SqlConnection(InstaHelpDbContext.ConnectionString))
            //{
            //    var result = db.Query<tbPersonas>(query, parameters, commandType: CommandType.StoredProcedure).ToList();
            //    return result;
            //}
            throw new NotImplementedException();
        }

        #region Dropdown
        public IEnumerable<PR_General_Municipios_DropdownResult> MunicipioDropdown()
        {
            const string sqlQuery = "[General].[PR_General_Municipios_Dropdown]";
            return DbApp.Dropdown<PR_General_Municipios_DropdownResult>(sqlQuery);
        }

        #endregion Dropdown

    }
}
