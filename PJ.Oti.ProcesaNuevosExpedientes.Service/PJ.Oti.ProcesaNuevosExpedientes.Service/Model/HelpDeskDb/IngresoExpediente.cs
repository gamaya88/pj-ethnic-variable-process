using System;
using System.Collections.Generic;

namespace PJ.Oti.ProcesaNuevosExpedientes.Service.Model.HelpDeskDb;

public partial class IngresoExpediente
{
    public Guid IexId { get; set; }

    public string IexFraseActivacion { get; set; } = null!;

    public string IexUsuario { get; set; } = null!;

    public string IexHost { get; set; } = null!;

    public DateTime IexHoraHost { get; set; }

    public bool IexProcesado { get; set; }

    public string IexSede { get; set; } = null!;

    public bool IexVariableEtnica { get; set; }

    public bool SecActivo { get; set; }

    public string SecUsuarioCreacionId { get; set; } = null!;

    public string? SecUsuarioActualizacionId { get; set; }

    public string? SecUsuarioEliminacionId { get; set; }

    public DateTime SecFechaCreacion { get; set; }

    public DateTime? SecFechaActualizacion { get; set; }

    public DateTime? SecFechaEliminacion { get; set; }
}
