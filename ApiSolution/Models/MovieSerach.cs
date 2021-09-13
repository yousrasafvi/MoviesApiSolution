using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSolution.Models
{
    public class MovieSerach
    {
        public string? genreString { get; set; }

        [RegularExpression(@"^(19|20)\d{2}$")]
        public int? year { get; set; }

        public string? searchString { get; set; }
    }
}
