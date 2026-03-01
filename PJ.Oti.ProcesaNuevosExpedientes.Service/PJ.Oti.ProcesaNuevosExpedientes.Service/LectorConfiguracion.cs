using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJ.Oti.ProcesaNuevosExpedientes.Service
{
    public class LectorConfiguracion
    {
        public static Configuracion LeerConfiguracion(string rutaArchivo)
        {
            string json = File.ReadAllText(rutaArchivo);
            return JsonConvert.DeserializeObject<Configuracion>(json);
        }
    }
}
