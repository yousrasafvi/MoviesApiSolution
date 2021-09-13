using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiSolution.Models;

namespace ApiSolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly ApiSolutionContext _context;

        public RatingsController(ApiSolutionContext context)
        {
            _context = context;
        }

        // POST: api/Ratings/SaveRating  
        /// <summary>
        /// Add or Update rating (Solution to question- API D)
        /// </summary>
        /// <param name="rating"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveRating")]
        public async Task<ActionResult> SaveRating(Rating rating)
        {
            if (!ModelState.IsValid || (rating.RatingValue < 1 || rating.RatingValue > 5))
            {
                return BadRequest();
            }

            bool movie_exists = _context.Movies.Any(e => e.MovieId == rating.MovieId);
            bool user_exits = _context.Users.Any(e => e.UserId == rating.UserId);
            if (!movie_exists || !user_exits)
            {
                return NotFound();
            }

            //To check if Rating exists by ratingid or by user and movie id then just update else add a new record
            if (RatingExists(rating.UserId, rating.MovieId)){

                int id = GetRatingIdByDetails(rating.MovieId, rating.UserId);
                var rating_modified = await _context.Ratings.FindAsync(id);
                _context.Entry(rating_modified).State = EntityState.Modified;
            }
            else
            {
                _context.Ratings.Add(rating);
                
            }

            await _context.SaveChangesAsync();

            return Ok();
        }


        //Post: api/Ratings/SearchMovies
        /// <summary>
        /// Solution to API A. I used search model for expandibility of search
        /// </summary>
        /// <param name="MovieSearch"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SearchMovies")]
        public async Task<ActionResult<IEnumerable<Movie>>> SearchMovies(MovieSerach MovieSearch)
        {

            if (!ModelState.IsValid || (MovieSearch.year == null && string.IsNullOrEmpty(MovieSearch.genreString) && string.IsNullOrEmpty(MovieSearch.searchString)))
            {
                return BadRequest();
            }
            var result = from m in _context.Movies
                         select m;

            if (!string.IsNullOrEmpty(MovieSearch.genreString))
                result = _context.Movies.Where(x => x.Genre.Equals(MovieSearch.genreString));
            if (!string.IsNullOrEmpty(MovieSearch.searchString))
                result = result.Where(x => x.Title.Contains(MovieSearch.searchString));
            if (MovieSearch.year.HasValue)
                result = result.Where(x => x.YearOfRelease == MovieSearch.year);

            var movie = await result.ToListAsync();

            if (movie.Count < 1)
            {
                return NotFound();
            }


            foreach (Movie m in movie)
            {
                m.averageRating = RatingForMovie(m.MovieId);
            }

            return movie;

        }


        //GET: api/Ratings/Top5Movies
        /// <summary>
        /// Top 5 movies by average rating  (Solution to question API B)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Top5Movies")]
        public async Task<ActionResult> Top5Movies()
        {
            var result = _context.Movies.Join(_context.Ratings.GroupBy(r => r.MovieId).Select(g => new { MovieId = g.Key, AvgRating = g.Average(r => r.RatingValue) }),
                        m => m.MovieId,
                        r => r.MovieId,
                        (m, r) => new { title = m.Title, yearOfRealease = m.YearOfRelease, runningTime = m.runningTime, averageRating = RatingRounding(r.AvgRating) }).ToList().OrderByDescending(x => x.averageRating).ThenBy(x => x.title).Take(5);
            if (result.Count() < 0)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //GET: api/Ratings/Top5MoviesByUser/2
        /// <summary>
        /// Gets highest rated movie by specific user (Solution to question API C)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Top5MoviesByUser/{userId}")]
        public async Task<ActionResult> Top5MoviesByUser(int userId)
        {

            bool userExists = _context.Users.Any(e => e.UserId == userId);
            if (!userExists)
            {
                return BadRequest();
            }

            var result = (from m in _context.Movies
                          join r in _context.Ratings on m.MovieId equals r.MovieId
                          where r.UserId == userId
                          select new
                          {
                              title = m.Title,
                              runningTime = m.runningTime,
                              yearOfRelease = m.YearOfRelease,
                              genre = m.Genre,
                              rating = r.RatingValue
                          }).ToList().OrderByDescending(x => x.rating).Take(5);


            if (result.Count() < 0)
            {
                return NotFound();
            }

            return Ok(result);
        }


        // DELETE: api/Ratings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        /// <summary>
        /// Helper Functions
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.RatingId == id);
        }

        /// <summary>
        /// Helper functions
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="movieid"></param>
        /// <returns></returns>
        private bool RatingExists(int userid, int movieid)
        {
            return _context.Ratings.Any(e => e.MovieId == movieid && e.UserId == userid);
        }


        private int  GetRatingIdByDetails(int movId, int userId)
        {
            int rating_id = _context.Ratings.Where(r => r.MovieId == movId && r.UserId == userId).Select( r => r.RatingId).FirstOrDefault();
            return rating_id;
        }

        [HttpGet]
        [Route("RatingForMovie")]
        public double RatingForMovie(int movieId)
        {
            bool movieExits = _context.Ratings.Any(e => e.MovieId == movieId);

            if (movieExits)
            {
                var ratings = from r in _context.Ratings
                              select r;

                double avg_rating = RatingRounding((from r in _context.Ratings where r.MovieId == movieId select r.RatingValue).Average());

                return avg_rating;
            }
            return 0;

        }
               

        //Rouding the number to the closest 0.5
        private double RatingRounding(double ratingVal)
        {
            return Math.Round(ratingVal * 2, 0, MidpointRounding.AwayFromZero) / 2;
        }

        
        //dDefault get, gets details for rating
        // GET: api/Ratings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRatings()
        {
            return await _context.Ratings.ToListAsync();
        }

        //default get, to get rating details by rating id
        // GET: api/Ratings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rating>> GetRating(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);

            if (rating == null)
            {
                return NotFound();
            }

            return rating;
        }

        // PUT: api/Ratings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutRating(int id, Rating rating)
        //{
        //    if (id != rating.RatingId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(rating).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!RatingExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Ratings
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Rating>> PostRating(Rating rating)
        //{
        //    _context.Ratings.Add(rating);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetRating), new { id = rating.RatingId }, rating);
        //}

    }
}
