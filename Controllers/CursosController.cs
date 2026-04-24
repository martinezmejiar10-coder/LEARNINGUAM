using Microsoft.AspNetCore.Mvc;
using LEARNINGUAM.Web.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;

    public CursosController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    public IActionResult Index()
    {
        var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

        if (idUsuario == null)
            return RedirectToAction("Login", "Auth");

        var listaProfesor = (
            from c in _context.Cursos
            join i in _context.InscribirCurso
                on c.IdCurso equals i.IdCurso
            where i.IdUsuario == idUsuario && i.IdRol == 3
            select c
        ).ToList();

        var listaAlumno = (
            from c in _context.Cursos
            join i in _context.InscribirCurso
                on c.IdCurso equals i.IdCurso
            where i.IdUsuario == idUsuario && i.IdRol == 4
            select c
        ).ToList();

        ViewBag.CursosProfesor = listaProfesor;
        ViewBag.CursosAlumno = listaAlumno;

        return View();
    }

    
    public IActionResult Foro(int idCurso)
    {
        int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

        var curso = _context.Cursos
            .FirstOrDefault(c => c.IdCurso == idCurso);

        if (curso == null)
            return NotFound();

        var rol = _context.InscribirCurso
            .Where(i => i.IdCurso == idCurso && i.IdUsuario == idUsuario)
            .Select(i => i.IdRol)
            .FirstOrDefault();

        if (rol == 0)
            return Unauthorized();

        bool esProfesor = rol == 3;
        bool esAlumno = rol == 4;

        var actividades = _context.Actividades
            .Where(a => a.IdCurso == idCurso)
            .OrderByDescending(a => a.FechaCreacion)
            .ToList();

        var anuncios = _context.Anuncios
            .Where(a => a.IdCurso == idCurso)
            .OrderByDescending(a => a.Fecha)
            .ToList();

        ViewBag.Curso = curso;
        ViewBag.EsProfesor = esProfesor;
        ViewBag.EsAlumno = esAlumno;

        ViewBag.Anuncios = anuncios;

        return View(actividades);
    }

    
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Curso curso)
    {
        int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

        curso.IdProfesor = idUsuario;

        _context.Cursos.Add(curso);
        _context.SaveChanges();

       
        var inscripcion = new InscribirCurso
        {
            IdUsuario = idUsuario,
            IdCurso = curso.IdCurso,
            IdRol = 3 // Maestro
        };

        _context.InscribirCurso.Add(inscripcion);
        _context.SaveChanges();

        TempData["Codigo"] = curso.CodigoAcceso;

        return RedirectToAction("Index");
    }

    
    public IActionResult Unirse()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Unirse(string codigo)
    {
        int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

        var curso = _context.Cursos
            .FirstOrDefault(c => c.CodigoAcceso == codigo);

        if (curso == null)
        {
            ViewBag.Error = "Código incorrecto";
            return View();
        }

        
        bool yaInscrito = _context.InscribirCurso
            .Count(i => i.IdUsuario == idUsuario && i.IdCurso == curso.IdCurso) > 0;

        if (yaInscrito)
        {
            ViewBag.Error = "Ya estás inscrito en este curso";
            return View();
        }

        
        var inscripcion = new InscribirCurso
        {
            IdUsuario = idUsuario,
            IdCurso = curso.IdCurso,
            IdRol = 4 // Alumno
        };

        _context.InscribirCurso.Add(inscripcion);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    
    public IActionResult Delete(int id)
    {
        var curso = _context.Cursos
            .FirstOrDefault(c => c.IdCurso == id);

        if (curso == null)
            return NotFound();

        int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

        if (curso.IdProfesor != idUsuario)
            return Unauthorized();

        return View(curso); 
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int IdCurso)
    {
        int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

        var curso = _context.Cursos
            .FirstOrDefault(c => c.IdCurso == IdCurso);

        if (curso == null)
            return NotFound();

        if (curso.IdProfesor != idUsuario)
            return Unauthorized();


        _context.Cursos.Remove(curso);
        _context.SaveChanges();

        TempData["Mensaje"] = $"El curso '{curso.NombreCurso}' ha sido eliminado correctamente";

        return RedirectToAction("Index");
    }
    



}