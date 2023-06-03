using PetsHome.Common.InternalEntities;
using System.Collections.Generic;

namespace PetsHome.Business.Data
{
    /// <summary>
    /// Clase que contiene los datos de los dropdownlist
    /// </summary>
    public class Dropdownlist
    {
        /// <summary>
        /// Conjunto de datos de tipo sexo.
        /// </summary>
        /// <returns>listado de datos </returns>
        public static IEnumerable<Dropdown> LoadSexo()
        {
            List<Dropdown> lista = new List<Dropdown>() {
                new Dropdown(){Value = 'H', Text = "Hombre"},
                new Dropdown(){Value = 'M', Text= "Mujer"}
            };
            return lista;
        }
    }
}