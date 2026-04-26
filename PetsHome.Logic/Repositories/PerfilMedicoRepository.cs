        using PetsHome.Common.Entities;
        using PetsHome.DataAccess.Extensions;
        using System.Collections.Generic;
        using System.Threading.Tasks;
using Dapper;
using System.Data;

        namespace PetsHome.Logic.Repositories
        {
            public class PerfilMedicoRepository
            {
            public async Task<IEnumerable<PR_Medico_PerfilMedico_FichaMascotaResult>> FichaMascotaAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_PerfilMedico_FichaMascota]";
            var p = new DynamicParameters();
p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.Select<PR_Medico_PerfilMedico_FichaMascotaResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_PerfilMedico_UltimasCitasResult>> UltimasCitasAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_PerfilMedico_UltimasCitas]";
            var p = new DynamicParameters();
p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.Select<PR_Medico_PerfilMedico_UltimasCitasResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_PerfilMedico_MedicamentosActivosResult>> MedicamentosActivosAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_PerfilMedico_MedicamentosActivos]";
            var p = new DynamicParameters();
p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.Select<PR_Medico_PerfilMedico_MedicamentosActivosResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_PerfilMedico_TodasCitasResult>> TodasCitasAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_PerfilMedico_TodasCitas]";
            var p = new DynamicParameters();
p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.Select<PR_Medico_PerfilMedico_TodasCitasResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_PerfilMedico_TratamientosResult>> TratamientosAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_PerfilMedico_Tratamientos]";
            var p = new DynamicParameters();
p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.Select<PR_Medico_PerfilMedico_TratamientosResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_PerfilMedico_VacunasResult>> VacunasAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_PerfilMedico_Vacunas]";
            var p = new DynamicParameters();
p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.Select<PR_Medico_PerfilMedico_VacunasResult>(sql, p);
    }
            }
        }
