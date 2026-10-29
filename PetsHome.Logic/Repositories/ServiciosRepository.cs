using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class ServiciosRepository : IGenericRepository<tbServicios>
    {
        public async Task<IEnumerable<PR_Veterinaria_Servicios_ListResult>> ListAsync()
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Servicios_List]";
            return await DbApp.Select<PR_Veterinaria_Servicios_ListResult>(sql);
        }

        /// <summary>
        /// Picker de servicios facturables por categoria (Vacuna/Laboratorio/Procedimiento/Consulta).
        /// Solo activos y no eliminados. Si categoria es null devuelve todas.
        /// </summary>
        public async Task<IEnumerable<PR_Veterinaria_Servicio_DropdownResult>> ServicioDropdownAsync(string categoria = null)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Servicio_Dropdown]";
            var p = new DynamicParameters();
            p.Add("@categoria", string.IsNullOrWhiteSpace(categoria) ? null : categoria.Trim(), DbType.String, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Veterinaria_Servicio_DropdownResult>(sql, p);
        }

        public async Task<PR_Veterinaria_Servicios_FindResult> FindAsync(int id)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Servicios_Find]";
            var p = new DynamicParameters();
            p.Add("@serv_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Veterinaria_Servicios_FindResult>(sql, p);
        }

        public async Task<PR_Veterinaria_Servicios_DetailResult> DetailAsync(int id)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Servicios_Detail]";
            var p = new DynamicParameters();
            p.Add("@serv_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Veterinaria_Servicios_DetailResult>(sql, p);
        }

        public async Task<RequestResult> AddAsync(tbServicios entity)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Servicios_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@serv_Codigo", entity.serv_Codigo, DbType.String, ParameterDirection.Input);
    parameter.Add("@serv_Nombre", entity.serv_Nombre, DbType.String, ParameterDirection.Input);
    parameter.Add("@serv_Descripcion", entity.serv_Descripcion, DbType.String, ParameterDirection.Input);
    parameter.Add("@serv_Precio", entity.serv_Precio, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@serv_UsuarioCrea", entity.serv_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, parameter);
        }

        public async Task<RequestResult> EditAsync(tbServicios entity)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Servicios_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@serv_Id", entity.serv_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@serv_Codigo", entity.serv_Codigo, DbType.String, ParameterDirection.Input);
    parameter.Add("@serv_Nombre", entity.serv_Nombre, DbType.String, ParameterDirection.Input);
    parameter.Add("@serv_Descripcion", entity.serv_Descripcion, DbType.String, ParameterDirection.Input);
    parameter.Add("@serv_Precio", entity.serv_Precio, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@serv_UsuarioModifica", entity.serv_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, parameter);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Servicios_Delete]";
            var p = new DynamicParameters();
            p.Add("@serv_Id", id, DbType.Int32, ParameterDirection.Input);
            p.Add("@serv_UsuarioModifica", 1, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }
    }
}
