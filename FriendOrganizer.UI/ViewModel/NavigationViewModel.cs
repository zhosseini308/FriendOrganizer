using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Event;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IFriendLookupDataService _friendLookupService;
        private IEventAggregator _eventAggregator;

        public NavigationViewModel(IFriendLookupDataService friendLookupService,
            IEventAggregator eventAggregator)
            
        {
            _friendLookupService = friendLookupService;
            _eventAggregator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Subscribe(AfterDetailSaved);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);
        }

        

        public async Task LoadAsync()
        {
            var lookup = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in lookup)
            {
                Friends.Add(new NavigationItemViewModel(item.ID , item.DisplayMember,
                    nameof(FriendDetailViewModel),
                    _eventAggregator));
            }
        }
        public ObservableCollection<NavigationItemViewModel> Friends { get; }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    var friend = Friends.FirstOrDefault(f => f.Id == args.Id);

                    if (friend != null)
                    {
                        Friends.Remove(friend);
                    }
                    break;
            }
           
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs obj)
        {
            switch (obj.ViewModelName)
            {
                case nameof(FriendDetailViewModel):

            var lookupItem = Friends.FirstOrDefault(l => l.Id == obj.Id);
            if (lookupItem == null)
            {
                Friends.Add(new NavigationItemViewModel(obj.Id, obj.DisplayMember,
                    nameof(FriendDetailViewModel),
                    _eventAggregator));
            }
            else
            {
                //update displaymember property:
                lookupItem.DisplayMember = obj.DisplayMember;
            }

                    break;
            }
        }


    }
}
