using ClosedXML.Excel;
using PJ.Oti.ProcesaNuevosExpedientes.Service.Model.HelpDeskDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PJ.Oti.ProcesaNuevosExpedientes.Service
{
    public class ProcesadorDeDatos
    {
        private readonly HelpDeskDbContext _context;

        public ProcesadorDeDatos(HelpDeskDbContext context)
        {
            _context = context;
        }

        public void EjecutarProceso(string rutaArchivoExcel, List<string> correosDestino)
        {
            try
            {
                Console.WriteLine("Iniciando proceso de lectura y sincronización...");

                // 1. Leer el Excel y guardar los datos en la base de datos
                var datosExcel = LeerExcel(rutaArchivoExcel);
                InsertarDatosEnBD(datosExcel);

                var fechasProceso = datosExcel.Select(dato => dato.FechaIngreso.Date).Distinct().ToList();

                // 2. Procesar los registros no procesados
                ProcesarRegistros(fechasProceso);

                // 3. Enviar correo electrónico con los registros no procesados
                EnviarEmail(correosDestino, fechasProceso);

                Console.WriteLine("Proceso completado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }
        }

        private List<IngresoMesaDeParte> LeerExcel(string rutaArchivo)
        {
            var listaIngresos = new List<IngresoMesaDeParte>();
            using (var libro = new XLWorkbook(rutaArchivo))
            {
                var hoja = libro.Worksheet(1); // Asume que los datos están en la primera hoja
                var fila = 2; // Asume que la primera fila es el encabezado
                while (!hoja.Cell(fila, 1).IsEmpty())
                {
                    var ingreso = new IngresoMesaDeParte();

                    ingreso.ImpId = Guid.NewGuid(); // Genera un nuevo GUID para cada registro
                    ingreso.SecActivo = true;

                    string valorCelda;

                    // Intentamos leer el valor de cada celda como una cadena.
                    // Usamos TryParse para convertir de forma segura.

                    // 1. FechaIngreso (DateTime)
                    valorCelda = hoja.Cell(fila, 1).GetString();
                    if (DateTime.TryParse(valorCelda, out DateTime fechaIngreso))
                    {
                        ingreso.FechaIngreso = fechaIngreso;
                    }
                    else
                    {
                        // Asignamos un valor por defecto si la conversión falla.
                        ingreso.FechaIngreso = DateTime.Now;
                        Console.WriteLine($"Advertencia: FechaIngreso en la fila {fila} no es un formato válido. Se asignará la fecha actual.");
                    }

                    // 2. FechaAsociacion (DateTime)
                    valorCelda = hoja.Cell(fila, 2).GetString();
                    if (DateTime.TryParse(valorCelda, out DateTime fechaAsociacion))
                    {
                        ingreso.FechaAsociacion = fechaAsociacion;
                    }
                    else
                    {
                        ingreso.FechaAsociacion = null;
                        Console.WriteLine($"Advertencia: FechaAsociacion en la fila {fila} no es un formato válido. Se asignará valor null.");
                    }

                    // 3. DescripcionSede (string) - No requiere TryParse
                    ingreso.DescripcionSede = hoja.Cell(fila, 3).GetString();

                    // 4. CodigoInstancia (string)
                    ingreso.CodigoInstancia = hoja.Cell(fila, 4).GetString();

                    // 5. IdentificadorUnico (double)
                    valorCelda = hoja.Cell(fila, 5).GetString();
                    if (double.TryParse(valorCelda, out double identificadorUnico))
                    {
                        ingreso.IdentificadorUnico = identificadorUnico;
                    }
                    else
                    {
                        ingreso.IdentificadorUnico = 0;
                        Console.WriteLine($"Advertencia: IdentificadorUnico en la fila {fila} no es un formato válido. Se asignará 0.");
                    }

                    // 6. NumeroIncidente (int)
                    valorCelda = hoja.Cell(fila, 6).GetString();
                    if (int.TryParse(valorCelda, out int numeroIncidente))
                    {
                        ingreso.NumeroIncidente = numeroIncidente;
                    }
                    else
                    {
                        ingreso.NumeroIncidente = 0;
                        Console.WriteLine($"Advertencia: NumeroIncidente en la fila {fila} no es un formato válido. Se asignará 0.");
                    }

                    // 7. CantidadFolios (double)
                    valorCelda = hoja.Cell(fila, 7).GetString();
                    if (double.TryParse(valorCelda, out double cantidadFolios))
                    {
                        ingreso.CantidadFolios = cantidadFolios;
                    }
                    else
                    {
                        ingreso.CantidadFolios = 0;
                        Console.WriteLine($"Advertencia: CantidadFolios en la fila {fila} no es un formato válido. Se asignará 0.");
                    }

                    // 8. Ventanilla (int)
                    valorCelda = hoja.Cell(fila, 8).GetString();
                    if (int.TryParse(valorCelda, out int ventanilla))
                    {
                        ingreso.Ventanilla = ventanilla;
                    }
                    else
                    {
                        ingreso.Ventanilla = 0;
                        Console.WriteLine($"Advertencia: Ventanilla en la fila {fila} no es un formato válido. Se asignará 0.");
                    }

                    // 9. NombreInstancia (string)
                    ingreso.NombreInstancia = hoja.Cell(fila, 9).GetString();

                    // 10. Modulo (int)
                    valorCelda = hoja.Cell(fila, 10).GetString();
                    if (int.TryParse(valorCelda, out int modulo))
                    {
                        ingreso.Modulo = modulo;
                    }
                    else
                    {
                        ingreso.Modulo = 0;
                        Console.WriteLine($"Advertencia: Modulo en la fila {fila} no es un formato válido. Se asignará 0.");
                    }

                    // 11. MotivoIngreso (string)
                    ingreso.MotivoIngreso = hoja.Cell(fila, 11).GetString();

                    // 12. Distrito (string)
                    ingreso.Distrito = hoja.Cell(fila, 12).GetString();

                    // 13. Provincia (string)
                    ingreso.Provincia = hoja.Cell(fila, 13).GetString();

                    // 14. Especialidad (string)
                    ingreso.Especialidad = hoja.Cell(fila, 14).GetString();

                    // 15. Sede (string)
                    ingreso.Sede = hoja.Cell(fila, 15).GetString();

                    // 16. NumeroExpediente (int)
                    valorCelda = hoja.Cell(fila, 16).GetString();
                    if (int.TryParse(valorCelda, out int numeroExpediente))
                    {
                        ingreso.NumeroExpediente = numeroExpediente;
                    }
                    else
                    {
                        ingreso.NumeroExpediente = 0;
                        Console.WriteLine($"Advertencia: NumeroExpediente en la fila {fila} no es un formato válido. Se asignará 0.");
                    }

                    // 17. Anio (int)
                    valorCelda = hoja.Cell(fila, 17).GetString();
                    if (int.TryParse(valorCelda, out int anio))
                    {
                        ingreso.Anio = anio;
                    }
                    else
                    {
                        ingreso.Anio = 0;
                        Console.WriteLine($"Advertencia: Anio en la fila {fila} no es un formato válido. Se asignará 0.");
                    }

                    // 18. OrganoJurisdiccional (string)
                    ingreso.OrganoJurisdiccional = hoja.Cell(fila, 18).GetString();

                    // 19. Expediente (string)
                    ingreso.Expediente = hoja.Cell(fila, 19).GetString();

                    // 20. Usuario (string)
                    ingreso.Usuario = hoja.Cell(fila, 20).GetString();

                    // 21. SecuenciaDigitalizacion (double)
                    valorCelda = hoja.Cell(fila, 21).GetString();
                    if (double.TryParse(valorCelda, out double secuenciaDigitalizacion))
                    {
                        ingreso.SecuenciaDigitalizacion = secuenciaDigitalizacion;
                    }
                    else
                    {
                        ingreso.SecuenciaDigitalizacion = 0;
                        Console.WriteLine($"Advertencia: SecuenciaDigitalizacion en la fila {fila} no es un formato válido. Se asignará 0.");
                    }

                    // 22. AnioDigitalizacion (int)
                    valorCelda = hoja.Cell(fila, 22).GetString();
                    if (int.TryParse(valorCelda, out int anioDigitalizacion))
                    {
                        ingreso.AnioDigitalizacion = anioDigitalizacion;
                    }
                    else
                    {
                        ingreso.AnioDigitalizacion = 0;
                        Console.WriteLine($"Advertencia: AnioDigitalizacion en la fila {fila} no es un formato válido. Se asignará 0.");
                    }

                    ingreso.Procesado = false; // Se inicializa como no procesado

                    listaIngresos.Add(ingreso);
                    fila++;
                }
            }
            return listaIngresos;
        }

        private void InsertarDatosEnBD(List<IngresoMesaDeParte> datos)
        {
            Console.WriteLine("Verificando registros duplicados antes de la inserción...");

            // 1. Obtiene todos los NumeroExpediente existentes de la base de datos
            // y los carga en un HashSet para búsquedas rápidas.
            var expedientesExistentes = _context.IngresoMesaDePartes
                                                .Select(i => i.Expediente)
                                                .ToList();

            var expedientesSet = new HashSet<string>(expedientesExistentes);

            // 2. Filtra los registros del Excel para obtener solo los que no están duplicados.
            var nuevosRegistros = datos.Where(item => !expedientesSet.Contains(item.Expediente)).ToList();

            int registrosDuplicados = datos.Count - nuevosRegistros.Count;

            // 3. Inserta solo los registros que son nuevos.
            if (nuevosRegistros.Any())
            {
                _context.IngresoMesaDePartes.AddRange(nuevosRegistros);
                _context.SaveChanges();
                Console.WriteLine($"Se insertaron {nuevosRegistros.Count} nuevos registros en la base de datos.");
            }
            else
            {
                Console.WriteLine("No se encontraron registros nuevos para insertar.");
            }

            if (registrosDuplicados > 0)
            {
                Console.WriteLine($"Se omitieron {registrosDuplicados} registros que ya existían.");
            }
        }

        private void ProcesarRegistros(List<DateTime> fechasProceso)
        {
            Console.WriteLine("Buscando registros para procesar...");
            var expedientesNoProcesados = _context.IngresoExpedientes
                                                   .Where(e => e.IexProcesado == false && e.IexVariableEtnica == false && fechasProceso.Contains(e.IexHoraHost.Date))
                                                   .ToList();

            foreach (var expediente in expedientesNoProcesados)
            {
                var ingresosAMarcar = _context.IngresoMesaDePartes
                                              .Where(i => i.Procesado == false &&
                                                          expediente.IexSede.Contains(i.Sede) &&
                                                          i.FechaIngreso >= expediente.IexHoraHost &&
                                                          i.FechaIngreso.AddMinutes(-15) <= expediente.IexHoraHost)
                                              .OrderBy(i => i.FechaIngreso)
                                              .FirstOrDefault();

                if (ingresosAMarcar != null)
                {
                    // Marcar ambos como procesados
                    ingresosAMarcar.Procesado = true;
                    ingresosAMarcar.SecFechaActualizacion = DateTime.Now;
                    expediente.IexProcesado = true;
                    expediente.SecFechaActualizacion = DateTime.Now;
                    Console.WriteLine($"Se encontró coincidencia para el expediente {expediente.IexId}.");
                }
            }

            _context.SaveChanges();
            Console.WriteLine("Registros procesados y actualizados.");
        }

        private void EnviarEmail(List<string> correosDestino, List<DateTime> fechasProceso)
        {
            Console.WriteLine("Generando y enviando informe por correo electrónico...");
            var registrosNoProcesados = _context.IngresoMesaDePartes
                                                .Where(i => i.Procesado == false && fechasProceso.Contains(i.FechaIngreso.Date))
                                                .OrderBy(i => i.DescripcionSede)
                                                .ThenBy(i => i.FechaIngreso)
                                                .ToList();

            if (registrosNoProcesados.Any())
            {
                string tablaHtml = GenerarTablaHtml(registrosNoProcesados);

                // ** Nueva lógica para incrustar la imagen desde una ruta local **
                string rutaImagenLocal = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "logo.png"); // Cambia esta ruta a la de tu imagen
                string contentId = "logoApp"; // Identificador único para la imagen

                // 1. Crea una vista alternativa para el cuerpo HTML del correo.
                AlternateView vistaHtml = AlternateView.CreateAlternateViewFromString(GenerarCuerpoEmail(tablaHtml, contentId), null, "text/html");

                // 2. Crea un recurso vinculado (la imagen) a partir de la ruta local.
                LinkedResource recursoImagen = new LinkedResource(rutaImagenLocal, "image/png");
                recursoImagen.ContentId = contentId; // Asigna el identificador

                // 3. Añade la imagen a la vista HTML.
                vistaHtml.LinkedResources.Add(recursoImagen);

                // Configura tu cliente SMTP
                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("informatica_sanmartin@pj.gob.pe", "xunftteemocvvoya"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("no-reply@example.com", "Sistema de Sincronización"),
                    Subject = "Informe de Registros no Procesados para variable étnica",
                    // Ya no necesitas IsBodyHtml ni Body si usas AlternateView
                };
                mailMessage.AlternateViews.Add(vistaHtml);

                foreach (var correo in correosDestino)
                {
                    mailMessage.To.Add(correo);
                }

                try
                {
                    smtpClient.Send(mailMessage);
                    Console.WriteLine("Correo enviado exitosamente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar correo: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("No hay registros no procesados para enviar en el informe.");
            }
        }

        private string GenerarCuerpoEmail(string tablaHtml, string imagenCid)
        {
            string html = $@"
        <html>
        <body>
            <h1>Informe de Registros de Mesa de Partes no Procesados</h1>
            <p>A continuación, se presenta la lista de los registros del archivo de Excel que no pudieron ser procesados o asociados.</p>
            <img src='cid:{imagenCid}' alt='Logo de la aplicación' style='width:150px; height:auto;'/>
            <br>
            {tablaHtml}
        </body>
        </html>
    ";
            return html;
        }

        private string GenerarTablaHtml(List<IngresoMesaDeParte> registros)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table border='1' style='border-collapse: collapse;'>");
            sb.AppendLine("<thead><tr><th>Fecha Ingreso</th><th>Usuario</th><th>Sede</th><th>Expediente</th><th>Motivo</th><th>OO.JJ</th></tr></thead>");
            sb.AppendLine("<tbody>");
            foreach (var registro in registros)
            {
                sb.AppendLine($"<tr><td>{registro.FechaIngreso.ToString("dd/yy/MM HH:mm:ss")}</td><td>{registro.Usuario}</td><td>{registro.DescripcionSede}</td><td>{registro.Expediente}</td><td>{registro.MotivoIngreso}</td><td>{registro.NombreInstancia}</td></tr>");
            }
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            return sb.ToString();
        }
    }
}
