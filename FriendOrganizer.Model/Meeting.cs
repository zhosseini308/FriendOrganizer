using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FriendOrganizer.Model
{
    public class Meeting
    {
        public Meeting()
        {
            //a meeting can have many friends
            Friends = new Collection<Friend>();
        }

        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [StringLength(50)]
        public string Title { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public ICollection<Friend> Friends { get; set; }
    }
}
