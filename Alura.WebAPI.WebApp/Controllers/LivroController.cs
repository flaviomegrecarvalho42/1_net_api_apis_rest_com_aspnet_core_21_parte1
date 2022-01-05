using System.Linq;
using Alura.ListaLeitura.Persistencia;
using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class LivroController : Controller
    {
        private readonly IRepository<Livro> _repository;

        public LivroController(IRepository<Livro> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Novo()
        {
            return View(new LivroUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Novo(LivroUpload livroUploadModel)
        {
            if (ModelState.IsValid)
            {
                _repository.Incluir(livroUploadModel.ToLivro());
                return RedirectToAction("Index", "Home");
            }

            return View(livroUploadModel);
        }

        [HttpGet]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repository.All
                                    .Where(l => l.Id == id)
                                    .Select(l => l.ImagemCapa)
                                    .FirstOrDefault();

            if (img != null)
            {
                return File(img, "image/png");
            }

            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            var livroModel = _repository.Find(id);

            if (livroModel == null)
            {
                return NotFound();
            }

            return View(livroModel.ToLivroUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detalhes(LivroUpload livroUploadModel)
        {
            if (ModelState.IsValid)
            {
                var livro = livroUploadModel.ToLivro();

                if (livroUploadModel.Capa == null)
                {
                    livro.ImagemCapa = _repository.All
                                                  .Where(l => l.Id == livro.Id)
                                                  .Select(l => l.ImagemCapa)
                                                  .FirstOrDefault();
                }

                _repository.Alterar(livro);
                return RedirectToAction("Index", "Home");
            }

            return View(livroUploadModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remover(int id)
        {
            var livroModel = _repository.Find(id);

            if (livroModel == null)
            {
                return NotFound();
            }

            _repository.Excluir(livroModel);
            return RedirectToAction("Index", "Home");
        }
    }
}