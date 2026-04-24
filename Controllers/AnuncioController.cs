using Microsoft.AspNetCore.Mvc;
using LEARNINGUAM.Web.Models;
using System;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore;

namespace LEARNINGUAM.Web.Controllers
{
    public class AnunciosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnunciosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Crear([FromBody] Anuncio anuncio)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            if (idUsuario == 0)
                return Json(new { success = false, mensaje = "No autorizado" });

            anuncio.IdUsuario = idUsuario;
            anuncio.Fecha = DateTime.Now;

            _context.Anuncios.Add(anuncio);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        public IActionResult ObtenerPorCurso(int idCurso)
        {
            var anuncios = _context.Anuncios
                .Where(a => a.IdCurso == idCurso)
                .OrderByDescending(a => a.Fecha)
                .ToList();

            return Json(anuncios);
        }


        public class EliminarRequest
        {
            public int idAnuncio { get; set; }
        }

        [HttpPost]
        public IActionResult Eliminar([FromBody] EliminarRequest req)
        {
            try
            {
                if (req == null)
                    return Json(new { success = false, mensaje = "Request vacío" });

                Console.WriteLine("ID: " + req.idAnuncio);

                _context.Database.ExecuteSqlRaw(
                    $"DELETE FROM ANUNCIOS WHERE ID_ANUNCIO = {req.idAnuncio}"
                );

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }


        /*[HttpPost]
        public IActionResult Eliminar([FromBody] object data)
        {
            Console.WriteLine("RAW: " + data?.ToString());
            return Json(new { success = true });
        }*/


        [HttpPost]
        public IActionResult Editar([FromBody] Anuncio anuncio)
        {
            try
            {
                var existente = _context.Anuncios
                    .FirstOrDefault(a => a.IdAnuncio == anuncio.IdAnuncio);

                if (existente == null)
                    return Json(new { success = false, mensaje = "No encontrado" });

                existente.Titulo = anuncio.Titulo;
                existente.Contenido = anuncio.Contenido;
                existente.Archivos = anuncio.Archivos;

                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }
    }
}