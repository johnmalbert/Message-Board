using System;
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage="Please provide your email.")]
        [EmailAddress(ErrorMessage="Please Provide a valid email.")]
        public string LoginEmail { get; set; }

        [Required(ErrorMessage="Please provide your password.")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }
    }
}