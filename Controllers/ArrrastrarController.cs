using Microsoft.AspNetCore.Mvc;
using LEARNINGUAM.Web.Models;
using System;
using System.Linq;
using System.Text.Json;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

namespace LEARNINGUAM.Web.Controllers
{
    public class ArrastrarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;

        public ArrastrarController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("OracleConnection");
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

            var respuesta = _context.RespuestasActividad
                .FirstOrDefault(r => r.IdActividad == idActividad &&
                                     r.IdUsuario == idUsuario);

            if (respuesta != null)
            {
                return RedirectToAction("VerResultadoArrastrar",
                    new { idActividad });
            }

            var contenido = _context.ActividadContenidos
                .Where(a => a.IdActividad == idActividad)
                .ToList();

            ViewBag.IdCurso = actividad.IdCurso;
            ViewBag.IdActividad = idActividad;

            return View(contenido);
        }

        /*[HttpPost]
        public IActionResult GuardarRespuesta(int idActividad, IFormCollection form)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario").Value;

            var actividad = _context.Actividades
                .FirstOrDefault(a => a.IdActividad == idActividad);

            if (actividad == null)
                return NotFound();

            var contenido = _context.ActividadContenidos
                .FirstOrDefault(a => a.IdActividad == idActividad);

            var data = System.Text.Json.JsonSerializer
                .Deserialize<ContenidoArrastrar>(contenido.Contenido);

            int correctas = 0;
            int total = data.espacios.Count;

            var detalle = new List<object>();

            foreach (var espacio in data.espacios)
            {
                string respuesta = form["respuesta_" + espacio.id];

                bool esCorrecta = !string.IsNullOrEmpty(respuesta) &&
                    respuesta.Trim().ToLower() == espacio.palabra.ToLower();

                if (esCorrecta)
                    correctas++;

                detalle.Add(new
                {
                    Id = espacio.id,
                    Usuario = respuesta,
                    Correcta = espacio.palabra,
                    EsCorrecta = esCorrecta
                });

                Console.WriteLine("Campo: respuesta_" + espacio.id + " = " + form["respuesta_" + espacio.id]);
            }

            var jsonDetalle = System.Text.Json.JsonSerializer.Serialize(detalle);

            _context.RespuestasActividad.Add(new RespuestaActividad
            {
                IdUsuario = idUsuario,
                IdActividad = idActividad,
                FechaRespuesta = DateTime.Now,
                DetalleJson = jsonDetalle
            });

            _context.SaveChanges();

            return RedirectToAction("VerResultadoArrastrar",
                new { idActividad });
        }*/

        [HttpPost]
        public IActionResult GuardarRespuesta([FromBody] JsonElement data)
        {
            Console.WriteLine("ENTRÓ A GUARDAR RESPUESTA");

            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            if (idUsuario == 0)
                return Unauthorized();

            int idActividad = data.GetProperty("idActividad").GetInt32();

            var respuestasJson = data.GetProperty("respuestas").GetRawText();

            Console.WriteLine("JSON recibido: " + respuestasJson);

            if (string.IsNullOrWhiteSpace(respuestasJson) || respuestasJson == "[]")
            {
                return Json(new { success = false, mensaje = "Respuestas vacías" });
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

            Console.WriteLine("JSON guardado: " + respuestasJson);

            return Json(new { success = true });
        }




        public IActionResult VerResultadoArrastrar(int idActividad)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            if (idUsuario == 0)
                return RedirectToAction("Login", "Cuenta");

            var actividad = _context.Actividades
                .FirstOrDefault(a => a.IdActividad == idActividad);

            if (actividad == null)
                return NotFound();

            string json = "";

            using (var conn = new OracleConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT DETALLE_JSON
                        FROM RESPUESTAS_ACTIVIDAD
                        WHERE ID_ACTIVIDAD = :idActividad
                        AND ID_USUARIO = :idUsuario
                        ORDER BY FECHA_RESPUESTA DESC
                        FETCH FIRST 1 ROWS ONLY";

                    cmd.Parameters.Add(new OracleParameter("idActividad", idActividad));
                    cmd.Parameters.Add(new OracleParameter("idUsuario", idUsuario));

                    var result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        json = result.ToString();
                    }

                    Console.WriteLine("JSON obtenido: " + json);
                }
            }

            if (string.IsNullOrWhiteSpace(json) || json == "[]" || json == "[{}]")
            {
                return Content("No hay respuestas guardadas o el JSON está vacío");
            }

            List<DetalleArrastrarVM> detalle;

            try
            {
                detalle = System.Text.Json.JsonSerializer.Deserialize<List<DetalleArrastrarVM>>(
                    json,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al deserializar: " + ex.Message);
                return Content("Error al procesar las respuestas");
            }

            // 🔴 VALIDAR RESULTADO FINAL
            if (detalle == null || detalle.Count == 0)
            {
                return Content("No se pudieron obtener las respuestas correctamente");
            }

            ViewBag.IdCurso = actividad.IdCurso;

            return View("ResultadoArrastrar", detalle);
        }



        public IActionResult Arrastrar(int idActividad)
        {
            var actividad = _context.Actividades
                .FirstOrDefault(a => a.IdActividad == idActividad);

            if (actividad == null)
                return NotFound();

            var contenido = _context.ActividadContenidos
                .Where(a => a.IdActividad == idActividad)
                .Select(a => a.Contenido)
                .FirstOrDefault();

            ViewBag.IdActividad = idActividad;
            ViewBag.IdCurso = actividad.IdCurso;
            ViewBag.Contenido = contenido;

            return View();
        }
    }
}