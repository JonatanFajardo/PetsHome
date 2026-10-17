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
    public class ClientesRepository : IGenericRepository<tbClientes>
    {
        public async Task<IEnumerable<PR_Veterinaria_Clientes_ListResult>> ListAsync()
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Clientes_List]";
            return await DbApp.Select<PR_Veterinaria_Clientes_ListResult>(sql);
        }

        public async Task<PR_Veterinaria_Clientes_FindResult> FindAsync(int id)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Clientes_Find]";
            var p = new DynamicParameters();
            p.Add("@cli_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Veterinaria_Clientes_FindResult>(sql, p);
        }

        public async Task<PR_Veterinaria_Clientes_DetailResult> DetailAsync(int id)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Clientes_Detail]";
            var p = new DynamicParameters();
            p.Add("@cli_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Veterinaria_Clientes_DetailResult>(sql, p);
        }

        public async Task<RequestResult> AddAsync(tbClientes entity)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Clientes_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@cli_Identidad", entity.cli_Identidad, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_Nombres", entity.cli_Nombres, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_Apellidos", entity.cli_Apellidos, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_Telefono", entity.cli_Telefono, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_Correo", entity.cli_Correo, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_Direccion", entity.cli_Direccion, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_EsConsumidorFinal", entity.cli_EsConsumidorFinal, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@cli_UsuarioCrea", entity.cli_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, parameter);
        }

        public async Task<RequestResult> EditAsync(tbClientes entity)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Clientes_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@cli_Id", entity.cli_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cli_Identidad", entity.cli_Identidad, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_Nombres", entity.cli_Nombres, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_Apellidos", entity.cli_Apellidos, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_Telefono", entity.cli_Telefono, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_Correo", entity.cli_Correo, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_Direccion", entity.cli_Direccion, DbType.String, ParameterDirection.Input);
    parameter.Add("@cli_EsConsumidorFinal", entity.cli_EsConsumidorFinal, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@cli_UsuarioModifica", entity.cli_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, parameter);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Clientes_Delete]";
            var p = new DynamicParameters();
            p.Add("@cli_Id", id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cli_UsuarioModifica", 1, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }
    }
}
