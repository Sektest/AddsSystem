using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AddsSystem.Models
{
    public class UserPost
    {
        public UserPost()
        {
            this.PostImages = new HashSet<PostImage>();
        }

        public int UserPostId { get; set; }

        [Required(ErrorMessage = "Please Enter Your Name")]
        [Display(Name = "Title")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please Enter Summary")]
        [Display(Name = "Info")]
        [MaxLength(500)]
        public string Info { get; set; }

        public virtual ICollection<PostImage> PostImages { get; set; }
        public string UserId { get; set; }
        //public virtual ApplicationUser ApplicationUser { get; set; }
    }
}