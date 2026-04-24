using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LEARNINGUAM.Web.Models
{
    [Table("ACTIVIDAD_CONTENIDO")]
    public class ActividadContenido
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_CONTENIDO")]
        public int IdContenido { get; set; }

        [Column("ID_ACTIVIDAD")]
        public int IdActividad { get; set; }

        [Column("TIPO")]
        [MaxLength(30)]
        public string? Tipo { get; set; }

        [Column("CONTENIDO")]
        public string? Contenido { get; set; }

        // Relación con Actividad
        [ForeignKey("IdActividad")]
        public Actividad Actividad { get; set; }
        
    } 
}