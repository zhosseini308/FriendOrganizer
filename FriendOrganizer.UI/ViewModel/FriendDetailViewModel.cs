using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Generic;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.Model;
using System;
using FriendOrganizer.UI.View.Services;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private IFriendRepository _friendRepository;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private FriendWrapper _friend;
        private bool _hasChanges;

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public FriendDetailViewModel(IFriendRepository FriendRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            _friendRepository = FriendRepository;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;

            SaveCommand = new DelegateCommand(OnSaveExecute , OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);


        }

        

        public async Task LoadAsync(int? friendId)
        {
            var friend = friendId.HasValue
                ? await _friendRepository.GetByIDAsync(friendId.Value)
                : CreateNewFriend();
            Friend = new FriendWrapper(friend);

            Friend.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _friendRepository.HasChanges();
                }
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Friend.Id == 0) // new friend is adding
            {
                //little trick to trigger the validation
                Friend.FirstName = "";
            }
        }

        

        public FriendWrapper Friend
        {
            get { return _friend; }
            private set
            {
                _friend = value;
                OnpropertyChanged();
            }
        }


        public bool HasChanges
        {
            get { return _hasChanges; }
            set {
                if(_hasChanges != value) { 
                _hasChanges = value;
                OnpropertyChanged();
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

       


        private bool OnSaveCanExecute()
        {
            return Friend!=null && !Friend.HasErrors && HasChanges;

        }

        private async void OnSaveExecute()
        {
           await _friendRepository.SaveAsync();
            HasChanges = _friendRepository.HasChanges();
            _eventAggregator.GetEvent<AfterFriendSavedEvent>()
                .Publish(
                new AfterFriendSavedEventArgs
                { Id = Friend.Id,
                    DisplayMember = $"{Friend.FirstName} {Friend.LastName}"
                });
        }

        private Friend CreateNewFriend()
        {
            var friend = new Friend();
            _friendRepository.Add(friend);
            return friend;
        }

        private async void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancelDialog($"Do you really want to " +
                $"delete the friend" +
                $" {Friend.FirstName} {Friend.LastName}?","Question");
            if (result == MessageDialogResult.Ok)
            {
                _friendRepository.Remove(Friend.Model);
                await _friendRepository.SaveAsync();
                _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Publish(Friend.Id);
            }
        }
    }
}
