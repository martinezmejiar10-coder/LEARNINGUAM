using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LEARNINGUAM.Web.Models
{
    [Table("USUARIOS")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Column("CORREO")]
        public string Correo { get; set; }

        [Column("CONTRASEÑA")]
        public string? Contraseña { get; set; }

        /*[Column("ID_ROL")]
        public int IdRol { get; set; }*/
    }
}