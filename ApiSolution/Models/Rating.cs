using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSolution.Models
{
    public class Rating
    {
        public int RatingId { get; set; }

        [Required]
        public int UserId {get;set;}

        [Required]
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual User User { get; set; }

        //[RegularExpression(@"/^[1-5]$/")]
        [Required]
        public int RatingValue { get; set; }

    }
}
