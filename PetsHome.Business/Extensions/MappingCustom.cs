using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetsHome.Business.Extensions
{
    public class MappingCustom
    {
        public static EmpleadoViewModel Map(PR_Refugio_Empleados_FindResult map)
        {
            EmpleadoViewModel model = new EmpleadoViewModel()
            {
                emp_Id = map.emp_Id,
                emp_Codigo = map.emp_Codigo,
                cag_Id = map.cag_Id,
                cag_Descripcion = map.cag_Descripcion,
                refg_Id = map.refg_Id,
                refg_Nombre = map.refg_Nombre,
                emp_EsActivo = map.emp_EsActivo,
                esActivo = map.esActivo,
                per = new PersonaViewModel()
                {
                    per_Id = map.per_Id,
                    per_PrimerNombre = map.per_PrimerNombre,
                    per_SegundoNombre = map.per_SegundoNombre,
                    per_ApellidoPaterno = map.per_ApellidoPaterno,
                    per_ApellidoMaterno = map.per_ApellidoMaterno,
                    per_Identidad = map.per_Identidad,
                    per_FechaNacimiento = map.per_FechaNacimiento,
                    per_Domicilio = map.per_Domicilio,
                    per_Telefono = map.per_Telefono,
                    per_Correo = map.per_Correo,
                    per_UsuarioCrea = map.per_UsuarioCrea,
                    per_FechaCrea = map.per_FechaCrea,
                    per_UsuarioModifica = map.per_UsuarioModifica,
                    per_FechaModifica = map.per_FechaModifica
                },
            };
            return model;
        }
        //internal static VoluntarioViewModel Map(PR_Refugio_Voluntarios_ListResult map)
        //{
        //    VoluntarioViewModel model = new VoluntarioViewModel()
        //    {
        //        vol_Id = map.vol_Id,
        //        vol_HorasTrabajadas = map.vol_HorasTrabajadas,
        //        emp_Nombres = map.per_PrimerNombre + " " + map.per_ApellidoPaterno,
        //        per = new PersonaViewModel()
        //        {
        //            per_Identidad = map.per_Identidad
        //        }
        //    };
        //    return model;
        //}

        public static VoluntarioViewModel Map(PR_Refugio_Voluntarios_FindResult map)
        {
            VoluntarioViewModel model = new VoluntarioViewModel()
            {
                vol_Id = map.vol_Id,
                vol_HorasTrabajadas = map.vol_HorasTrabajadas,
                vol_Recurrente = map.vol_Recurrente,
                estado = map.estado,
                per = new PersonaViewModel()
                {
                    per_Id = map.per_Id,
                    per_PrimerNombre = map.per_PrimerNombre,
                    per_SegundoNombre = map.per_SegundoNombre,
                    per_ApellidoPaterno = map.per_ApellidoPaterno,
                    per_ApellidoMaterno = map.per_ApellidoMaterno,
                    per_Identidad = map.per_Identidad,
                    per_FechaNacimiento = map.per_FechaNacimiento,
                    per_Domicilio = map.per_Domicilio,
                    per_Telefono = map.per_Telefono,
                    per_Correo = map.per_Correo,
                    usuarioCrea = map.usuarioCrea,
                    usuarioModifica = map.usuarioModifica
                },
            };
            return model;
        }
    }
}
