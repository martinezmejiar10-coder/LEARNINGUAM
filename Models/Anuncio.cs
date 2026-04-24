using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ANUNCIOS")]
public class Anuncio
{
    [Key]
    [Column("ID_ANUNCIO")]
    public int IdAnuncio { get; set; }

    [Column("ID_CURSO")]
    public int IdCurso { get; set; }

    [Column("ID_USUARIO")]
    public int IdUsuario { get; set; }

    [Column("TITULO")]
    public string? Titulo { get; set; }

    [Column("CONTENIDO")]
    public string? Contenido { get; set; }

    [Column("ARCHIVOS")]
    public string? Archivos { get; set; } // JSON

    [Column("FECHA")]
    public DateTime Fecha { get; set; }
}