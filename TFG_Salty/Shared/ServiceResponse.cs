using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFG_Salty.Shared
{
    /// <summary>
    /// Modelo que parametriza las respuestas que se dan desde la API. 
    /// De esta manera cohesionamos el concepto de respuesta y trabajamos de manera coherente con los distintos servicios sabiendo lo que devuelven
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}
