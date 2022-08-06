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
using FriendOrganizer.UI.Data.Lookups;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private IFriendRepository _friendRepository;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;
        private FriendWrapper _friend;
        private bool _hasChanges;
       

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RemovePhoneNumberCommand { get; private set; }
        public ICommand AddPhoneNumberCommand { get; private set; }
        public ObservableCollection<LookupItem> ProgrammingLanguages { get; }
        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; set; }

        public FriendDetailViewModel(IFriendRepository FriendRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLanguageLookupDataService programmingLanguageLookupDataService)
        {
            _friendRepository = FriendRepository;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            SaveCommand = new DelegateCommand(OnSaveExecute , OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute , OnRemovePhoneNumberCanExecute);

            ProgrammingLanguages = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();


        }


        public async Task LoadAsync(int? friendId)
        {
            var friend = friendId.HasValue
                ? await _friendRepository.GetByIDAsync(friendId.Value)
                : CreateNewFriend();

            InitializeFriend(friend);

            await LoadProgrammingLanguagesLookupAsync();

        }

        private void InitializeFriend(Friend friend)
        {
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

        private async Task LoadProgrammingLanguagesLookupAsync()
        {
            //if this method called twice:
            ProgrammingLanguages.Clear();
            ProgrammingLanguages.Add(new NullLookupItem { DisplayMember= "-"});
            var lookup = await _programmingLanguageLookupDataService.GetProgrammingLanguageLookupAsync();
            foreach (var lookupItem in lookup)
            {
                ProgrammingLanguages.Add(lookupItem);
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

       
        public IFriendRepository SelectedPhoneNumber { get; set; }

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

        private void OnAddPhoneNumberExecute()
        {
            var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            PhoneNumbers.Add(newNumber);
            Friend.Model.PhoneNumbers.Add(newNumber.Model);
            newNumber.Number = ""; //Trigger validation
        }
        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _friendRepository.HasChanges();
            }
            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }


        private void OnRemovePhoneNumberExecute()
        {
            SelectedPhoneNumber.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
             _friendRepository.RemovePhoneNumber(SelectedPhoneNumber.Model);
            PhoneNumbers.Remove(SelectedPhoneNumber);
            SelectedPhoneNumber = null;
            HasChanges = _friendRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }


      
        private bool OnRemovePhoneNumberCanExecute()
        {
            return SelectedPhoneNumber != null;
        }

    }
}
