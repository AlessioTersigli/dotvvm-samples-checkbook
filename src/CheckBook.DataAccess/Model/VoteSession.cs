using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBook.DataAccess.Model
{
    public class VoteSession
    {

        public int Id { get; set; }

        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public DateTime Date { get; set; }
    }


}
