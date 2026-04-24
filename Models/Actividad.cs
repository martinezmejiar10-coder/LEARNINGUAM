using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace LEARNINGUAM.Web.Models
{
    [Table("ACTIVIDADES")]
    public class Actividad
    {
        [Key]
        [Column("ID_ACTIVIDAD")]
        public int IdActividad { get; set; }

        [Column("TITULO")]
        [MaxLength(100)]
        public string Titulo { get; set; }

        [Column("DESCRIPCION")]
        [MaxLength(500)]
        public string Descripcion { get; set; }

        [Column("TIPO")]
        [MaxLength(30)]
        public string Tipo { get; set; }

        [Column("FECHA_CREACION")]
        public DateTime FechaCreacion { get; set; }

        [Column("ID_CURSO")]
        public int IdCurso { get; set; }

        // Relación con Curso
        [ForeignKey("IdCurso")]
        [ValidateNever]
        public Curso Curso { get; set; }

        [ValidateNever]
        public Cuestionario Cuestionario { get; set; }


        [Column("ESTADO")]
        [MaxLength(20)]
        public string Estado { get; set; } // Ejemplo: "Activo", "Inactivo", "Completado"

    }
}