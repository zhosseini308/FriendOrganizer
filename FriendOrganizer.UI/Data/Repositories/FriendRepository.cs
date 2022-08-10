﻿using FriendOrganizer.Model;
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

       
        public void Add(Friend friend)
        {
            _context.Friends.Add(friend);
        }




        public async Task<Friend> GetByIdAsync(int friendId)
        {
            return await _context.Friends
              .Include(f => f.PhoneNumbers)
              .SingleAsync(f => f.ID == friendId);
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

        public async Task SaveAsync()
        {

            await _context.SaveChangesAsync();


        }


    }
}
