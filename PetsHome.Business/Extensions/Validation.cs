using System;

namespace PetsHome.Business.Extensions
{
    public static class Validation
    {
        //public static Boolean IsInsert(int repositorio, bool modelState)
        //{
        //    //List<T> lista = new List<T>();
        //    //int Id = lista.ElementAt(0);
        //    bool respuesta = true;
        //    if (repositorio != 0 && modelState == true)
        //    {
        //        respuesta = false;
        //    }
        //    return respuesta;
        //}

        public static Boolean IsInsert(bool repositorio, bool modelState)
        {
            //List<T> lista = new List<T>();
            //int Id = lista.ElementAt(0);
            bool respuesta = true;
            if (repositorio != true && modelState == true)
            {
                respuesta = false;
            }
            return respuesta;
        }

        public static Boolean IsUpdate(bool repositorio, bool modelState)
        {
            //List<T> lista = new List<T>();
            //int Id = lista.ElementAt(0);
            bool respuesta = true;
            if (repositorio != true && modelState == true)
            {
                respuesta = false;
            }
            return respuesta;
        }
    }
}
