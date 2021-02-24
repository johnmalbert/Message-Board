using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        [Display(Name = "Comment on this post?")]
        [MinLength(2, ErrorMessage="Comment must be at least 2 characters.")]
        public string CommentBody { get; set; }

        public int UserId { get; set; }

        public int MessageId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public User UserWhoCommented { get; set; }
        public Message MessageWithComment { get; set; }
    }
}   