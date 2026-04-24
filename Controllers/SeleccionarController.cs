using Microsoft.AspNetCore.Mvc;
using LEARNINGUAM.Web.Models;
using System.Linq;

namespace LEARNINGUAM.Web.Controllers
{
    public class SeleccionarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeleccionarController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*public IActionResult Seleccionar(int idActividad, int idCurso)
        {
            ViewBag.IdActividad = idActividad;
            ViewBag.IdCurso = idCurso;

            return View();
        }*/ 


        public IActionResult Seleccionar(int idActividad, int idCurso)
        {
            var contenido = _context.ActividadContenidos
                .Where(a => a.IdActividad == idActividad)
                .Select(a => a.Contenido)
                .FirstOrDefault();

            ViewBag.IdActividad = idActividad;
            ViewBag.IdCurso = idCurso;
            ViewBag.Contenido = contenido; 

            return View();
        }






        public IActionResult Resolver(int idActividad)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            if (idUsuario == 0)
                return RedirectToAction("Login", "Cuenta");

            var actividad = _context.Actividades
                .FirstOrDefault(a => a.IdActividad == idActividad);

            if (actividad == null)
                return NotFound();

            int existe = _context.RespuestasActividad
                .Count(r => r.IdActividad == idActividad && r.IdUsuario == idUsuario);

            if (existe > 0)
            {
                return RedirectToAction("VerResultadoSeleccionar",
                    new { idActividad });
            }

            var contenido = _context.ActividadContenidos
                .Where(a => a.IdActividad == idActividad)
                .Select(a => new ActividadContenido
                {
                    IdActividad = a.IdActividad,
                    Contenido = a.Contenido
                })
                .FirstOrDefault();

            if (contenido == null || contenido.Contenido == null)
            {
                return Content("Esta actividad aún no tiene contenido.");
            }

            ViewBag.IdCurso = actividad.IdCurso;
            ViewBag.IdActividad = idActividad;

            return View(new List<ActividadContenido> { contenido });
        }

        [HttpPost]
        public IActionResult GuardarRespuesta([FromBody] System.Text.Json.JsonElement data)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            if (idUsuario == 0)
                return Unauthorized();

            int idActividad = data.GetProperty("idActividad").GetInt32();

            var respuestasJson = data.GetProperty("respuestas").GetRawText();

            if (string.IsNullOrWhiteSpace(respuestasJson) || respuestasJson == "[]")
            {
                return Json(new { success = false });
            }

            var existente = _context.RespuestasActividad
                .FirstOrDefault(r => r.IdActividad == idActividad &&
                                    r.IdUsuario == idUsuario);

            if (existente != null)
            {
                existente.DetalleJson = respuestasJson;
                existente.FechaRespuesta = DateTime.Now;
            }
            else
            {
                _context.RespuestasActividad.Add(new RespuestaActividad
                {
                    IdUsuario = idUsuario,
                    IdActividad = idActividad,
                    FechaRespuesta = DateTime.Now,
                    DetalleJson = respuestasJson
                });
            }

            _context.SaveChanges();

            return Json(new { success = true });
        }

        public IActionResult VerResultadoSeleccionar(int idActividad)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            var actividad = _context.Actividades
                .FirstOrDefault(a => a.IdActividad == idActividad);

            if (actividad == null)
                return NotFound();

            var respuesta = _context.RespuestasActividad
                .FirstOrDefault(r => r.IdActividad == idActividad &&
                                    r.IdUsuario == idUsuario);

            if (respuesta == null || string.IsNullOrEmpty(respuesta.DetalleJson))
            {
                return Content("No hay respuestas");
            }

            var detalle = System.Text.Json.JsonSerializer
                .Deserialize<List<DetalleArrastrarVM>>(respuesta.DetalleJson);

            ViewBag.IdCurso = actividad.IdCurso;

            return View("ResultadoSeleccionar", detalle);
        }
    }
}