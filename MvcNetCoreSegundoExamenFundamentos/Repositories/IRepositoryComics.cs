using MvcNetCoreSegundoExamenFundamentos.Models;

namespace MvcNetCoreSegundoExamenFundamentos.Repositories
{
    public interface IRepositoryComics
    {
        List<Comic> GetComics();

        DatosComic GetComicsByName(string nombre);
        List<string> GetComicsDatos();
        void InsertComic(string nombre, string imagen, string descripcion);
        void InsertComicProcedure(string nombre, string imagen, string descripcion);

        void DeleteComic(int idComic);

        Comic FindComic(int idComic);

    }
}
