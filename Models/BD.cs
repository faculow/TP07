using Dapper;
using Microsoft.Data.SqlClient;
using TP07.Models;

namespace TP07.Models;

public class BD{
    //Conexion a la base de datos
    private string _connectionString = @"Server=localhost;Database=TP07;
    Integrated Security=True;TrustServerCertificate=True;";


    public void AgregarUsuario(Usuarios usuario){
        string query = "INSERT INTO Usuarios (NombreUsuario, Contraseña, Nombre, Apellido, TipoUsuario) VALUES (@pNombreUsuario, @pContraseña, @pNombre, @pApellido, @pTipoUsuario)";
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Execute(query, new { pNombreUsuario = usuario.NombreUsuario, pContraseña = usuario.Contraseña, pNombre = usuario.Nombre, pApellido = usuario.Apellido, pTipoUsuario = usuario.TipoUsuario });
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

}