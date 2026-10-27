using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio de padecimientos / crónicos de mascota ([Medico].[tbMascotaPadecimiento]).
    /// Operaciones: List por mascota / Insert / Corregir (soft-delete).
    /// </summary>
    public class MascotaPadecimientoRepository
    {
        public async Task<IEnumerable<PR_Medico_MascotaPadecimiento_ListResult>> ListAsync(int masc_Id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_MascotaPadecimiento_List]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", masc_Id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_MascotaPadecimiento_ListResult>(sqlQuery, p);
        }

        public async Task<RequestResult> AddAsync(tbMascotaPadecimiento entity)
        {
            const string sqlQuery = "[Medico].[PR_Medico_MascotaPadecimiento_Insert]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@pade_Nombre", entity.pade_Nombre, DbType.String, ParameterDirection.Input);
            p.Add("@pade_Estado", entity.pade_Estado, DbType.String, ParameterDirection.Input);
            p.Add("@pade_Desde", entity.pade_Desde, DbType.Date, ParameterDirection.Input);
            p.Add("@pade_Hasta", entity.pade_Hasta, DbType.Date, ParameterDirection.Input);
            p.Add("@pade_UsuarioCrea", entity.pade_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, p);
        }

        public async Task<RequestResult> CorregirAsync(int pade_Id, int usuarioModifica)
        {
            const string sqlQuery = "[Medico].[PR_Medico_MascotaPadecimiento_Corregir]";
            var p = new DynamicParameters();
            p.Add("@pade_Id", pade_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@pade_UsuarioModifica", usuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, p);
        }
    }
}
