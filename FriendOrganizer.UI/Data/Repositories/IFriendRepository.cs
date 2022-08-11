using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IFriendRepository :IGenericRepository<Friend>
    {

        void RemovePhoneNumber(FriendPhoneNumber model);
        
    }
}
