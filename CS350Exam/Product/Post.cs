using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS350Exam.Product
{
    public class Post
    {

        public int ID { get; set; }
        public string opID { get; set; }
        public string content { get; set; }
        public List<string> comments { get; set; }

    }
}
