# PJ - Procesa Variable Étnica (Sincronización Mesa de Partes / SIJ)

<!-- Badges (HTML dentro de Markdown) -->
<p align="left">
  <img alt="C#" src="https://img.shields.io/badge/C%23-100%25-512BD4?style=for-the-badge&logo=csharp&logoColor=white" />
  <img alt=".NET" src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img alt="EF Core" src="https://img.shields.io/badge/EF%20Core-9.x-6DB33F?style=for-the-badge&logo=databricks&logoColor=white" />
  <img alt="SQL Server" src="https://img.shields.io/badge/SQL%20Server-Database-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
  <img alt="ClosedXML" src="https://img.shields.io/badge/ClosedXML-Excel%20Reader-217346?style=for-the-badge&logo=microsoftexcel&logoColor=white" />
  <img alt="Console App" src="https://img.shields.io/badge/Console%20App-Worker%20Style-444?style=for-the-badge" />
</p>

Procesa los registros leídos desde un **Excel** (Mesa de Partes) y los cruza con registros en **base de datos (SIJ / HelpDesk)** para **marcar coincidencias** y **reportar por correo** los pendientes (no asociados / no procesados).

---

## 📦 El proyecto incluye

- Aplicación **Console** (.NET 8) con `Host.CreateDefaultBuilder`.
- Lectura de Excel con **ClosedXML**.
- Persistencia y consultas con **Entity Framework Core** (SQL Server).
- Lógica de deduplicación antes de insertar datos.
- Cruce de información (ventana temporal) para marcar procesados.
- Envío de correo SMTP con:
  - Cuerpo HTML (tabla de pendientes).
  - **Logo incrustado** (CID) desde `Assets/logo.png`.
- Configuración centralizada en `appsettings.json`.

---

## ✨ Características

- **Importación de Excel** (hoja 1, desde fila 2) con validaciones tipo `TryParse`.
- **Evita duplicados** comparando por `Expediente`.
- **Procesamiento por fechas**: toma las fechas de `FechaIngreso` importadas y trabaja por ese rango.
- **Asociación inteligente**:
  - Busca expedientes no procesados en BD.
  - Intenta encontrar el mejor match de Mesa de Partes según sede y ventana de tiempo.
- **Reporte automático** por correo con los registros no procesados (HTML + logo).

---

## 🤖 ¿Qué hace este proyecto?

1. **Lee** un archivo Excel desde una ruta configurada.
2. **Inserta** registros nuevos en la tabla de ingresos (evitando duplicados).
3. **Procesa** registros pendientes en otra tabla (expedientes) y marca coincidencias como procesadas.
4. **Envía** un correo con una tabla HTML listando lo que quedó **no procesado** (para revisión/acción manual).

---

## 🔁 Flujo general

1. **Inicio** del host (.NET Generic Host).
2. Carga de `appsettings.json`.
3. Creación de `HelpDeskDbContext` (SQL Server).
4. Resolución del servicio `ProcesadorDeDatos`.
5. Ejecución:
   - `LeerExcel(ruta)`
   - `InsertarDatosEnBD(datosExcel)`
   - `ProcesarRegistros(fechasProceso)`
   - `EnviarEmail(correosDestino, fechasProceso)`

---

## 🧠 ¿Cómo funciona?

### 1) Lectura del Excel
- Lee la **primera hoja**.
- Recorre filas hasta que la primera columna esté vacía.
- Convierte campos con `TryParse` y aplica defaults si fallan (por ejemplo, fechas o números).

### 2) Inserción evitando duplicados
- Obtiene los `Expediente` existentes en BD.
- Inserta solo los registros del Excel cuyo `Expediente` **no existe**.

### 3) Procesamiento / asociación
- Busca expedientes en la tabla de expedientes con condiciones similares a:
  - no procesado (`IexProcesado == false`)
  - variable étnica no marcada (`IexVariableEtnica == false`)
  - dentro de las fechas del proceso
- Para cada expediente:
  - busca un registro Mesa de Partes no procesado donde:
    - la sede coincida (comparación por `Contains`)
    - el `FechaIngreso` esté alrededor de `IexHoraHost` (ventana aproximada)
  - marca ambos como procesados y actualiza fecha de actualización.

### 4) Email de pendientes
- Obtiene registros de Mesa de Partes que quedaron `Procesado == false`.
- Genera tabla HTML.
- Incrusta `Assets/logo.png` con Content-ID para que aparezca dentro del HTML.
- Envía por SMTP (Gmail: host/puerto 587 + SSL).

---

## 🧩 Componentes principales

- **`Program.cs`**
  - Construye host, inyecta dependencias (`DbContext`, `ProcesadorDeDatos`).
  - Lee `ConfiguracionApp` desde `appsettings.json`.
  - Ejecuta el proceso principal.

