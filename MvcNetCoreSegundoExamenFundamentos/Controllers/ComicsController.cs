using Microsoft.AspNetCore.Mvc;
using MvcNetCoreSegundoExamenFundamentos.Models;
using MvcNetCoreSegundoExamenFundamentos.Repositories;

namespace MvcNetCoreSegundoExamenFundamentos.Controllers
{
    public class ComicsController : Controller
    {
        private IRepositoryComics repo;
        public ComicsController(IRepositoryComics repo)
        {
            this.repo = repo;
        }
        public IActionResult Index()
        {
            List<Comic> comics = this.repo.GetComics();
            return View(comics);
        }

        public IActionResult Details(int idcomic)
        {
            Comic comic = this.repo.FindComic(idcomic);
            return View(comic);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Comic comic)
        {
            this.repo.InsertComic(
               comic.Nombre, comic.Imagen, comic.Descripcion
                );
            return RedirectToAction("Index");   
        }

        public IActionResult CreateWithProcedure()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateWithProcedure(Comic comic)
        {
            this.repo.InsertComicProcedure(
                comic.Nombre, comic.Imagen, comic.Descripcion
                );
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int idcomic)
        {
             this.repo.DeleteComic(idcomic);
            return RedirectToAction("Index");   
        }

        public IActionResult DeleteForm(int idcomic)
        {
            Comic comic = this.repo.FindComic(idcomic);
            return View(comic);
        }
        [HttpPost]
        public IActionResult DeleteForm(Comic comic)
        {
            this.repo.DeleteComic(comic.IdComic);
            return RedirectToAction("Index");
        }

        public IActionResult FindComicByName()
        {
            ViewData["COMICS"] = this.repo.GetComicsDatos();
            return View();
        }
        [HttpPost]
        public IActionResult FindComicByName(string nombre)
        {
            ViewData["COMICS"] = this.repo.GetComicsDatos();
            DatosComic datosComic = this.repo.GetComicsByName(nombre);
            return View(datosComic);
        }
    }
}
