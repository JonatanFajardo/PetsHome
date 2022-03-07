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
    public class LocalidadRepository : IGenericRepository<tbDepartamentos>
    {
        #region Consultas
        public async Task<IEnumerable<PR_General_Departamentos_ListResult>> ListAsync()
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_List]";
            return await DbApp.Select<PR_General_Departamentos_ListResult>(sqlQuery);
        }
        public async Task<PR_General_Departamentos_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@depto_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<PR_General_Departamentos_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_General_Departamentos_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@depto_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_General_Departamentos_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbDepartamentos entity)
        {
            entity.depto_UsuarioCrea = 1;
            const string sqlQuery = "[General].[PR_General_Departamentos_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@depto_Codigo", entity.depto_Codigo, DbType.String, ParameterDirection.Input);
            parameter.Add("@depto_Descripcion", entity.depto_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@depto_UsuarioCrea", entity.depto_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbDepartamentos entity)
        {
            entity.depto_UsuarioModifica = 1;
            const string sqlQuery = "[General].[PR_General_Departamentos_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@depto_Id", entity.depto_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@depto_Codigo", entity.depto_Codigo, DbType.String, ParameterDirection.Input);
            parameter.Add("@depto_Descripcion", entity.depto_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@depto_UsuarioModifica", entity.depto_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@depto_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        #endregion Consultas
       
       

        //public async Task<PR_General_Departamentos_DetailResult> DetailAsync(int id)
        //{
        //    const string sqlQuery = "[General].[PR_General_Departamentos_Detail]";
        //    var parameter = new DynamicParameters();
        //    parameter.Add("@depto_Id", id, DbType.Int32, ParameterDirection.Input);
        //    return await DbApp.Detail<PR_General_Departamentos_DetailResult>(sqlQuery, parameter);
        //}

        #region Dropdown
        public IEnumerable<PR_General_Departamentos_DropdownResult> DepartamentoDropdown()
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_Dropdown]";
            return DbApp.Dropdown<PR_General_Departamentos_DropdownResult>(sqlQuery);
        }

        #endregion Dropdown
    }
}
