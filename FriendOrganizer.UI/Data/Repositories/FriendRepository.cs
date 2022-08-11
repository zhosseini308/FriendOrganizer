using FriendOrganizer.Model;
using System.Collections.Generic;
using FriendOrganizer.DataAccess;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class FriendRepository : GenericRepository<Friend, FriendOrganizerDbContext>, IFriendRepository
    {
       

        public FriendRepository(FriendOrganizerDbContext context)
            :base(context)
        {
         

        }






        public override async Task<Friend> GetByIdAsync(int friendId)
        {
            return await Context.Friends
              .Include(f => f.PhoneNumbers)
              .SingleAsync(f => f.ID == friendId);
        }

       

      
        public void RemovePhoneNumber(FriendPhoneNumber model)
        {
            Context.FriendPhoneNumbers.Remove(model);
        }

      


    }
}
