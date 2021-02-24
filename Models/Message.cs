using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage="Messages must be at least 2 characters.")]
        [Display(Name = "Message Body")]
        public string MessageBody { get; set; }
        
        [MinLength(2, ErrorMessage="Messages must be at least 2 characters.")]
        [Display(Name = "Image Url (optional)")]
        public string ImgUrl { get; set; }

        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User Creator { get; set; }

        public List<Comment> CommentsOnPost { get; set; }
    }
}