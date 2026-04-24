using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LEARNINGUAM.Web.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Curso> Cursos { get; set; }

        public DbSet<Actividad> Actividades {get; set; }

        public DbSet<Cuestionario> Cuestionarios {get; set; }

        public DbSet<Pregunta> Preguntas {get; set; }

        public DbSet<Opcion> Opciones {get; set; }

        public DbSet<ActividadContenido> ActividadContenidos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<InscribirCurso> InscribirCurso { get; set; }

        public DbSet<Respuesta> Respuestas { get; set; }

        public DbSet<RespuestaActividad> RespuestasActividad { get; set; }

        public DbSet<Anuncio> Anuncios { get; set; }
        
    }
}
