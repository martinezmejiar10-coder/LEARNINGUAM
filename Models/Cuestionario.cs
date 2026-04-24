using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LEARNINGUAM.Web.Models
{
    [Table("CUESTIONARIOS")]
    public class Cuestionario
    {
        [Key]
        [Column("ID_CUESTIONARIO")]
        public int IdCuestionario { get; set; }

        [Column("ID_ACTIVIDAD")]
        public int IdActividad { get; set; }

        // Relación con Actividad
        [ForeignKey("IdActividad")]
        
        public Actividad Actividad { get; set; }
    }
}