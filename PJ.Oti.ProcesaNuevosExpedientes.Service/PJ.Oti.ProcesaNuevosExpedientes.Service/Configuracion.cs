using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJ.Oti.ProcesaNuevosExpedientes.Service
{
    public class Configuracion
    {
        public string RutaExcel { get; set; }
        public List<string> CorreosDestino { get; set; }
    }
}
