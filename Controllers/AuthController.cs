using Microsoft.AspNetCore.Mvc;
using LEARNINGUAM.Web.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Login", "Auth");
    }

    public IActionResult LoginGoogle()
    {
        var redirectUrl = Url.Action("GoogleResponse", "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return Challenge(properties, "Google");
    }

    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync("Cookies");

        var email = result.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var nombre = result.Principal.Identity.Name;

        // Buscar usuario en tu BD
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == email);

        if (usuario == null)
        {
            usuario = new Usuario
            {
                Nombre = nombre,
                Correo = email
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);

        return RedirectToAction("Index", "Cursos");
    }



    [HttpPost]
    public IActionResult Login(string correo, string password)
    {
        Console.WriteLine("---- LOGIN DEBUG ----");
        Console.WriteLine("Correo recibido: [" + correo + "]");
        Console.WriteLine("Password recibido: [" + password + "]");

        var usuarios = _context.Usuarios.ToList();

        foreach (var u in usuarios)
        {
            Console.WriteLine("BD -> [" + u.Correo + "] | [" + u.Contraseña + "]");
        }

        correo = correo?.Trim().ToLower();
        password = password?.Trim();

        var usuario = usuarios
            .FirstOrDefault(u => u.Correo.Trim().ToLower() == correo
                            && u.Contraseña.Trim() == password);

        if (usuario == null)
        {
            Console.WriteLine(" NO MATCH");
            ViewBag.Error = "Correo o contraseña incorrectos";
            return View();
        }

        Console.WriteLine(" MATCH: " + usuario.Nombre);

        HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);

        return RedirectToAction("Index", "Cursos");
    }
}
