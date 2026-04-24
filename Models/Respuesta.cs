using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("RESPUESTAS")]
public class Respuesta
{
    [Key]
    [Column("ID_RESPUESTA")]
    public int IdRespuesta { get; set; }

    [Column("ID_PREGUNTA")]
    public int IdPregunta { get; set; }

    [Column("ID_ALUMNO")]
    public int IdAlumno { get; set; }

    [Column("ID_OPCION")]
    public int IdOpcion { get; set; }

    [Column("FECHA_RESPUESTA")]
    public DateTime FechaRespuesta { get; set; }

    // Relaciones (opcional pero recomendado)
    /*[ForeignKey("IdPregunta")]
    public Pregunta Pregunta { get; set; }

    [ForeignKey("IdOpcion")]
    public Opcion Opcion { get; set; }

    [ForeignKey("IdAlumno")]
    public Usuario Alumno { get; set; }*/
}