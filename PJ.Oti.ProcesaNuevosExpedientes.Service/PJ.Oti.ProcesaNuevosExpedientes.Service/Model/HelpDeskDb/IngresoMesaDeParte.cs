using System;
using System.Collections.Generic;

namespace PJ.Oti.ProcesaNuevosExpedientes.Service.Model.HelpDeskDb;

public partial class IngresoMesaDeParte
{
    public Guid ImpId { get; set; }

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaAsociacion { get; set; }

    public string DescripcionSede { get; set; } = null!;

    public string CodigoInstancia { get; set; } = null!;

    public double IdentificadorUnico { get; set; }

    public int NumeroIncidente { get; set; }

    public double CantidadFolios { get; set; }

    public int Ventanilla { get; set; }

    public string NombreInstancia { get; set; } = null!;

    public int Modulo { get; set; }

    public string MotivoIngreso { get; set; } = null!;

    public string Distrito { get; set; } = null!;

    public string Provincia { get; set; } = null!;

    public string Especialidad { get; set; } = null!;

    public string Sede { get; set; } = null!;

    public int NumeroExpediente { get; set; }

    public int Anio { get; set; }

    public string OrganoJurisdiccional { get; set; } = null!;

    public string Expediente { get; set; } = null!;

    public string Usuario { get; set; } = null!;

    public double SecuenciaDigitalizacion { get; set; }

    public int AnioDigitalizacion { get; set; }

    public bool Procesado { get; set; }

    public bool SecActivo { get; set; }

    public string SecUsuarioCreacionId { get; set; } = null!;

    public string? SecUsuarioActualizacionId { get; set; }

    public string? SecUsuarioEliminacionId { get; set; }

    public DateTime SecFechaCreacion { get; set; }

    public DateTime? SecFechaActualizacion { get; set; }

    public DateTime? SecFechaEliminacion { get; set; }
}
