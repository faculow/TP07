using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP_05.Models;

namespace TP_05.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger){
        _logger = logger;
    }
    public IActionResult Privacy(){
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(){
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    public IActionResult Index(){
        return View();
    }
    public IActionResult Login(){
        return View();
    }
    [HttpPost]
    public IActionResult Login(string usuario, string contrasena){

        if (usuario == null || usuario.Length < 4 ||
            contrasena == null || contrasena.Length < 6){
            ViewBag.Error = "El usuario debe tener al menos 4 caracteres y la contraseña 6.";
            return View();
        }

        BD bd = new BD();
        Usuarios usuarioEncontrado = bd.ObtenerUsuario(usuario, contrasena);

        if (usuarioEncontrado == null){
            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }

        HttpContext.Session.SetString("usuario", usuarioEncontrado.NombreUsuario);
        HttpContext.Session.SetString("nombre", usuarioEncontrado.Nombre);
        HttpContext.Session.SetString("apellido", usuarioEncontrado.Apellido);
        HttpContext.Session.SetString("tipoUsuario", usuarioEncontrado.TipoUsuario);

        return RedirectToAction("RedSocial");
    }

    public IActionResult Registrarse(){
        return View();
    }
    [HttpPost]
    public IActionResult Registrarse(string nombre, string apellido, string usuario, string contrasena, string tipoUsuario){
        if (nombre == null || nombre.Length < 2 ||
            apellido == null || apellido.Length < 2 ||
            usuario == null || usuario.Length < 4 ||
            contrasena == null || contrasena.Length < 6 ||
            tipoUsuario == null || tipoUsuario.Length == 0){
            ViewBag.Error = "Complete los campos con datos válidos: nombre y apellido de al menos 2 caracteres, usuario de 4 y contraseña de 6.";
            return View();
        }

        BD bd = new BD();
        Usuarios usuarioExistente = bd.ObtenerUsuarioPorNombre(usuario);

        if (usuarioExistente != null){
            ViewBag.Error = "El nombre de usuario ya existe.";
            return View();
        }

        Usuarios nuevoUsuario = new Usuarios{
            Nombre = nombre,
            Apellido = apellido,
            NombreUsuario = usuario,
            Contraseña = contrasena,
            TipoUsuario = tipoUsuario
        };

        bd.AgregarUsuario(nuevoUsuario);
        return RedirectToAction("Login");
    }

    public IActionResult RedSocial(){
        string usuario = HttpContext.Session.GetString("usuario");

        if (usuario == null){
           return RedirectToAction("Login");
        }

        BD bd = new BD();
        Usuarios usuarioActual = bd.ObtenerUsuarioPorNombre(usuario);

        if (usuarioActual == null){
            return RedirectToAction("Login");
        }

        ViewBag.Usuario = usuarioActual.Nombre;
        return View(usuarioActual);
    }

    [HttpPost]
    public IActionResult CerrarSesion(){
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

}
