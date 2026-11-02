using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class ProveedorRepository : IGenericRepository<tbProveedores>
    {
        public async Task<IEnumerable<PR_Inventario_Proveedores_ListResult>> ListAsync()
        {
            const string sql = "[Inventario].[PR_Inventario_Proveedores_List]";
            return await DbApp.Select<PR_Inventario_Proveedores_ListResult>(sql);
        }

        public async Task<IEnumerable<PR_Inventario_Proveedores_DropdownResult>> DropdownAsync()
        {
            const string sql = "[Inventario].[PR_Inventario_Proveedores_Dropdown]";
            return await DbApp.Select<PR_Inventario_Proveedores_DropdownResult>(sql);
        }

        public async Task<PR_Inventario_Proveedores_FindResult> FindAsync(int id)
        {
            const string sql = "[Inventario].[PR_Inventario_Proveedores_Find]";
            var p = new DynamicParameters();
            p.Add("@prov_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Inventario_Proveedores_FindResult>(sql, p);
        }

        public async Task<PR_Inventario_Proveedores_DetailResult> DetailAsync(int id)
        {
            const string sql = "[Inventario].[PR_Inventario_Proveedores_Detail]";
            var p = new DynamicParameters();
            p.Add("@prov_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Inventario_Proveedores_DetailResult>(sql, p);
        }

        public async Task<RequestResult> AddAsync(tbProveedores entity)
        {
            const string sql = "[Inventario].[PR_Inventario_Proveedores_Insert]";
            var p = new DynamicParameters();
            p.Add("@prov_Nombre", entity.prov_Nombre, DbType.String, ParameterDirection.Input);
            p.Add("@prov_Email", entity.prov_Email, DbType.String, ParameterDirection.Input);
            p.Add("@prov_Telefono", entity.prov_Telefono, DbType.String, ParameterDirection.Input);
            p.Add("@prov_Direccion", entity.prov_Direccion, DbType.String, ParameterDirection.Input);
            p.Add("@prov_Notas", entity.prov_Notas, DbType.String, ParameterDirection.Input);
            p.Add("@prov_EsActivo", entity.prov_EsActivo, DbType.Boolean, ParameterDirection.Input);
            p.Add("@prov_UsuarioCrea", entity.prov_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> EditAsync(tbProveedores entity)
        {
            const string sql = "[Inventario].[PR_Inventario_Proveedores_Update]";
            var p = new DynamicParameters();
            p.Add("@prov_Id", entity.prov_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@prov_Nombre", entity.prov_Nombre, DbType.String, ParameterDirection.Input);
            p.Add("@prov_Email", entity.prov_Email, DbType.String, ParameterDirection.Input);
            p.Add("@prov_Telefono", entity.prov_Telefono, DbType.String, ParameterDirection.Input);
            p.Add("@prov_Direccion", entity.prov_Direccion, DbType.String, ParameterDirection.Input);
            p.Add("@prov_Notas", entity.prov_Notas, DbType.String, ParameterDirection.Input);
            p.Add("@prov_EsActivo", entity.prov_EsActivo, DbType.Boolean, ParameterDirection.Input);
            p.Add("@prov_UsuarioModifica", entity.prov_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sql = "[Inventario].[PR_Inventario_Proveedores_Delete]";
            var p = new DynamicParameters();
            p.Add("@prov_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<bool> NombreExistsAsync(string nombre, int id)
        {
            const string sql = "[Inventario].[PR_Inventario_Proveedores_Existe]";
            var p = new DynamicParameters();
            p.Add("@prov_Nombre", nombre, DbType.String, ParameterDirection.Input);
            p.Add("@prov_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<bool>(sql, p);
        }
    }
}
