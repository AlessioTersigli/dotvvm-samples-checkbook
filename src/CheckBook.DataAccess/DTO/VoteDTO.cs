using CheckBook.DataAccess.Data;
using System.Collections.Generic;

namespace CheckBook.DataAccess.DTO
{
    public class VoteDTO
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public bool IsMyVote { get; set; }
        public List<UserBasicInfoData> Users { get; set; }

    }
}