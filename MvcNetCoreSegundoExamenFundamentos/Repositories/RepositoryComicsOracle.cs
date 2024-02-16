using Microsoft.AspNetCore.Http.HttpResults;
using MvcNetCoreSegundoExamenFundamentos.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MvcNetCoreSegundoExamenFundamentos.Repositories
{
    #region PROCEDIMIENTO ALMACENADO ORACLE
    //    create or replace procedure SP_DELETE_COMIC
    //(p_idcomic COMICS.IDCOMIC%TYPE)
    //as
    //begin
    //     delete from COMICS where IDCOMIC = p_idcomic;
    //    commit;
    //   end;

//    CREATE OR REPLACE PROCEDURE SP_INSERT_COMIC(
//    p_nombre COMICS.NOMBRE%TYPE,
//    p_imagen COMICS.IMAGEN%TYPE,
//    p_descripcion COMICS.DESCRIPCION%TYPE
//) AS
//    p_nextId COMICS.IDCOMIC%TYPE;

//BEGIN
//    SELECT MAX(IDCOMIC) + 1 INTO p_nextId FROM COMICS;
//    IF p_nextId IS NULL THEN
//        p_nextId := 1;
//    END IF;

//    INSERT INTO COMICS VALUES(p_nextId, p_nombre, p_imagen, p_descripcion);
//    COMMIT;
//END;

    #endregion 
    public class RepositoryComicsOracle: IRepositoryComics
    {
        private DataTable tablaComics;
        private OracleConnection connection;
        private OracleCommand command;
        private OracleDataReader reader;

        public RepositoryComicsOracle()
        {
            string connectionString = @"Data Source=LOCALHOST:1521/XE; Persist Security Info=True; User Id=SYSTEM; Password=oracle";
            string sql = "select * from COMICS";
            this.connection = new OracleConnection(connectionString);
            this.command = new OracleCommand();
            this.command.Connection = this.connection;

            this.tablaComics = new DataTable();
            OracleDataAdapter adapter = new OracleDataAdapter(sql, this.connection);

            adapter.Fill(this.tablaComics);
        }

        public void DeleteComic(int idComic)
        {
            OracleParameter pamId = new OracleParameter(":p_idcomic", idComic);
            this.command.Parameters.Add(pamId);
            this.command.CommandType = CommandType.StoredProcedure;
            this.command.CommandText = "SP_DELETE_COMIC";
            this.connection.Open();
            int result = this.command.ExecuteNonQuery();
            this.connection.Close();
            this.command.Parameters.Clear();
        }

        public Comic FindComic(int idComic)
        {
            string sql = "select * from COMICS where IDCOMIC=:p_idcomic";
            OracleParameter pamId = new OracleParameter(":p_idcomic", idComic);
            this.command.Parameters.Add(pamId);
            this.command.CommandType = CommandType.Text;
            this.command.CommandText = sql;
            this.connection.Open();
            this.reader = this.command.ExecuteReader();

            Comic comic = null;
            if (this.reader.Read())
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
            if (consulta.Count() == 0)
            {
                return null;
            }
            else
            {
                List<Comic> comics = new List<Comic>();
                foreach (var row in consulta)
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
            foreach (var comic in consulta)
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
            foreach (var row in consulta)
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

        public void InsertComic(string nombre, string imagen, string descripcion)
        {
            int maxId = (from datos in tablaComics.AsEnumerable()
                         select datos.Field<int>("IDCOMIC")).DefaultIfEmpty(0).Max();
            int nextId = maxId + 1;

            string sql = "insert into COMICS values(:idcomic, :nombre, :imagen, :descripcion)";
            OracleParameter pamId = new OracleParameter(":idcomic", nextId);
            this.command.Parameters.Add(pamId);

            OracleParameter pamNombre = new OracleParameter(":nombre", nombre);
            this.command.Parameters.Add(pamNombre);

            OracleParameter pamImagen = new OracleParameter(":imagen", imagen);
            this.command.Parameters.Add(pamImagen);

            OracleParameter pamDescripcion = new OracleParameter(":descripcion", descripcion);
            this.command.Parameters.Add(pamDescripcion);

            this.command.CommandType = CommandType.Text;
            this.command.CommandText = sql;
            this.connection.Open();
            int result = this.command.ExecuteNonQuery();
            this.connection.Close();
            this.command.Parameters.Clear();
        }
        public void InsertComicProcedure(string nombre, string imagen, string descripcion)
        {
            //this.command.Parameters.Add(":p_nombre", nombre);
            //this.command.Parameters.Add(":p_imagen", imagen);
            //this.command.Parameters.Add(":p_descripcion", descripcion);

            OracleParameter pamNombre = new OracleParameter(":nombre", nombre);
            this.command.Parameters.Add(pamNombre);

            OracleParameter pamImagen = new OracleParameter(":imagen", imagen);
            this.command.Parameters.Add(pamImagen);

            OracleParameter pamDescripcion = new OracleParameter(":descripcion", descripcion);
            this.command.Parameters.Add(pamDescripcion);

            this.command.CommandType = CommandType.StoredProcedure;
            this.command.CommandText = "SP_INSERT_COMIC";
            this.connection.Open();
            int result = this.command.ExecuteNonQuery();
            this.connection.Close();
            this.command.Parameters.Clear();
        }
    }
}
