using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LEARNINGUAM.Web.Models
{
    [Table("CURSOS")] 

    public class Curso
    {
        [Key]
        [Column(("ID_CURSO"))]
        public int IdCurso { get; set; }

        [Column(("NOMBRE_CURSO"))]
        public string? NombreCurso { get; set; }

        [Column(("ID_PROFESOR"))]
        public int IdProfesor { get; set; }

        [Column(("DESCRIPCION"))]
        public string? Descripcion { get; set; } 

        [Column(("CODIGO_ACCESO"))]
        public string? CodigoAcceso { get; set; }
    }

}