using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBook.DataAccess.Model
{
    public class Restaurant
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int GroupId { get; set; }

        public Group Group { get; set; }
    }
}
