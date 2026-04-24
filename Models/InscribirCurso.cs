using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("INSCRIBIRCURSO")]
public class InscribirCurso
{
    [Key]
    [Column("ID_INSCRIPCION")]
    public int IdInscripcion { get; set; }

    [Column("ID_USUARIO")]
    public int IdUsuario { get; set; }

    [Column("ID_CURSO")]
    public int IdCurso { get; set; }

    [Column("ID_ROL")]
    public int IdRol { get; set; }
}