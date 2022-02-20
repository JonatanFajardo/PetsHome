using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class EmpleadoRepository
    {
        #region Consultas

        public async Task<IEnumerable<PR_Refugio_Empleados_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Empleados_List]";
            return await DbApp.Select<PR_Refugio_Empleados_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_Empleados_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Empleados_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Empleados_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_Empleados_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Empleados_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Empleados_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbEmpleados entity)
        {
            entity.per.per_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Empleados_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@emp_Codigo", entity.emp_Codigo, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.String, ParameterDirection.Input);
            parameter.Add("@cag_Id", entity.cag_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@per_Identidad", entity.per.per_Identidad, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@per_PrimerNombre", entity.per.per_PrimerNombre, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_SegundoNombre", entity.per.per_SegundoNombre, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_ApellidoPaterno", entity.per.per_ApellidoPaterno, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_ApellidoMaterno", entity.per.per_ApellidoMaterno, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_FechaNacimiento", entity.per.per_FechaNacimiento, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_CorreoElectronico", entity.per.per_Correo, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_Telefono", entity.per.per_Telefono, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_Direccion", entity.per.per_Domicilio, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_UsuarioRegistra", entity.per.per_UsuarioCrea, DbType.Double, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbEmpleados entity)
        {
            entity.per.per_UsuarioModifica = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Empleados_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@emp_Id", entity.emp_Id, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.String, ParameterDirection.Input);
            parameter.Add("@cag_Id", entity.cag_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@emp_EsActivo", entity.emp_EsActivo, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@per_Id", entity.per.per_Id, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_Identidad", entity.per.per_Identidad, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_PrimerNombre", entity.per.per_PrimerNombre, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_SegundoNombre", entity.per.per_SegundoNombre, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_ApellidoPaterno", entity.per.per_ApellidoPaterno, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_ApellidoMaterno", entity.per.per_ApellidoMaterno, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_FechaNacimiento", entity.per.per_FechaNacimiento, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_CorreoElectronico", entity.per.per_Correo, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_Direccion", entity.per.per_Domicilio, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_Telefono", entity.per.per_Telefono, DbType.Double, ParameterDirection.Input);
            parameter.Add("@per_UsuarioModifica", entity.per.per_UsuarioModifica, DbType.Double, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Empleados_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
        #endregion Consultas
    }
}
