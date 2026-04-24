using Microsoft.AspNetCore.Mvc;
using LEARNINGUAM.Web.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LEARNINGUAM.Web.Controllers
{
    public class ActividadesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActividadesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var actividades = _context.Actividades.ToList();
            return View(actividades);
        }


        public IActionResult Create(int idCurso)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            if (idUsuario == 0)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var rol = _context.InscribirCurso
                .Where(i => i.IdCurso == idCurso && i.IdUsuario == idUsuario)
                .Select(i => i.IdRol)
                .FirstOrDefault();

            if (rol != 3)
            {
                return RedirectToAction("Foro", "Cursos", new { idCurso = idCurso });
            }

            return View(new Actividad { IdCurso = idCurso });
        }

    


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Actividad actividad)
        {
            int usuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            var rol = _context.InscribirCurso
                .Where(i => i.IdCurso == actividad.IdCurso && i.IdUsuario == usuarioId)
                .Select(i => i.IdRol)
                .FirstOrDefault();

            if (rol != 3)
                return Unauthorized();

            if (!ModelState.IsValid)
                return View(actividad);

            actividad.FechaCreacion = DateTime.Now;

            _context.Actividades.Add(actividad);
            _context.SaveChanges();

            if (actividad.Tipo == "CUESTIONARIO")
            {
                var cuestionario = new Cuestionario
                {
                    IdActividad = actividad.IdActividad
                };

                _context.Cuestionarios.Add(cuestionario);
                _context.SaveChanges();

                return RedirectToAction("Create", "Preguntas",
                    new { 
                        idCuestionario = cuestionario.IdCuestionario,
                        idCurso = actividad.IdCurso   
                    });
            }

            if (actividad.Tipo == "ARRASTRAR")
            {
                return RedirectToAction("Arrastrar", "Arrastrar",
                    new { 
                        idActividad = actividad.IdActividad,
                        idCurso = actividad.IdCurso   
                    });
            }

            if (actividad.Tipo == "SELECCIONAR")
            {
                return RedirectToAction("Seleccionar", "Seleccionar",
                    new {
                        idActividad = actividad.IdActividad,
                        idCurso = actividad.IdCurso
                    });
            }

            TempData["Mensaje"] = "Actividad creada con éxito";
            return RedirectToAction("Foro", "Cursos",
                new { idCurso = actividad.IdCurso });
        }




        public IActionResult Confirmacion(int idCurso) 
        {
            ViewBag.IdCurso = idCurso;
            return View();
        }

        public IActionResult Menu(int? idCurso)
        {
            if (idCurso == null || idCurso == 0)
                return RedirectToAction("Index", "Cursos");

            ViewBag.IdCurso = idCurso;
            return View();
        }


        public IActionResult Detalles(int idActividad)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            var actividad = _context.Actividades
                .FirstOrDefault(a => a.IdActividad == idActividad);

            if (actividad == null)
                return NotFound();

            var rol = _context.InscribirCurso
                .Where(i => i.IdCurso == actividad.IdCurso && i.IdUsuario == idUsuario)
                .Select(i => i.IdRol)
                .FirstOrDefault();

            var tipo = actividad.Tipo?.Trim().ToUpper();

            Console.WriteLine("TIPO REAL: " + tipo);

            if (rol == 3)
            {
                if (tipo == "ARRASTRAR")
                {
                    return RedirectToAction("Arrastrar", "Arrastrar",
                        new { idActividad = actividad.IdActividad });
                }

                if (tipo == "CUESTIONARIO")
                {
                    var cuestionario = _context.Cuestionarios
                        .FirstOrDefault(c => c.IdActividad == idActividad);

                    if (cuestionario == null)
                        return NotFound();

                    return RedirectToAction("Create", "Preguntas",
                        new { idCuestionario = cuestionario.IdCuestionario, idCurso = actividad.IdCurso });
                }

                if (tipo == "SELECCIONAR")
                {
                    return RedirectToAction("Seleccionar", "Seleccionar",
                        new {
                            idActividad = actividad.IdActividad,
                            idCurso = actividad.IdCurso
                        });
                }
            }

            if (rol == 4)
            {
                if (tipo == "ARRASTRAR")
                {
                    int existe = _context.RespuestasActividad
                        .Count(r => r.IdActividad == idActividad && r.IdUsuario == idUsuario);

                    if (existe > 0)
                    {
                        return RedirectToAction("VerResultadoArrastrar", "Arrastrar",
                            new { idActividad = actividad.IdActividad });
                    }

                    return RedirectToAction("Resolver", "Arrastrar",
                        new { idActividad = actividad.IdActividad });
                }

                if (tipo == "CUESTIONARIO")
                {
                    var cuestionario = _context.Cuestionarios
                        .FirstOrDefault(c => c.IdActividad == idActividad);

                    if (cuestionario == null)
                        return NotFound();

                    var preguntasIds = _context.Preguntas
                        .Where(p => p.IdCuestionario == cuestionario.IdCuestionario)
                        .Select(p => p.IdPregunta)
                        .ToList();

                    int existe = _context.Respuestas
                        .Count(r => r.IdAlumno == idUsuario &&
                                    preguntasIds.Contains(r.IdPregunta));

                    bool yaRespondio = existe > 0;

                    if (yaRespondio)
                    {
                        return RedirectToAction("VerResultadoCuestionario",
                            new { idActividad });
                    }

                    return RedirectToAction("ResolverCuestionario",
                        new { idActividad });
                }

                if (tipo == "SELECCIONAR")
                {
                    return RedirectToAction("Resolver", "Seleccionar",
                        new {
                            idActividad = actividad.IdActividad,
                            idCurso = actividad.IdCurso
                        });
                }
            }

            return Unauthorized();
        }


       

        /*public IActionResult Arrastrar(int idActividad)
        {
            var actividad = _context.Actividades
                .FirstOrDefault(a => a.IdActividad == idActividad);

            if (actividad == null)
                return NotFound();

            var contenido = _context.ActividadContenidos
                .FirstOrDefault(a => a.IdActividad == idActividad);

            ViewBag.IdActividad = idActividad;
            ViewBag.IdCurso = actividad.IdCurso;
            ViewBag.Contenido = contenido?.Contenido; 
            return View();
        }*/

        public IActionResult ResolverArrastrar(int idActividad)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            var actividad = _context.Actividades
                .FirstOrDefault(a => a.IdActividad == idActividad);

            if (actividad == null)
                return NotFound();

            int existe = _context.RespuestasActividad
                .Count(r => r.IdActividad == idActividad && r.IdUsuario == idUsuario);

            bool yaRespondio = existe > 0;

            if (yaRespondio)
            {
                return View("ActividadRespondida", actividad.IdCurso); 
            }

            var contenido = _context.ActividadContenidos
                .Where(a => a.IdActividad == idActividad)
                .ToList();

            ViewBag.IdCurso = actividad.IdCurso; 
            ViewBag.IdActividad = idActividad;

            return View(contenido);
        }



        [HttpPost]
        public IActionResult GuardarRespuestaArrastrar([FromBody] int idActividad)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            int existe = _context.RespuestasActividad
                .Count(r => r.IdActividad == idActividad && r.IdUsuario == idUsuario);

            if (existe == 0)
            {
                var respuesta = new RespuestaActividad
                {
                    IdActividad = idActividad,
                    IdUsuario = idUsuario,
                    FechaRespuesta = DateTime.Now
                };

                _context.RespuestasActividad.Add(respuesta);
                _context.SaveChanges();
            }

            return Json(new { success = true });
        }
        

        /*public IActionResult Seleccionar(int idActividad)
        {
            var actividad = _context.Actividades
                .FirstOrDefault(a => a.IdActividad == idActividad);

            if (actividad == null)
                return NotFound();

            var contenido = _context.ActividadContenidos
                .FirstOrDefault(a => a.IdActividad == idActividad);

            ViewBag.IdActividad = idActividad;
            ViewBag.IdCurso = actividad.IdCurso;
            ViewBag.Contenido = contenido?.Contenido;

            return View();
        }*/


        public IActionResult ResolverSeleccionar(int idActividad)
        {
            var contenido = _context.ActividadContenidos
                .Where(a => a.IdActividad == idActividad)
                .ToList();

            return View(contenido);
        }



        [HttpPost]
        public IActionResult GuardarContenido([FromBody] ActividadContenido contenido)
        {
            var existente = _context.ActividadContenidos
                .Where(a => a.IdActividad == contenido.IdActividad)
                .Select(a => new ActividadContenido
                {
                    IdActividad = a.IdActividad,
                    Contenido = a.Contenido
                })
                .FirstOrDefault();

            if (existente != null)
            {
                var original = _context.ActividadContenidos
                    .FirstOrDefault(a => a.IdActividad == contenido.IdActividad);

                if (original != null)
                {
                    original.Contenido = contenido.Contenido;
                }

                TempData["Mensaje"] = "La Actividad se actualizó correctamente";
            }
            else
            {
                _context.ActividadContenidos.Add(contenido);
                TempData["Mensaje"] = "La Actividad se creó correctamente";
            }

            _context.SaveChanges();

            var respuestas = _context.RespuestasActividad
                .Where(r => r.IdActividad == contenido.IdActividad);

            _context.RespuestasActividad.RemoveRange(respuestas);
            _context.SaveChanges();

            return Json(new { success = true });
        }


        
        public IActionResult ResolverCuestionario(int idActividad)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario").Value;

            var cuestionario = _context.Cuestionarios
                .FirstOrDefault(c => c.IdActividad == idActividad);

            if (cuestionario == null)
                return NotFound();

            
            var preguntasIds = _context.Preguntas
                .Where(p => p.IdCuestionario == cuestionario.IdCuestionario)
                .Select(p => p.IdPregunta)
                .ToList();

           
            bool yaRespondio = _context.Respuestas
                .Count(r => r.IdAlumno == idUsuario &&
                            preguntasIds.Contains(r.IdPregunta)) > 0;

            if (yaRespondio)
            {
                var actividad = _context.Actividades
                    .FirstOrDefault(a => a.IdActividad == idActividad);

                return View("ActividadRespondida", actividad.IdCurso);
            }

            var preguntas = _context.Preguntas
                .Where(p => p.IdCuestionario == cuestionario.IdCuestionario)
                .Include(p => p.Opciones)
                .ToList();

            return View(preguntas);
        }


        [HttpPost]
        public IActionResult ResolverCuestionario(IFormCollection form)
        {
            int usuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            int total = 0;
            int correctas = 0;

            var preguntasIds = form.Keys
                .Where(k => k.StartsWith("pregunta_"))
                .Select(k => int.Parse(k.Replace("pregunta_", "")))
                .ToList();

            var respuestasExistentes = _context.Respuestas
                .Where(r => r.IdAlumno == usuarioId &&
                            preguntasIds.Contains(r.IdPregunta));

            _context.Respuestas.RemoveRange(respuestasExistentes);
            _context.SaveChanges();

            foreach (var key in form.Keys)
            {
                if (key.StartsWith("pregunta_"))
                {
                    var valores = form[key];

                    if (valores.Count == 0)
                        continue;

                    int idPregunta = int.Parse(key.Replace("pregunta_", ""));

                    var opcionesCorrectas = _context.Opciones
                        .Where(o => o.IdPregunta == idPregunta && o.EsCorrecta == 1)
                        .Select(o => o.IdOpcion)
                        .ToList();

                    var seleccionadas = new List<int>();

                    foreach (var v in valores)
                    {
                        int idOpcion = int.Parse(v);

                        seleccionadas.Add(idOpcion);

                        var respuesta = new Respuesta
                        {
                            IdPregunta = idPregunta,
                            IdAlumno = usuarioId,
                            IdOpcion = idOpcion,
                            FechaRespuesta = DateTime.Now
                        };

                        _context.Respuestas.Add(respuesta);
                    }

                    bool esCorrecta =
                        seleccionadas.Count == opcionesCorrectas.Count &&
                        !seleccionadas.Except(opcionesCorrectas).Any();

                    if (esCorrecta)
                        correctas++;

                    total++;
                }
            }

            _context.SaveChanges();

            ViewBag.Total = total;
            ViewBag.Correctas = correctas;
            ViewBag.Porcentaje = total > 0 ? (correctas * 100) / total : 0;

            return View("Resultado");
        }



        [HttpPost]
        public IActionResult GuardarCuestionario([FromBody] CuestionarioBuilderViewModel model)
        {
            try
            {
                if (model == null || model.Preguntas == null || model.Preguntas.Count == 0)
                {
                    return Json(new { success = false, mensaje = "No hay preguntas" });
                }

                foreach (var p in model.Preguntas)
                {
                    if (string.IsNullOrEmpty(p.TextoPregunta))
                        continue;

                    var pregunta = new Pregunta
                    {
                        IdCuestionario = model.IdCuestionario,
                        TextoPregunta = p.TextoPregunta,
                        Tipo = p.Tipo
                    };

                    _context.Preguntas.Add(pregunta);
                    _context.SaveChanges();

                    if (p.Opciones != null)
                    {
                        foreach (var op in p.Opciones)
                        {
                            if (string.IsNullOrEmpty(op.Texto))
                                continue;

                            var opcion = new Opcion
                            {
                                IdPregunta = pregunta.IdPregunta,
                                Texto = op.Texto,
                                EsCorrecta = op.EsCorrecta ? 1:0
                            };

                            _context.Opciones.Add(opcion);
                        }

                        _context.SaveChanges();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, mensaje = "Error en servidor" });
            }
        }


        [HttpPost]
        public IActionResult Eliminar(int idActividad)
        {
            var actividad = _context.Actividades
                .FirstOrDefault(a => a.IdActividad == idActividad);

            if (actividad == null)
                return NotFound();

            int idCurso = actividad.IdCurso;

            var respuestas = _context.RespuestasActividad
                .Where(r => r.IdActividad == idActividad);
            _context.RespuestasActividad.RemoveRange(respuestas);

            var contenidos = _context.ActividadContenidos
                .Where(c => c.IdActividad == idActividad);
            _context.ActividadContenidos.RemoveRange(contenidos);

            if (actividad.Tipo == "CUESTIONARIO")
            {
                var cuestionario = _context.Cuestionarios
                    .FirstOrDefault(c => c.IdActividad == idActividad);

                if (cuestionario != null)
                {
                    var preguntas = _context.Preguntas
                        .Where(p => p.IdCuestionario == cuestionario.IdCuestionario)
                        .ToList();

                    foreach (var p in preguntas)
                    {
                        var opciones = _context.Opciones
                            .Where(o => o.IdPregunta == p.IdPregunta);

                        _context.Opciones.RemoveRange(opciones);
                    }

                    _context.Preguntas.RemoveRange(preguntas);
                    _context.Cuestionarios.Remove(cuestionario);
                }
            }

            _context.Actividades.Remove(actividad);

            _context.SaveChanges();

            return RedirectToAction("Foro", "Cursos", new { idCurso });
        }






        public IActionResult VerResultadoCuestionario(int idActividad)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            var actividad = _context.Actividades
                .FirstOrDefault(a => a.IdActividad == idActividad);

            if (actividad == null)
                return NotFound();

            var cuestionario = _context.Cuestionarios
                .FirstOrDefault(c => c.IdActividad == idActividad);

            if (cuestionario == null)
                return NotFound();

            var preguntas = _context.Preguntas
                .Where(p => p.IdCuestionario == cuestionario.IdCuestionario)
                .Include(p => p.Opciones)
                .ToList();

            var respuestas = _context.Respuestas
                .Where(r => r.IdAlumno == idUsuario)
                .ToList();

            int correctas = 0;
            int total = preguntas.Count;

            var preguntasVM = new List<PreguntaResultadoVM>();

            foreach (var p in preguntas)
            {
                var respuestasUsuario = respuestas
                    .Where(r => r.IdPregunta == p.IdPregunta)
                    .ToList();

                var seleccionadas = respuestasUsuario
                    .Select(r => r.IdOpcion)
                    .ToList();

                var opcionesCorrectas = p.Opciones
                    .Where(o => o.EsCorrecta == 1)
                    .Select(o => o.IdOpcion)
                    .ToList();

                bool esCorrecta =
                    seleccionadas.Count == opcionesCorrectas.Count &&
                    !seleccionadas.Except(opcionesCorrectas).Any();

                if (esCorrecta)
                    correctas++;

                preguntasVM.Add(new PreguntaResultadoVM
                {
                    TextoPregunta = p.TextoPregunta,

                    OpcionesSeleccionadas = seleccionadas,

                    Opciones = p.Opciones.Select(o => new OpcionResultadoVM
                    {
                        IdOpcion = o.IdOpcion,
                        Texto = o.Texto,
                        EsCorrecta = o.EsCorrecta == 1
                    }).ToList()
                });
            }

            var vm = new ResultadoCuestionarioVM
            {
                Correctas = correctas,
                Total = total,
                Porcentaje = total > 0 ? (correctas * 100) / total : 0,
                Preguntas = preguntasVM,
                IdCurso = actividad.IdCurso   
            };

            return View("ResultadoCuestionario", vm);
        }


        
    }
}
