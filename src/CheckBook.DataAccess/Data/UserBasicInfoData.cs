using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckBook.DataAccess.Data
{
    public class UserBasicInfoData : IAvatarData 
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public int? UserId => Id;
    }
}