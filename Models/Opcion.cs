using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LEARNINGUAM.Web.Models
{
    [Table("OPCIONES")]
    public class Opcion
    {
        [Key]
        [Column("ID_OPCION")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdOpcion { get; set; }

        [Column("TEXTO_OPCION")]
        [MaxLength(300)]
        public string Texto { get; set; }

        [Column("ES_CORRECTA")]
        public int EsCorrecta { get; set; } // 1 = correcta, 0 = incorrecta

        [Column("ID_PREGUNTA")]
        public int IdPregunta { get; set; }

        // Relación muchos a uno
        [ForeignKey("IdPregunta")]
        public Pregunta Pregunta { get; set; }
    }
}
