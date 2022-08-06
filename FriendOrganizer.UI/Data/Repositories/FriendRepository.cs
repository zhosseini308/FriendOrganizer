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
    public class FriendRepository : IFriendRepository
    {
        private FriendOrganizerDbContext _context;

        public FriendRepository(FriendOrganizerDbContext context)
        {
            _context = context;


        }

        public Action<object, PropertyChangedEventArgs> PropertyChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public FriendPhoneNumber Model => throw new NotImplementedException();

        public void Add(Friend friend)
        {
            _context.Friends.Add(friend);
        }

        public async Task<Friend> GetByIDAsync(int friendId)
        {
            //load data from real database
            //yield return new Friend { FirstName = "zahra", LastName = "hosseini" };
            //yield return new Friend { FirstName = "maryam", LastName = "asghari" };
            //yield return new Friend { FirstName = "mona", LastName = "khosravi" };
            //yield return new Friend { FirstName = "sanaz", LastName = "mohammadi" };


            return await _context.Friends.SingleAsync(f => f.ID == friendId);



        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public void Remove(Friend model)
        {
            _context.Friends.Remove(model);

        }

        public void RemovePhoneNumber(FriendPhoneNumber model)
        {
            _context.FriendPhoneNumbers.Remove(model);
        }

        public void RemovePhoneNumber(object model)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync()
        {

            await _context.SaveChangesAsync();


        }


    }
}
