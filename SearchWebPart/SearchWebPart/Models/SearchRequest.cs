using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SearchWebPart.Models
{
    public class SearchRequest
    {
        public string TextRequest { get; set; }

        public int TypeRequest { get; set; }
    }
}