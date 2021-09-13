using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSolution.Models
{
    public class User
    {
        public int UserId { get; set; }

        //[Required]
        //[StringLength(10, MinimumLength = 3)]
        public string UserName { get; set; }

        //[Required]
        //[StringLength(30)]
        //[RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        //public string FirstName { get; set; }

        //[Required]
        //[StringLength(30)]
        //[RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        //public string LastName { get; set; }

    }
}
