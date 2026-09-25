using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP07.Models;

namespace TP07.Controllers;

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
        string requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        ViewBag.RequestId = requestId;
        ViewBag.ShowRequestId = !string.IsNullOrEmpty(requestId);
        return View();
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

        return RedirectToAction("RedSocial");
    }

    public IActionResult Registrarse(){
        return View();
    }
    [HttpPost]
    public IActionResult Registrarse(string nombre, string apellido, string usuario, string contrasena){
        if (nombre == null || nombre.Length < 2 ||
            apellido == null || apellido.Length < 2 ||
            usuario == null || usuario.Length < 4 ||
            contrasena == null || contrasena.Length < 6){
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
        };

        bd.AgregarUsuario(nuevoUsuario);

        HttpContext.Session.SetString("usuario", nuevoUsuario.NombreUsuario);
        HttpContext.Session.SetString("nombre", nuevoUsuario.Nombre);
        HttpContext.Session.SetString("apellido", nuevoUsuario.Apellido);

        return RedirectToAction("RedSocial");
    }

    public IActionResult RedSocial(){
        string usuario = HttpContext.Session.GetString("usuario");
    if (usuario == null) return RedirectToAction("Login");

    BD bd = new BD();
    Usuarios usuarioActual = bd.ObtenerUsuarioPorNombre(usuario);
    if (usuarioActual == null) return RedirectToAction("Login");

    ViewBag.Id = usuarioActual.Id;
    ViewBag.Nombre = usuarioActual.Nombre;

    List<Publicaciones> publicaciones = bd.ObtenerPublicacionesPaginadas(1);

    Dictionary<int, int> likes = new Dictionary<int, int>();
    Dictionary<int, List<Comentarios>> comentarios = new Dictionary<int, List<Comentarios>>();

    foreach (var publicacion in publicaciones)
    {
        likes[publicacion.Id] = bd.ObtenerCantidadMeGusta(publicacion.Id);
        comentarios[publicacion.Id] = bd.ObtenerComentarios(publicacion.Id);
    }

    ViewBag.Publicaciones = publicaciones;
    ViewBag.Likes = likes;
    ViewBag.Comentarios = comentarios;

    return View();
    }

    public IActionResult CrearPublicacion(){
        string usuario = HttpContext.Session.GetString("usuario");

        if (string.IsNullOrEmpty(usuario)){
            return RedirectToAction("Login");
        }

        BD bd = new BD();
        Usuarios usuarioActual = bd.ObtenerUsuarioPorNombre(usuario);

        if (usuarioActual == null){
            return RedirectToAction("Login");
        }

        ViewBag.UsuarioActual = usuarioActual.NombreUsuario;
        ViewBag.IdUsuario = usuarioActual.Id;
        ViewBag.Titulo = string.Empty;
        ViewBag.Descripcion = string.Empty;

        return View();
    }

    [HttpPost]
    public IActionResult CrearPublicacion(Publicaciones publicacion){
        string usuario = HttpContext.Session.GetString("usuario");

        if (string.IsNullOrEmpty(usuario)){
            return RedirectToAction("Login");
        }

        BD bd = new BD();
        Usuarios usuarioActual = bd.ObtenerUsuarioPorNombre(usuario);

        if (usuarioActual == null){
            return RedirectToAction("Login");
        }

        if (string.IsNullOrWhiteSpace(publicacion.Titulo) || string.IsNullOrWhiteSpace(publicacion.Descripcion)){
            ViewBag.Error = "Completa el título y la descripción de la publicación.";
            ViewBag.UsuarioActual = usuarioActual.NombreUsuario;
            ViewBag.IdUsuario = usuarioActual.Id;
            return View();
        }

        if (publicacion.ImagenArchivo == null || publicacion.ImagenArchivo.Length == 0){
            ViewBag.Error = "Debes seleccionar una imagen para la publicación.";
            ViewBag.UsuarioActual = usuarioActual.NombreUsuario;
            ViewBag.IdUsuario = usuarioActual.Id;
            return View();
        }

        string nombreArchivo = DateTime.Now.Ticks + "_" + publicacion.ImagenArchivo.FileName;
        string rutaArchivo = "wwwroot/uploads/" + nombreArchivo;

        using (Stream entrada = publicacion.ImagenArchivo.OpenReadStream())
        using (Stream salida = System.IO.File.Create(rutaArchivo))
        {
            int dato = 0;

            while ((dato = entrada.ReadByte()) != -1)
            {
                salida.WriteByte((byte)dato);
            }
        }

        publicacion.IdUsuario = usuarioActual.Id;
        publicacion.FechaPublicacion = DateTime.Now;
        publicacion.Imagen = "/uploads/" + nombreArchivo;

        bd.AgregarPublicacion(publicacion);
        return RedirectToAction("RedSocial");
    }

    [HttpPost]
    public IActionResult CerrarSesion(){
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DarMeGusta(int id)
    {
        BD bd = new BD();
        int meGustaActuales = bd.DarMeGusta(id); 

        return Json(new { meGusta = meGustaActuales });
    }

    [HttpPost]
    public IActionResult Comentar(int id, string contenido)
    {
        BD bd = new BD();
        bd.AgregarComentario(id, contenido);

        return Json(new { contenido = contenido });
    }

    public IActionResult ObtenerPublicaciones(int pagina)
    {
        BD bd = new BD();
        List<Publicaciones> lista = bd.ObtenerPublicacionesPaginadas(pagina);

        return Json(lista);
    }

}
