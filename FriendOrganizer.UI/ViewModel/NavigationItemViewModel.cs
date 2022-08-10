using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel :ViewModelBase
    {
        private string _displayMember;
        public NavigationItemViewModel(int id , string displayMember, 
            IEventAggregator eventAggregator)
        {
            OpenFriendDetailViewCommand = new DelegateCommand(OnOpenFriendDetailView);
            Id = id;
            DisplayMember = displayMember;
            _eventAggregator = eventAggregator;

        }

     
        public int Id { get; }
        public string DisplayMember {
            get
            { return _displayMember; }
                
             set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        private IEventAggregator _eventAggregator;

        public ICommand OpenFriendDetailViewCommand { get; }

        private void OnOpenFriendDetailView()
        {
            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                        .Publish(Id);
        }

    }
}
