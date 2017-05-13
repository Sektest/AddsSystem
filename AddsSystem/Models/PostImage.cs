using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddsSystem.Models
{
    public class PostImage
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }

        public int UserPostId { get; set; }

        //public virtual UserPost UserPostId { get; set; }
    }
}