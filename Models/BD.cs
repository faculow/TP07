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

    public Usuarios ObtenerUsuarioPorNombre(string nombreUsuario){
        Usuarios usuario = null;
        string query = "SELECT TOP 1 * FROM Usuarios WHERE NombreUsuario = @pNombreUsuario";
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            usuario = connection.QueryFirstOrDefault<Usuarios>(query, new { pNombreUsuario = nombreUsuario });
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
        List<Publicaciones> lista = new List<Publicaciones>();
        int limite = 10;
        int offset = (pagina - 1) * limite;

        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Publicaciones ORDER BY FechaPublicacion DESC " +
                        "OFFSET @offset ROWS FETCH NEXT @limite ROWS ONLY";
            lista = db.Query<Publicaciones>(sql, new { offset, limite }).ToList();
        }
        return lista;
    }

    public int ObtenerCantidadMeGusta(int idPublicacion)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT COUNT(*) FROM PublicacionesMeGusta WHERE IdPublicacion = @idPublicacion";
            return db.ExecuteScalar<int>(sql, new { idPublicacion });
        }
    }

    public int DarMeGusta(int idPublicacion, int idUsuario)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sqlInsert = "INSERT INTO PublicacionesMeGusta (IdPublicacion, IdUsuario) VALUES (@idPublicacion, @idUsuario)";
            db.Execute(sqlInsert, new { idPublicacion, idUsuario });

            string sqlCount = "SELECT COUNT(*) FROM PublicacionesMeGusta WHERE IdPublicacion = @idPublicacion";
            return db.ExecuteScalar<int>(sqlCount, new { idPublicacion });
        }
    }

    public List<Comentarios> ObtenerComentarios(int idPublicacion)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Comentarios WHERE IdPublicacion = @idPublicacion";
            return db.Query<Comentarios>(sql, new { idPublicacion }).ToList();
        }
    }

    public void AgregarComentario(int idPublicacion, int idUsuario, string contenido)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "INSERT INTO Comentarios (IdPublicacion, IdUsuario, Contenido) VALUES (@idPublicacion, @idUsuario, @contenido)";
            db.Execute(sql, new { idPublicacion, idUsuario, contenido });
        }
    }

}