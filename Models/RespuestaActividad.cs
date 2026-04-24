using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("RESPUESTAS_ACTIVIDAD")]
public class RespuestaActividad
{
    [Key]
    [Column("ID_RESPUESTA")]
    public int IdRespuesta { get; set; }

    [Column("ID_USUARIO")]
    public int IdUsuario { get; set; }

    [Column("ID_ACTIVIDAD")]
    public int IdActividad { get; set; }

    [Column("FECHA_RESPUESTA")]
    public DateTime FechaRespuesta { get; set; }

    [Column("CALIFICACION")]
    public int Calificacion { get; set; }

    [Column("DETALLE_JSON")]
    public string DetalleJson { get; set; }
}