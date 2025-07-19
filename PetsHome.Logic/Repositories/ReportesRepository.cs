using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio para la gestión de reportes del sistema PetsHome
    /// </summary>
    public class ReportesRepository
    {
        #region Dashboard Principal

        /// <summary>
        /// Obtiene las métricas principales para el dashboard de reportes
        /// </summary>
        /// <returns>Datos del dashboard</returns>
        public async Task<PR_Reportes_Dashboard_Result> GetDashboardAsync()
        {
            const string sqlQuery = "[Reportes].[PR_Reportes_Dashboard]";
            return await DbApp.Find<PR_Reportes_Dashboard_Result>(sqlQuery, null);
        }

        #endregion

        #region Reportes de Mascotas

        /// <summary>
        /// Obtiene el reporte de mascotas agrupadas por raza
        /// </summary>
        /// <param name="refugioId">ID del refugio (opcional)</param>
        /// <returns>Lista de mascotas por raza</returns>
        public async Task<IEnumerable<PR_Reportes_MascotasPorRaza_Result>> GetMascotasPorRazaAsync(int? refugioId = null)
        {
            const string sqlQuery = "[Reportes].[PR_Reportes_MascotasPorRaza]";
            var parameters = new DynamicParameters();
            parameters.Add("@refg_Id", refugioId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Reportes_MascotasPorRaza_Result>(sqlQuery, parameters);
        }

        /// <summary>
        /// Obtiene el reporte de salud de mascotas
        /// </summary>
        /// <param name="refugioId">ID del refugio (opcional)</param>
        /// <param name="soloProblematicas">Solo mascotas que requieren atención</param>
        /// <returns>Lista del estado de salud de mascotas</returns>
        public async Task<IEnumerable<PR_Reportes_SaludMascotas_Result>> GetSaludMascotasAsync(int? refugioId = null, bool soloProblematicas = false)
        {
            const string sqlQuery = "[Reportes].[PR_Reportes_SaludMascotas]";
            var parameters = new DynamicParameters();
            parameters.Add("@refg_Id", refugioId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@soloProblematicas", soloProblematicas, DbType.Boolean, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Reportes_SaludMascotas_Result>(sqlQuery, parameters);
        }

        #endregion

        #region Reportes de Adopciones

        /// <summary>
        /// Obtiene el reporte de adopciones agrupadas por mes
        /// </summary>
        /// <param name="mesesAtras">Número de meses hacia atrás a incluir</param>
        /// <param name="refugioId">ID del refugio (opcional)</param>
        /// <returns>Lista de adopciones por mes</returns>
        public async Task<IEnumerable<PR_Reportes_AdopcionesPorMes_Result>> GetAdopcionesPorMesAsync(int mesesAtras = 6, int? refugioId = null)
        {
            const string sqlQuery = "[Reportes].[PR_Reportes_AdopcionesPorMes]";
            var parameters = new DynamicParameters();
            parameters.Add("@mesesAtras", mesesAtras, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@refg_Id", refugioId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Reportes_AdopcionesPorMes_Result>(sqlQuery, parameters);
        }

        #endregion

        #region Reportes Médicos

        /// <summary>
        /// Obtiene el reporte de citas médicas agrupadas por tipo
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del período</param>
        /// <param name="fechaFin">Fecha de fin del período</param>
        /// <param name="refugioId">ID del refugio (opcional)</param>
        /// <returns>Lista de citas médicas por tipo</returns>
        public async Task<IEnumerable<PR_Reportes_CitasMedicasPorTipo_Result>> GetCitasMedicasPorTipoAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? refugioId = null)
        {
            const string sqlQuery = "[Reportes].[PR_Reportes_CitasMedicasPorTipo]";
            var parameters = new DynamicParameters();
            parameters.Add("@fechaInicio", fechaInicio, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@fechaFin", fechaFin, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@refg_Id", refugioId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Reportes_CitasMedicasPorTipo_Result>(sqlQuery, parameters);
        }

        #endregion

        #region Reportes de Voluntarios

        /// <summary>
        /// Obtiene el reporte de voluntarios con su participación en eventos
        /// </summary>
        /// <param name="soloActivos">Solo voluntarios activos</param>
        /// <param name="refugioId">ID del refugio (opcional)</param>
        /// <returns>Lista de voluntarios con estadísticas</returns>
        public async Task<IEnumerable<PR_Reportes_Voluntarios_Result>> GetVoluntariosAsync(bool soloActivos = false, int? refugioId = null)
        {
            const string sqlQuery = "[Reportes].[PR_Reportes_Voluntarios]";
            var parameters = new DynamicParameters();
            parameters.Add("@soloActivos", soloActivos, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@refg_Id", refugioId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Reportes_Voluntarios_Result>(sqlQuery, parameters);
        }

        #endregion

        #region Reportes de Inventario

        /// <summary>
        /// Obtiene el reporte de inventario con stock y valorización
        /// </summary>
        /// <param name="refugioId">ID del refugio (opcional)</param>
        /// <param name="soloCriticos">Solo items con stock crítico</param>
        /// <returns>Lista de items del inventario</returns>
        public async Task<IEnumerable<PR_Reportes_Inventario_Result>> GetInventarioAsync(int? refugioId = null, bool soloCriticos = false)
        {
            const string sqlQuery = "[Reportes].[PR_Reportes_Inventario]";
            var parameters = new DynamicParameters();
            parameters.Add("@refg_Id", refugioId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@soloCriticos", soloCriticos, DbType.Boolean, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Reportes_Inventario_Result>(sqlQuery, parameters);
        }

        #endregion

        #region Reportes de Eventos

        /// <summary>
        /// Obtiene el reporte de eventos del refugio
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del período</param>
        /// <param name="fechaFin">Fecha de fin del período</param>
        /// <param name="refugioId">ID del refugio (opcional)</param>
        /// <param name="soloFuturos">Solo eventos futuros</param>
        /// <returns>Lista de eventos con participantes</returns>
        public async Task<IEnumerable<PR_Reportes_Eventos_Result>> GetEventosAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? refugioId = null, bool soloFuturos = false)
        {
            const string sqlQuery = "[Reportes].[PR_Reportes_Eventos]";
            var parameters = new DynamicParameters();
            parameters.Add("@fechaInicio", fechaInicio, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@fechaFin", fechaFin, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@refg_Id", refugioId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@soloFuturos", soloFuturos, DbType.Boolean, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Reportes_Eventos_Result>(sqlQuery, parameters);
        }

        #endregion
    }
}