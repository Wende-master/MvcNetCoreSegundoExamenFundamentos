using Microsoft.AspNetCore.Http.HttpResults;
using MvcNetCoreSegundoExamenFundamentos.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MvcNetCoreSegundoExamenFundamentos.Repositories
{
    #region PROCEDIMIENTOS ALMACENADOS SQL SERVER

//    create procedure SP_INSERT_COMIC
//(@nombre nvarchar(50), @imagen nvarchar(400), @descripcion nvarchar(70))
//    as
//    	declare @nextId int --AUTOINCREMENTAR ID

//        select @nextId = max(IDCOMIC) + 1 from COMICS

//        insert into COMICS values(@nextId, @nombre, @imagen, @descripcion)
//    go
    #endregion
    public class RepositoryComicsSQLServer: IRepositoryComics
    {
        private DataTable tablaComics;
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader reader;
        public RepositoryComicsSQLServer()
        {
            string connectionString = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Password=MCSD2023";
            string sql = "select * from COMICS";
            this.connection = new SqlConnection(connectionString);
            this.command = new SqlCommand();
            this.command.Connection = this.connection;

            this.tablaComics = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(sql, this.connection);

            adapter.Fill(this.tablaComics);
        }

        public void DeleteComic(int idComic)
        {
            string sql = "delete from COMICS where IDCOMIC=@idcomic";
            this.command.Parameters.AddWithValue("@idcomic", idComic);
            this.command.CommandType = CommandType.Text;
            this.command.CommandText = sql;
            this.connection.Open();
            int result = this.command.ExecuteNonQuery(); 
            this.connection.Close();
            this.command.Parameters.Clear();
        }

        public Comic FindComic(int idComic)
        {
            string sql = "select * from COMICS where IDCOMIC=@idcomic";
            this.command.Parameters.AddWithValue("@idcomic", idComic);
            this.command.CommandType = CommandType.Text;
            this.command.CommandText = sql;
            this.connection.Open();
            this.reader = this.command.ExecuteReader();

            Comic comic = null;
            if(this.reader.Read())
            {
                comic = new Comic();
                comic.IdComic = int.Parse(this.reader["IDCOMIC"].ToString());
                comic.Nombre = this.reader["NOMBRE"].ToString();
                comic.Imagen = this.reader["IMAGEN"].ToString();
                comic.Descripcion = this.reader["DESCRIPCION"].ToString();
            }
            else
            {
                //NO HAY DATOS
            }
            this.reader.Close();
            this.connection.Close();
            this.command.Parameters.Clear();
            return comic;
        }

        public DatosComic GetComicsByName(string nombre)
        {
            var consulta = from datos in tablaComics.AsEnumerable()
                           where datos.Field<string>("NOMBRE") == nombre
                           select datos;
            if(consulta.Count() == 0)
            {
                return null;
            }
            else
            {
                List<Comic> comics = new List<Comic>();
                foreach(var row in consulta)
                {
                    Comic comic = new Comic
                    {
                        IdComic = row.Field<int>("IDCOMIC"),
                        Nombre = row.Field<string>("NOMBRE"),
                        Imagen = row.Field<string>("IMAGEN"),
                        Descripcion = row.Field<string>("DESCRIPCION")
                    };
                    comics.Add(comic);
                }
                DatosComic datosComic = new DatosComic
                {
                    Comics = comics
                };
                return datosComic;
            }
        }

        public List<string> GetComicsDatos()
        {
            var consulta = (from datos in this.tablaComics.AsEnumerable()
                           select datos.Field<string>("NOMBRE")).Distinct();
            
            List<string> comics = new List<string>();
            foreach(var comic in consulta)
            {
                comics.Add(comic);
            }
            return comics;
        }

        public List<Comic> GetComics()
        {
            var consulta = from datos in this.tablaComics.AsEnumerable()
                           select datos;
            List<Comic> comics = new List<Comic>();
            foreach(var row in consulta)
            {
                Comic comic = new Comic
                {
                    IdComic = row.Field<int>("IDCOMIC"),
                    Nombre = row.Field<string>("NOMBRE"),
                    Imagen = row.Field<string>("IMAGEN"),
                    Descripcion = row.Field<string>("DESCRIPCION")
                };
                comics.Add(comic);
            }
            return comics;
        }

        //INSERT SQL NORMAL
        public void InsertComic(string nombre, string imagen, string descripcion)
            
        {
            int maxId = (from datos in tablaComics.AsEnumerable()
                         select datos.Field<int>("IDCOMIC")).DefaultIfEmpty(0).Max();
            int nextId = maxId + 1;

            string sql = "insert into COMICS values(@idcomic, @nombre, @imagen, @descripcion)";
            this.command.Parameters.AddWithValue("@idcomic", nextId);
            this.command.Parameters.AddWithValue("@nombre", nombre);
            this.command.Parameters.AddWithValue("@imagen", imagen);
            this.command.Parameters.AddWithValue("@descripcion", descripcion);
            
            this.command.CommandType = CommandType.Text;
            this.command.CommandText = sql;
            this.connection.Open();
            int result = this.command.ExecuteNonQuery();
            this.connection.Close();
            this.command.Parameters.Clear();

        }

        //INSERT CON PROCEDIMIENTO
        public void InsertComicProcedure(string nombre, string imagen, string descripcion)
        {
            this.command.Parameters.AddWithValue("@nombre", nombre);
            this.command.Parameters.AddWithValue("@imagen", imagen);
            this.command.Parameters.AddWithValue("@descripcion", descripcion);

            this.command.CommandType = CommandType.StoredProcedure;
            this.command.CommandText = "SP_INSERT_COMIC";
            this.connection.Open();
            int result = this.command.ExecuteNonQuery();
            this.connection.Close();
            this.command.Parameters.Clear();
        }
    }
}
