using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IFriendDetailViewModel _friendDetailViewModel;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private Func<IFriendDetailViewModel> _friendDetailViewModelCreator;


        public MainViewModel(INavigationViewModel navigationViewModel ,
            Func<IFriendDetailViewModel> friendDetailViewModelCreator , 
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService
            )
        {
            _eventAggregator = eventAggregator;
            _friendDetailViewModelCreator = friendDetailViewModelCreator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
               .Subscribe(OnOpenFriendDetailView);
            _eventAggregator.GetEvent<AfterFriendDeletedEvent>()
                .Subscribe(AfterFriendDeleted);

            CreateNewFriendCommand = new DelegateCommand(OnCreateNewFriendExecuete);

            NavigationViewModel = navigationViewModel;

        }

       
        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewFriendCommand { get; }

        public INavigationViewModel NavigationViewModel { get; }


        public IFriendDetailViewModel FriendDetailViewModel
        {
            get { return _friendDetailViewModel; }
            private set { _friendDetailViewModel = value;
                OnpropertyChanged();
            }
        }

        private async void OnOpenFriendDetailView(int? frientId)
        {
            if (FriendDetailViewModel != null && FriendDetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You've made changes. Navigate away?" , "Question");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            FriendDetailViewModel = _friendDetailViewModelCreator();
            await FriendDetailViewModel.LoadAsync(frientId);

        }

        private void OnCreateNewFriendExecuete()
        {
            OnOpenFriendDetailView(null);
        }

        private void AfterFriendDeleted(int frientId)
        {
            FriendDetailViewModel = null;
        }

       

    }
}

    