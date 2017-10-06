using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckBook.DataAccess.Model;

namespace CheckBook.DataAccess.DTO
{
    public class VoteSessionDTO
    {
        public List<VoteDTO> Votes { get; set; } = new List<VoteDTO>();

    }
}
