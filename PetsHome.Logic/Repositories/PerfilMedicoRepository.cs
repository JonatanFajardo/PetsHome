        using PetsHome.Common.Entities;
        using PetsHome.DataAccess.Extensions;
        using System.Collections.Generic;
        using System.Linq;
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
        return await DbApp.SelectById<PR_Medico_PerfilMedico_FichaMascotaResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_PerfilMedico_UltimasCitasResult>> UltimasCitasAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_PerfilMedico_UltimasCitas]";
        var p = new DynamicParameters();
        p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.SelectById<PR_Medico_PerfilMedico_UltimasCitasResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_PerfilMedico_MedicamentosActivosResult>> MedicamentosActivosAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_PerfilMedico_MedicamentosActivos]";
        var p = new DynamicParameters();
        p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.SelectById<PR_Medico_PerfilMedico_MedicamentosActivosResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_PerfilMedico_TodasCitasResult>> TodasCitasAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_PerfilMedico_TodasCitas]";
        var p = new DynamicParameters();
        p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.SelectById<PR_Medico_PerfilMedico_TodasCitasResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_PerfilMedico_TratamientosResult>> TratamientosAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_PerfilMedico_Tratamientos]";
        var p = new DynamicParameters();
        p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.SelectById<PR_Medico_PerfilMedico_TratamientosResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_PerfilMedico_VacunasResult>> VacunasAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_PerfilMedico_Vacunas]";
        var p = new DynamicParameters();
        p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.SelectById<PR_Medico_PerfilMedico_VacunasResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_Vacuna_HistorialPorMascotaResult>> HistorialVacunasAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_Vacuna_HistorialPorMascota]";
        var p = new DynamicParameters();
        p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.SelectById<PR_Medico_Vacuna_HistorialPorMascotaResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_AlertaMedica_VacunasProximasResult>> VacunasProximasAsync(int? mascId)
    {
        const string sql = "[Medico].[PR_Medico_AlertaMedica_VacunasProximas]";
        var p = new DynamicParameters();
        p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.SelectById<PR_Medico_AlertaMedica_VacunasProximasResult>(sql, p);
    }

    // ── Pestanas nuevas vs OpenVPM (script 88) ───────────────────
    public async Task<IEnumerable<PR_Veterinaria_Facturas_PorMascotaResult>> FacturasPorMascotaAsync(int mascId)
    {
        const string sql = "[Veterinaria].[PR_Veterinaria_Facturas_PorMascota]";
        var p = new DynamicParameters();
        p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.SelectById<PR_Veterinaria_Facturas_PorMascotaResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_CitaMedica_VitalesPorMascotaResult>> VitalesPorMascotaAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_CitaMedica_VitalesPorMascota]";
        var p = new DynamicParameters();
        p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.SelectById<PR_Medico_CitaMedica_VitalesPorMascotaResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_CitaMedica_PesoPorMascotaResult>> PesoPorMascotaAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_CitaMedica_PesoPorMascota]";
        var p = new DynamicParameters();
        p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.SelectById<PR_Medico_CitaMedica_PesoPorMascotaResult>(sql, p);
    }

    public async Task<IEnumerable<PR_Medico_Laboratorio_PorMascotaResult>> LaboratorioPorMascotaAsync(int mascId)
    {
        const string sql = "[Medico].[PR_Medico_Laboratorio_PorMascota]";
        var p = new DynamicParameters();
        p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
        return await DbApp.SelectById<PR_Medico_Laboratorio_PorMascotaResult>(sql, p);
    }

    public async Task<int> RandomMascIdAsync()
    {
        const string sql = "SELECT TOP 1 masc_Id FROM [Refugio].[tbMascotas] WHERE masc_EsEliminado = 0 ORDER BY NEWID()";
        var result = await DbApp.Select<int>(sql);
        return result?.FirstOrDefault() ?? 1;
    }
            }
        }
