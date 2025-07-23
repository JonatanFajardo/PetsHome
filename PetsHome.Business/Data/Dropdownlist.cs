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

        /// <summary>
        /// Conjunto de datos de tipos de recepción.
        /// </summary>
        /// <returns>listado de tipos de recepción</returns>
        public static IEnumerable<Dropdown> LoadTipoRecepcion()
        {
            List<Dropdown> lista = new List<Dropdown>() {
                new Dropdown(){Value = "Compra", Text = "Compra"},
                new Dropdown(){Value = "Donación", Text = "Donación"},
                new Dropdown(){Value = "Transferencia", Text = "Transferencia"},
                new Dropdown(){Value = "Devolución", Text = "Devolución"},
                new Dropdown(){Value = "Otro", Text = "Otro"}
            };
            return lista;
        }

        /// <summary>
        /// Conjunto de datos de tipos de salida.
        /// </summary>
        /// <returns>listado de tipos de salida</returns>
        public static IEnumerable<Dropdown> LoadTipoSalida()
        {
            List<Dropdown> lista = new List<Dropdown>() {
                new Dropdown(){Value = "Consumo", Text = "Consumo"},
                new Dropdown(){Value = "Donación", Text = "Donación"},
                new Dropdown(){Value = "Transferencia", Text = "Transferencia"},
                new Dropdown(){Value = "Pérdida", Text = "Pérdida"},
                new Dropdown(){Value = "Vencimiento", Text = "Vencimiento"},
                new Dropdown(){Value = "Rotura", Text = "Rotura"},
                new Dropdown(){Value = "Otro", Text = "Otro"}
            };
            return lista;
        }
    }
}