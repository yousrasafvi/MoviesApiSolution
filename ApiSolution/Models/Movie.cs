using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSolution.Models
{
    public class Movie
    {
        public int MovieId { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Title { get; set; }

               
        [RegularExpression(@"^(19|20)\d{2}$")]
        public int YearOfRelease { get; set; }

        [RegularExpression(@"^[0-9]{3}$")]
        public int runningTime { get; set; }

        //public List<Rating> Rating { get; set; }

        public string Genre { get; set; }
        //public virtual ICollection<Rating> Ratings { get; set; }

        public double averageRating { get; set; }

    }
}
