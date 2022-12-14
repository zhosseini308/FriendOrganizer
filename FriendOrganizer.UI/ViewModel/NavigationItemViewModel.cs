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
            string detailViewModelName,
            IEventAggregator eventAggregator)
        {
            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailView);
            Id = id;
            DisplayMember = displayMember;
            _detailViewModelName = detailViewModelName;
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

        private string _detailViewModelName;
        private IEventAggregator _eventAggregator;

        public ICommand OpenDetailViewCommand { get; }

        private void OnOpenDetailView()
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                        .Publish(
                new OpenDetailViewEventArgs
                {
                    Id = Id,
                    ViewModelName = _detailViewModelName
                }
                );
        }

    }
}
