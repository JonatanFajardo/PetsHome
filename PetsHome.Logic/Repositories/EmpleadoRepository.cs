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
            parameter.Add("@emp_Id", id, DbType.Int32, ParameterDirection.Input);
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
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cag_Id", entity.cag_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@emp_EsActivo", entity.emp_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@per_Identidad", entity.per.per_Identidad, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_PrimerNombre", entity.per.per_PrimerNombre, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_SegundoNombre", entity.per.per_SegundoNombre, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_ApellidoPaterno", entity.per.per_ApellidoPaterno, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_ApellidoMaterno", entity.per.per_ApellidoMaterno, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_FechaNacimiento", entity.per.per_FechaNacimiento, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@per_Domicilio", entity.per.per_Domicilio, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_Telefono", entity.per.per_Telefono, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_Correo", entity.per.per_Correo, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_UsuarioCrea", entity.per.per_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbEmpleados entity)
        {
            entity.per.per_UsuarioModifica = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Empleados_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@emp_Id", entity.emp_Id, DbType.String, ParameterDirection.Input);
            parameter.Add("@emp_Codigo", entity.emp_Codigo, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_Id", entity.per.per_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cag_Id", entity.cag_Id, DbType.String, ParameterDirection.Input);
            parameter.Add("@emp_EsActivo", entity.emp_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@per_Identidad", entity.per.per_Identidad, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_PrimerNombre", entity.per.per_PrimerNombre, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_SegundoNombre", entity.per.per_SegundoNombre, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_ApellidoPaterno", entity.per.per_ApellidoPaterno, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_ApellidoMaterno", entity.per.per_ApellidoMaterno, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_FechaNacimiento", entity.per.per_FechaNacimiento, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@per_Domicilio", entity.per.per_Domicilio, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_Telefono", entity.per.per_Telefono, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_Correo", entity.per.per_Correo, DbType.String, ParameterDirection.Input);
            parameter.Add("@per_UsuarioModifica", entity.per.per_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
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

        #region Dropdown
        public IEnumerable<PR_Refugio_Refugio_DropdownResult> RefugioDropdown()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Refugio_Dropdown]";
            return DbApp.Dropdown<PR_Refugio_Refugio_DropdownResult>(sqlQuery);
        }
        public IEnumerable<PR_Refugio_EmpleadosCargos_DropdownResult> EmpleadoCargoDropdown()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_EmpleadosCargos_Dropdown]";
            return DbApp.Dropdown<PR_Refugio_EmpleadosCargos_DropdownResult>(sqlQuery);
        }

        #endregion Dropdown

    }
}
