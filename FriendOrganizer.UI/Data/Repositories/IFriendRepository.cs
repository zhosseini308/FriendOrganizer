using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IFriendRepository
    {
        Action<object, PropertyChangedEventArgs> PropertyChanged { get; set; }
        FriendPhoneNumber Model { get; }

        Task<Friend> GetByIDAsync(int friendId);
        Task SaveAsync();
        bool HasChanges();
        void Add(Friend friend);
        void Remove(Friend model);
        void RemovePhoneNumber(FriendPhoneNumber model);
        void RemovePhoneNumber(object model);
    }
}
