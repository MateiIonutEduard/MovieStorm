using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MovieStorm.Services;
using MovieStorm.Models;
using MovieStorm.Data;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MovieStorm.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration Configuration;
        private readonly StormContext db;

        public HomeController(ILogger<HomeController> logger, IConfiguration Configuration)
        {
            _logger = logger;
            this.Configuration = Configuration;
            db = new StormContext(Configuration);
        }

        public IActionResult GetLatest()
        {
            var count = db.Movie.Count();
            int i = count - 4, j = count;

            var movies = (from movie in db.Movie.ToList()
                          orderby movie.name ascending
                          where movie.Id >= i && movie.Id <= j
                          let likes = db.Review.Where(r => r.movie_id == movie.Id)
                          select new
                          {
                              id = movie.Id,
                              name = movie.name,
                              preview = movie.preview,
                              rating = (likes.Count() != 0) ? likes.Average(v => v.rating) : 0
                          }).ToList();

            return Json(movies);
        }

        public IActionResult GetMovies(string filter)
        {
            if(string.IsNullOrEmpty(filter) || (!string.IsNullOrEmpty(filter) && filter == "all"))
            {
                var movies = (from movie in db.Movie.ToList()
                              orderby movie.name ascending
                              let likes = db.Review.Where(r => r.movie_id == movie.Id)
                              select new { 
                                  id = movie.Id,
                                  name = movie.name,
                                  preview = movie.preview,
                                  rating = (likes.Count() != 0) ? likes.Average(v => v.rating) : 0
                              }).ToList();

                return Json(movies);
            }
            else
            {
                if(filter == "views")
                {
                    var list = (from movie in db.Movie.ToList()
                                orderby movie.views descending
                                let likes = db.Review.Where(r => r.movie_id == movie.Id)
                                select new
                                {
                                    id = movie.Id,
                                    name = movie.name,
                                    preview = movie.preview,
                                    rating = (likes.Count() != 0) ? likes.Average(v => v.rating) : 0
                                }).ToList();

                    return Json(list);
                }
                else
                if(filter == "rating")
                {
                    var list = (from movie in db.Movie.ToList()
                                let likes = db.Review.Where(r => r.movie_id == movie.Id)
                                let avg = (likes.Count() != 0) ? likes.Average(v => v.rating) : 0
                                orderby avg descending
                                select new
                                {
                                    id = movie.Id,
                                    name = movie.name,
                                    preview = movie.preview,
                                    rating = avg
                                }).ToList();

                    return Json(list);
                }
                else
                {
                    var list = (from movie in db.Movie.ToList()
                                let likes = db.Review.Where(r => r.movie_id == movie.Id)
                                orderby movie.genre descending
                                select new
                                {
                                    id = movie.Id,
                                    name = movie.name,
                                    preview = movie.preview,
                                    rating = (likes.Count() != 0) ? likes.Average(v => v.rating) : 0
                                }).ToList();

                    return Json(list);
                }
            }
        }

        public IActionResult Show(string path)
        {
            int index = path.LastIndexOf('.');
            string ext = path.Substring(index + 1);
            byte[] buffer = System.IO.File.ReadAllBytes(path);
            return File(buffer, $"image/{ext}");
        }

        public IActionResult Watch(int id)
        {
            return PhysicalFile($"{id}", "application/octet-stream", enableRangeProcessing: true);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> AddMovie(string name, string genre, DateTime released, string description, IFormFile img, IFormFile buffer, int uid)
        {
            var exists = db.Movie.Where(m => m.name == name)
                        .FirstOrDefault();

            if (exists != null) return Forbid();
            if (img == null || buffer == null) return Forbid();

            var path = $"./Storage/{name}";
            Directory.CreateDirectory(path);

            FileStream fs = new FileStream($"{path}/{img.FileName}", FileMode.CreateNew);
            await img.CopyToAsync(fs);

            fs = new FileStream($"{path}/{buffer.FileName}", FileMode.CreateNew);
            await buffer.CopyToAsync(fs);

            var movie = new Movie
            {
                name = name,
                genre = genre,
                released = released,
                description = description,
                preview = $"{path}/{img.FileName}",
                user_id = uid,
                path = $"{path}/{buffer.FileName}"
            };

            db.Movie.Add(movie);
            await db.SaveChangesAsync();
            return Ok();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
