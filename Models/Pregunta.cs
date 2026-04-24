using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LEARNINGUAM.Web.Models
{
    [Table("PREGUNTAS")]
    public class Pregunta
    {
        [Key]
        [Column("ID_PREGUNTA")]
        public int IdPregunta { get; set; }

        [Column("TEXTO_PREGUNTA")]
        [MaxLength(500)]
        public string? TextoPregunta { get; set; }

        [Column("ID_CUESTIONARIO")]
        public int IdCuestionario { get; set; }

        // Relación muchos a uno
        [ForeignKey("IdCuestionario")]
        [ValidateNever]
        public Cuestionario Cuestionario { get; set; }

        [Column("TIPO")]
        [MaxLength(20)]
        public string Tipo { get; set; } 


       
        //ValidateNever es para no validar las propiedades que no estene en el model orignial
        [ValidateNever]
        public ICollection<Opcion> Opciones { get; set; }
    }
}
