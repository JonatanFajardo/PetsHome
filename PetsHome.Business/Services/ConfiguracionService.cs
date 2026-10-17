using PetsHome.Business.Models;
using PetsHome.Logic.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio del motor de modulos configurables (Refugio / Veterinaria)
    /// y de la configuracion global del sistema.
    /// </summary>
    public class ConfiguracionService
    {
        private readonly ConfiguracionRepository _configuracionRepository;

        public ConfiguracionService(ConfiguracionRepository configuracionRepository)
        {
            _configuracionRepository = configuracionRepository;
        }

        public async Task<IEnumerable<ModuloViewModel>> ListModulosAsync()
        {
            var result = await _configuracionRepository.ListModulosAsync();
            return result.Select(m => new ModuloViewModel
            {
                modu_Id = m.modu_Id,
                modu_Codigo = m.modu_Codigo,
                modu_Nombre = m.modu_Nombre,
                modu_Area = m.modu_Area,
                modu_EsNucleo = m.modu_EsNucleo,
                modu_EsActivo = m.modu_EsActivo,
                modu_RequiereModulos = m.modu_RequiereModulos,
                modu_Orden = m.modu_Orden
            });
        }

        /// <summary>
        /// Codigos de los modulos actualmente activos.
        /// </summary>
        public async Task<List<string>> GetModulosActivosAsync()
        {
            var result = await _configuracionRepository.ModulosActivosAsync();
            return result.Select(m => m.modu_Codigo).ToList();
        }

        /// <summary>
        /// Activa o desactiva un modulo (el nucleo queda protegido en el SP).
        /// </summary>
        public async Task<bool> SetModuloActivoAsync(string codigo, bool activo)
        {
            return await _configuracionRepository.SetModuloActivoAsync(codigo, activo);
        }

        public async Task<ConfiguracionSistemaViewModel> GetConfiguracionAsync()
        {
            var r = await _configuracionRepository.GetConfiguracionAsync();
            if (r == null) return null;
            return new ConfiguracionSistemaViewModel
            {
                cfg_Id = r.cfg_Id,
                cfg_PerfilPrincipal = r.cfg_PerfilPrincipal,
                cfg_WizardCompletado = r.cfg_WizardCompletado,
                cfg_NombreOrganizacion = r.cfg_NombreOrganizacion,
                cfg_FechaConfig = r.cfg_FechaConfig,
                cfg_FechaModifica = r.cfg_FechaModifica
            };
        }

        public async Task<bool> CompletarWizardAsync(string perfil, string nombreOrg, int usuarioConfig)
        {
            return await _configuracionRepository.CompletarWizardAsync(perfil, nombreOrg, usuarioConfig);
        }
    }
}
