using System;
using System.IO;
using System.Text;
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
using Microsoft.AspNetCore.Authorization;

namespace MovieStorm.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration Configuration;
        private readonly StormContext db;

        public HomeController(ILogger<HomeController> logger, StormContext _context, IConfiguration Configuration)
        {
            _logger = logger;
            this.Configuration = Configuration;
            db = _context;
        }

        public IActionResult GetLangs()
        {
            byte[] data = System.IO.File.ReadAllBytes(@"./Storage/data.json");
            return File(data, "application/json");
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

        public IActionResult GetGenre(int id)
        {
            var model = db.Movie.FirstOrDefault(m => m.Id == id);
            var list = db.Movie.Where(m => m.genre == model.genre);

            var count = list.Count();
            if (count > 4)
            {
                int i = count - 4, j = count;

                var movies = (from movie in list
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
            else
            {
                var movies = (from movie in list
                              orderby movie.name ascending
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
        }

        public IActionResult WatchMovie()
        {
            return View();
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
            var movie = db.Movie.FirstOrDefault(m => m.Id == id);
            var path = Path.GetFullPath(movie.path);
            return PhysicalFile($"{path}", "application/octet-stream", enableRangeProcessing: true);
        }

        public IActionResult NameOf(int id)
        {
            var movie = db.Movie.FirstOrDefault(m => m.Id == id);
            var likes = db.Review.Where(r => r.movie_id == movie.Id);
            var rating = (likes.Count() != 0) ? likes.Average(v => v.rating) : 0;
            return Json(new { name = movie.name, details = movie.description, rating });
        }

        public IActionResult GetReviews(int id)
        {
            var reviews = (from r in db.Review.ToList()
                           join u in db.User.ToList() on r.user_id equals u.Id
                           where r.movie_id == id
                           select new { id = r.Id, r.rating, post = r.content, u.username });
            return Json(reviews);
        }

        public IActionResult Transcribe(int id)
        {
            var subtitle = db.Subtitle.FirstOrDefault(t => t.movie_id == id);
            var path = Path.GetFullPath(subtitle.path);
            byte[] buffer = System.IO.File.ReadAllBytes(path);
            return File(buffer, "text/vtt");
        }

        public IActionResult AddMovie()
        {
            return View();
        }

        [HttpPost, Authorize]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> AddMovie(string name, string genre, DateTime released, string description, IFormFile[] files, string lang)
        {
            var exists = db.Movie.Where(m => m.name == name)
                        .FirstOrDefault();

            if (exists != null) return Forbid();
            if (files.Length != 3) return Forbid();

            var path = $"./Storage/{Convert.ToBase64String(Encoding.UTF8.GetBytes(name))}";
            Directory.CreateDirectory(path);

            var img = files.FirstOrDefault(n => n.ContentType.Contains("image/"));
            var subtitle = files.FirstOrDefault(n => n.ContentType.Contains("text/vtt"));
            var buffer = files.FirstOrDefault(n => n.ContentType.Contains("video/"));

            FileStream fs = new FileStream($"{path}/{img.FileName}", FileMode.Create);
            await img.CopyToAsync(fs);

            fs = new FileStream($"{path}/{buffer.FileName}", FileMode.Create);
            await buffer.CopyToAsync(fs);

            fs = new FileStream($"{path}/{subtitle.FileName}", FileMode.Create);
            await subtitle.CopyToAsync(fs);

            var token = HttpContext.Request.Cookies["token"];
            var user = db.User.FirstOrDefault(u => u.token == token);

            if (user != null && user.admin)
            {
                var movie = new Movie
                {
                    name = name,
                    genre = genre,
                    released = released,
                    description = description,
                    preview = $"{path}/{img.FileName}",
                    user_id = user.Id,
                    path = $"{path}/{buffer.FileName}"
                };

                db.Movie.Add(movie);
                await db.SaveChangesAsync();

                var transcribe = new Subtitle
                {
                    code = lang,
                    path = $"{path}/{subtitle.FileName}",
                    movie_id = movie.Id
                };

                db.Subtitle.Add(transcribe);
                await db.SaveChangesAsync();
                return Ok();
            }
            else
                return Forbid();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Results()
        {
            return View();
        }

        public IActionResult FindResults(string name)
        {
            var list = (from movie in db.Movie.ToList()
                        orderby movie.name ascending
                        where movie.name.Contains(name)
                        let likes = db.Review.Where(r => r.movie_id == movie.Id)
                        select new
                        {
                            id = movie.Id,
                            movie.name,
                            movie.preview,
                            movie.views,
                            rating = (likes.Count() != 0) ? likes.Average(v => v.rating) : 0
                        }).ToList();

            return Json(list);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Rate(int id, string comment, int count)
        {
            string header = HttpContext.Request.Headers["Authorization"];
            string token = header.Split(' ')[1];

            var user = db.User.FirstOrDefault(u => u.token == token);
            var review = db.Review.FirstOrDefault(r => r.movie_id == id && r.user_id == user.Id);

            if (review != null) return Forbid();

            var post = new Review
            {
                content = comment,
                rating = count,
                movie_id = id,
                user_id = user.Id
            };

            db.Review.Add(post);
            await db.SaveChangesAsync();
            return Ok();
        }

        [Authorize]
        public IActionResult HasRating(int id)
        {
            string header = HttpContext.Request.Headers["Authorization"];
            string token = header.Split(' ')[1];

            var user = db.User.FirstOrDefault(u => u.token == token);
            var review = db.Review.FirstOrDefault(r => r.movie_id == id && r.user_id == user.Id);
            return Ok(new { valid = (review == null)});
        }

        public IActionResult UpdateProfile()
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
