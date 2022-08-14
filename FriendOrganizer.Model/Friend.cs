
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FriendOrganizer.Model
{
    public class Friend
    {
        public Friend()
        {
            PhoneNumbers = new Collection<FriendPhoneNumber>();
            //a friend can have many meetings
            Meetings = new Collection<Meeting>();
        }
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        public int? FavoriteLanguageId { get; set; }
        public ProgrammingLanguage FavoriteLanguage { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public ICollection<FriendPhoneNumber> PhoneNumbers { get; set; }
        public ICollection<Meeting> Meetings { get; set; }
    }
}
