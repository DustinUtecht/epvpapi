﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace epvpapi
{
    public class Post : Message
    {
        /// <summary>
        /// Title of the <c>Post</c>
        /// </summary>
        public string Title { get; set; }

        public Post(string content, string title = null)
            : this(0, content, title)
        { }

        public Post(uint id, string content = null, string title = null)
            : this(id, content, title, DateTime.Now)
        { }

        public Post(uint id, string content, string title, DateTime date)
            : base(id, content, date)
        {
            Title = title;
        }
    }
}
