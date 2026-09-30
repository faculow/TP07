using Dapper;
using Microsoft.Data.SqlClient;
using TP07.Models;

namespace TP07.Models;

public class BD{
    
    private string _connectionString = @"Server=localhost;Database=TP07;
    Integrated Security=True;TrustServerCertificate=True;";


    public void AgregarUsuario(Usuarios usuario){
        string query = "INSERT INTO Usuarios (NombreUsuario, Contraseña, Nombre, Apellido) VALUES (@pNombreUsuario, @pContraseña, @pNombre, @pApellido)";
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Execute(query, new { pNombreUsuario = usuario.NombreUsuario, pContraseña = usuario.Contraseña, pNombre = usuario.Nombre, pApellido = usuario.Apellido});
        }
    }

    public void AgregarPublicacion(Publicaciones publicacion){
        string query = "INSERT INTO Publicaciones (IdUsuario, Titulo, Descripcion, Imagen, FechaPublicacion) VALUES (@pIdUsuario, @pTitulo, @pDescripcion, @pImagen, @pFechaPublicacion)";
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Execute(query, new {
                pIdUsuario = publicacion.IdUsuario,
                pTitulo = publicacion.Titulo,
                pDescripcion = publicacion.Descripcion,
                pImagen = publicacion.Imagen,
                pFechaPublicacion = publicacion.FechaPublicacion
            });
        }
    }

    public List<Usuarios> ObtenerUsuarios(){
        List<Usuarios> usuarios = new List<Usuarios>();
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            string query = "SELECT * FROM Usuarios";
            usuarios = connection.Query<Usuarios>(query).ToList();
        }
        return usuarios;
    }

    public Usuarios ObtenerUsuarioPorId(int Id){
        Usuarios usuario = null;
        string query = "SELECT * FROM Usuarios WHERE Id = @pId";
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            usuario = connection.QueryFirstOrDefault<Usuarios>(query, new { pId = Id });
        }
        return usuario;
    }

    

    public Usuarios ObtenerUsuario(string nombreUsuario, string contrasena){
        Usuarios usuario = null;
        string query = "SELECT TOP 1 * FROM Usuarios WHERE NombreUsuario = @pNombreUsuario AND Contraseña = @pContraseña";
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            usuario = connection.QueryFirstOrDefault<Usuarios>(query, new { pNombreUsuario = nombreUsuario, pContraseña = contrasena });
        }
        return usuario;
    }

    public List<Publicaciones> ObtenerPublicacionesRecientes(){
        List<Publicaciones> publicaciones = new List<Publicaciones>();
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            string query = @"SELECT p.Id, p.IdUsuario, p.Titulo, p.Descripcion, p.Imagen, p.FechaPublicacion,
                            u.NombreUsuario
                            FROM Publicaciones p
                            INNER JOIN Usuarios u ON u.Id = p.IdUsuario
                            ORDER BY p.FechaPublicacion DESC";
            publicaciones = connection.Query<Publicaciones>(query).ToList();
        }
        return publicaciones;
    }

    public List<Publicaciones> ObtenerPublicacionesPorUsuario(int idUsuario){
        List<Publicaciones> publicaciones = new List<Publicaciones>();
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            string query = "SELECT * FROM Publicaciones WHERE IdUsuario = @pIdUsuario ORDER BY FechaPublicacion DESC";
            publicaciones = connection.Query<Publicaciones>(query, new { pIdUsuario = idUsuario }).ToList();
        }
        return publicaciones;
    }
    
    public int DarMeGusta(int idPublicacion)
    {
        int likesActuales = 0;
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "UPDATE Publicaciones SET MeGusta = ISNULL(MeGusta, 0) + 1 WHERE Id = @id; " +
                        "SELECT MeGusta FROM Publicaciones WHERE Id = @id;";
            likesActuales = db.ExecuteScalar<int>(sql, new { id = idPublicacion });
        }
        return likesActuales;
    }

    public void AgregarComentario(int idPublicacion, string contenido)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "INSERT INTO Comentarios (IdPublicacion, Contenido) VALUES (@idPublicacion, @contenido)";
            db.Execute(sql, new { idPublicacion, contenido });
        }
    }

    public List<Publicaciones> ObtenerPublicacionesPaginadas(int pagina)
    {
        int registrosPorPagina = 10;
    int offset = (pagina - 1) * registrosPorPagina;

    using (SqlConnection db = new SqlConnection(_connectionString))
    {
        string sql = @"SELECT Id, IdUsuario, Titulo, Descripcion, Imagen, FechaPublicacion 
                       FROM Publicaciones 
                       ORDER BY FechaPublicacion DESC 
                       OFFSET @offset ROWS FETCH NEXT @registrosPorPagina ROWS ONLY";

        return db.Query<Publicaciones>(sql, new { offset, registrosPorPagina }).ToList();
    }
    }

    // Obtener cantidad de Likes de una publicación
public int ObtenerCantidadMeGusta(int idPublicacion)
{
    using (SqlConnection db = new SqlConnection(_connectionString))
    {
        string sql = "SELECT COUNT(*) FROM PublicacionesMeGusta WHERE [IdPublicación] = @idPublicacion";
        return db.ExecuteScalar<int>(sql, new { idPublicacion });
    }
}

// Dar/Quitar Me Gusta
public int DarMeGusta(int idPublicacion, int idUsuario)
{
    using (SqlConnection db = new SqlConnection(_connectionString))
    {
        string sqlExiste = "SELECT COUNT(*) FROM PublicacionesMeGusta WHERE [IdPublicación] = @idPublicacion AND IdUsuario = @idUsuario";
        int existe = db.ExecuteScalar<int>(sqlExiste, new { idPublicacion, idUsuario });

        if (existe == 0)
        {
            string sqlInsert = "INSERT INTO PublicacionesMeGusta ([IdPublicación], IdUsuario) VALUES (@idPublicacion, @idUsuario)";
            db.Execute(sqlInsert, new { idPublicacion, idUsuario });
        }
        else
        {
            string sqlDelete = "DELETE FROM PublicacionesMeGusta WHERE [IdPublicación] = @idPublicacion AND IdUsuario = @idUsuario";
            db.Execute(sqlDelete, new { idPublicacion, idUsuario });
        }

        string sqlCount = "SELECT COUNT(*) FROM PublicacionesMeGusta WHERE [IdPublicación] = @idPublicacion";
        return db.ExecuteScalar<int>(sqlCount, new { idPublicacion });
    }
}


    public List<Comentarios> ObtenerComentarios(int idPublicacion)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT Id, IdPublicacion, IdUsuarioComenta, Texto, FechaComentario FROM Comentarios WHERE IdPublicacion = @idPublicacion";
            return db.Query<Comentarios>(sql, new { idPublicacion }).ToList();
        }
    }

    public void AgregarComentario(int idPublicacion, int idUsuario, string contenido)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "INSERT INTO Comentarios (IdPublicacion, IdUsuarioComenta, Texto, FechaComentario) VALUES (@idPublicacion, @idUsuario, @contenido, GETDATE())";
            db.Execute(sql, new { idPublicacion, idUsuario, contenido });
        }
    }

    public Usuarios ObtenerUsuarioPorNombre(string usuario)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = @usuario OR Nombre = @usuario";
            return db.QueryFirstOrDefault<Usuarios>(sql, new { usuario });
        }
    }
    public List<int> ObtenerLikesDelUsuario(int idUsuario)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT [IdPublicación] FROM PublicacionesMeGusta WHERE IdUsuario = @idUsuario";
            return db.Query<int>(sql, new { idUsuario }).ToList();
        }
    }
}