- **`ProcesadorDeDatos.cs`**
  - Orquesta la lógica: lectura Excel → inserción → procesamiento → email.
  - Contiene métodos privados:
    - `LeerExcel`
    - `InsertarDatosEnBD`
    - `ProcesarRegistros`
    - `EnviarEmail`
    - `GenerarCuerpoEmail`
    - `GenerarTablaHtml`

- **`appsettings.json`**
  - `ConnectionStrings:DefaultConnection`
  - `ConfiguracionApp:RutaExcel`
  - `ConfiguracionApp:CorreosDestino`

- **`Assets/logo.png`**
  - Logo embebido en correo (CID).
  - Copiado al directorio de salida por configuración del `.csproj`.

---

## 🛠️ Configuración

En `appsettings.json`:

- **Cadena de conexión**
  - `ConnectionStrings:DefaultConnection`

- **Ruta del Excel**
  - `ConfiguracionApp:RutaExcel`

- **Correos destino**
  - `ConfiguracionApp:CorreosDestino` (lista)

> Recomendación: usa `appsettings.Development.json` o variables de entorno para no exponer credenciales ni rutas locales.

---

## ✅ Requisitos

- **.NET SDK 8.0**
- Acceso a **SQL Server** (y cadena de conexión válida)
- Archivo Excel accesible en la ruta configurada
- Acceso a un servidor SMTP (si usas Gmail: app password / políticas de cuenta)

---

## 🧪 Tecnologías (Dependencias principales)

Paquetes NuGet observados:

- `ClosedXML`
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools` (dev)
- `Microsoft.EntityFrameworkCore.Design` (dev)
- `Microsoft.Extensions.Hosting`
- `Newtonsoft.Json`

---

## 🗃️ Persistencia (tablas esperadas)

Según las consultas y escrituras en el código, se esperan entidades/tablas similares a:

- `IngresoMesaDePartes`
  - Usada para **insertar** datos del Excel.
  - Campos relevantes observados: `Expediente`, `FechaIngreso`, `DescripcionSede`, `Usuario`, `Procesado`, `SecFechaActualizacion`, etc.

- `IngresoExpedientes`
  - Usada para **cruzar** y marcar procesado.
  - Campos relevantes observados: `IexProcesado`, `IexVariableEtnica`, `IexHoraHost`, `IexSede`, `SecFechaActualizacion`, etc.

> Nota: los nombres exactos (schema/PK) dependen de tu `DbContext` y modelos, pero el README asume estas tablas por el uso directo en el servicio.

---

## 🚀 Instalación / Ejecución

1. Clonar el repositorio
2. Abrir la solución:
   - `PJ.Oti.ProcesaNuevosExpedientes.Service.sln`
3. Configurar `appsettings.json`:
   - Connection string real
   - Ruta real del Excel
   - Correos destino reales
4. Restaurar y ejecutar:
   - `dotnet restore`
   - `dotnet run --project PJ.Oti.ProcesaNuevosExpedientes.Service/PJ.Oti.ProcesaNuevosExpedientes.Service`

Salida esperada en consola (resumen):
- Inicio de lectura
- Inserción de nuevos registros / omisión de duplicados
- Marcado de procesados
- Envío de correo o mensaje indicando que no hay pendientes

---

## 🧯 Solución de problemas

- **No encuentra `appsettings.json`**
  - Verifica que esté en el mismo directorio desde donde ejecutas.
  - Está configurado con `CopyToOutputDirectory=PreserveNewest`.

- **Falla al leer el Excel**
  - Revisa la ruta `ConfiguracionApp:RutaExcel`.
  - Confirma formato y que la hoja 1 tenga encabezado en fila 1 y datos desde fila 2.

- **No envía correos**
  - Revisa host/puerto/SSL y credenciales SMTP.
  - Si es Gmail: usa contraseña de aplicación y valida permisos/políticas.
  - Verifica que `Assets/logo.png` exista en el output (porque se adjunta como recurso vinculado).

- **No marca procesados**
  - Verifica consistencia de sede (`IexSede` vs `Sede`) y la ventana temporal.
  - Confirma que existan registros `IngresoExpedientes` en las fechas del proceso.

---

## 🗺️ Roadmap

- [ ] Externalizar credenciales SMTP (secret manager / env vars)
- [ ] Logging estructurado (Serilog) y niveles configurables
- [ ] Parametrizar ventana temporal de asociación (minutos)
- [ ] Reporte en Excel/PDF adicional para auditoría
- [ ] Tests unitarios para parsing, deduplicación y matching
- [ ] Modo “dry-run” (solo simular sin escribir en BD)

---

## 📄 Licencia

Pendiente de definir.  
Sugerencia: agrega `LICENSE` (MIT/Apache-2.0/Propietaria) según corresponda al uso institucional.
