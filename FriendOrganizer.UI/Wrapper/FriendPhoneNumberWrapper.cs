using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Wrapper
{
    public class FriendPhoneNumberWrapper 
    {
        private FriendPhoneNumber friendPhoneNumber;

     

        public FriendPhoneNumberWrapper(FriendPhoneNumber friendPhoneNumber)
        {
            this.friendPhoneNumber = friendPhoneNumber;
        }

        public Action<object, PropertyChangedEventArgs> PropertyChanged { get; internal set; }
    }
}
