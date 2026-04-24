using Microsoft.AspNetCore.Mvc;
using LEARNINGUAM.Web.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LEARNINGUAM.Web.Controllers
{
    public class PreguntasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PreguntasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Confirmacion()
        {
            return View();
        } 

       
        
        public IActionResult Create(int idCuestionario, int idCurso)
        {
            var preguntasDB = _context.Preguntas
                .Where(p => p.IdCuestionario == idCuestionario)
                .ToList();

            var preguntasVM = new List<PreguntaViewModel>();

            foreach (var p in preguntasDB)
            {
                var opcionesDB = _context.Opciones
                    .Where(o => o.IdPregunta == p.IdPregunta)
                    .ToList();

                var opcionesVM = opcionesDB.Select(o => new OpcionViewModel
                {
                    Texto = o.Texto,
                    EsCorrecta = o.EsCorrecta == 1
                }).ToList();

                preguntasVM.Add(new PreguntaViewModel
                {
                    TextoPregunta = p.TextoPregunta,
                    Tipo = p.Tipo,
                    Opciones = opcionesVM
                });
            }

            var vm = new CuestionarioBuilderViewModel
            {
                IdCuestionario = idCuestionario,
                IdCurso = idCurso,
                Preguntas = preguntasVM
            };

            return View(vm);
        }



        [HttpPost]
        public IActionResult AgregarPregunta(CuestionarioBuilderViewModel vm)
        {
            var nuevaPregunta = new PreguntaViewModel
            {
                TextoPregunta = vm.TextoPregunta,
                Tipo = vm.Tipo,
                Opciones = vm.Opciones
            };

            vm.Preguntas.Add(nuevaPregunta);

            // Limpiar campos actuales
            vm.TextoPregunta = "";
            vm.Opciones = new List<OpcionViewModel>();

            return View("Create", vm);
        } 


        //POST para guardar
        [HttpPost]
        public IActionResult GuardarCuestionario([FromBody] CuestionarioRequest request)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var preguntasExistentes = _context.Preguntas
                    .Where(p => p.IdCuestionario == request.IdCuestionario)
                    .ToList();

                foreach (var p in preguntasExistentes)
                {
                    var opciones = _context.Opciones
                        .Where(o => o.IdPregunta == p.IdPregunta);

                    _context.Opciones.RemoveRange(opciones);
                }

                _context.Preguntas.RemoveRange(preguntasExistentes);
                _context.SaveChanges();

                foreach (var p in request.Preguntas)
                {
                    var pregunta = new Pregunta
                    {
                        TextoPregunta = p.TextoPregunta,
                        Tipo = p.Tipo,
                        IdCuestionario = request.IdCuestionario
                    };

                    _context.Preguntas.Add(pregunta);
                    _context.SaveChanges();

                    foreach (var op in p.Opciones)
                    {
                        var opcion = new Opcion
                        {
                            Texto = op.Texto,
                            EsCorrecta = op.EsCorrecta ? 1 : 0,
                            IdPregunta = pregunta.IdPregunta
                        };

                        _context.Opciones.Add(opcion);
                    }
                }

                _context.SaveChanges();

                transaction.Commit();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }




        public IActionResult CreateOpciones(int idPregunta)
        {
            var pregunta = _context.Preguntas
                .FirstOrDefault(p => p.IdPregunta == idPregunta);

            if (pregunta == null)
                return NotFound();


            ViewBag.IdCuestionario=pregunta.IdCuestionario;

            return View(pregunta);
        }



        [HttpPost]
        public IActionResult AgregarOpcion(int IdPregunta, string Texto, int? EsCorrecta)
        {
            var opcion = new Opcion
            {
                IdPregunta = IdPregunta,
                Texto = Texto,
                EsCorrecta = EsCorrecta == 1 ? 1 : 0
            };

            _context.Opciones.Add(opcion);
            _context.SaveChanges();

            return RedirectToAction("CreateOpciones", new { idPregunta = IdPregunta });
        } 



        [HttpPost]
        public IActionResult GuardarVF(int IdPregunta, string correcta)
        {
            _context.Opciones.Add(new Opcion
            {
                IdPregunta = IdPregunta,
                Texto = "Verdadero",
                EsCorrecta = correcta == "Verdadero" ? 1 : 0
            });

            _context.Opciones.Add(new Opcion
            {
                IdPregunta = IdPregunta,
                Texto = "Falso",
                EsCorrecta = correcta == "Falso" ? 1 : 0
            });

            _context.SaveChanges();

            return RedirectToAction("Create", new { idCuestionario = 
                _context.Preguntas.First(p => p.IdPregunta == IdPregunta).IdCuestionario });
        }

    }
}




