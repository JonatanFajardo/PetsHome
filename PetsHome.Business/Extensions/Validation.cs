using System;

namespace PetsHome.Business.Extensions
{
    public static class Validation
    {
        public static Boolean IsInsert(bool repositorio, bool modelState)
        {
            bool respuesta = true;
            if (repositorio != true && modelState == true)
            {
                respuesta = false;
            }
            return respuesta;
        }

        public static Boolean IsUpdate(bool repositorio, bool modelState)
        {
            bool respuesta = true;
            if (repositorio != true && modelState == true)
            {
                respuesta = false;
            }
            return respuesta;
        }
    }
}