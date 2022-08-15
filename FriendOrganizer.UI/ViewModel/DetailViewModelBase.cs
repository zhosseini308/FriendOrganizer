﻿using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static FriendOrganizer.UI.View.Services.MessageDialogService;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {

        private bool _hasChanges;
        protected readonly IEventAggregator EventAggregator;
        protected readonly IMessageDialogService MessageDialogService;
        private int _id;
        private string _title;

        public DetailViewModelBase(IEventAggregator eventAggregator ,
            IMessageDialogService messageDialogService)
        {
            EventAggregator = eventAggregator;
            MessageDialogService = messageDialogService;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            closeDetailViewCommand = new DelegateCommand(OnCloseDetailViewExecute);
        }



        public abstract Task LoadAsync(int id);
        public ICommand SaveCommand { get; private set; }

        public ICommand DeleteCommand { get; }
        public ICommand closeDetailViewCommand { get; private set; }

       

        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                OnPropertyChanged();
            }
        }


        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }


        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        protected abstract void OnDeleteExecute();

        protected abstract bool OnSaveCanExecute();

        protected abstract void OnSaveExecute();

        protected virtual void RaiseCollectionSavedEvent()
        {
            EventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Publish(new AfterCollectionSavedEventArgs
                {
                    ViewModelName = this.GetType().Name
                }
                );
        }
        protected virtual void RaiseDetailDeletedEvent(int modelId)
        {
            EventAggregator.GetEvent<AfterDetailDeletedEvent>().Publish(new
             AfterDetailDeletedEventArgs
            {
                Id = modelId,
                ViewModelName = this.GetType().Name
            });
        }

        protected virtual void RaiseDetailSavedEvent(int modelId, string displayMember)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(new AfterDetailSavedEventArgs
            {
                Id = modelId,
                DisplayMember = displayMember,
                ViewModelName = this.GetType().Name
            });
        }

        protected async virtual void OnCloseDetailViewExecute()
        {
            if (HasChanges)
            {
                var result =await MessageDialogService.ShowOkCancelDialogAsync(
                    "You've made changes, Close this item?", "Question");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            EventAggregator.GetEvent<AfterDetailClosedEvent>()
                 .Publish(new AfterDetailClosedEventArgs
                 {
                     Id = this.Id,
                     ViewModelName = this.GetType().Name
                 });
        }
        protected  async Task SaveWithOptimisticConcurrencyAsync(Func <Task> saveFunc,
            Action afterSaveAction)
        {
            try
            {
                await saveFunc();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var databaseValues = ex.Entries.Single().GetDatabaseValues();
                if (databaseValues == null)
                {
                    await MessageDialogService.ShowInfoDialogAsync("the entity has been deleted by another user.");
                    RaiseDetailDeletedEvent(Id);
                    return;
                }
                var result = await MessageDialogService.ShowOkCancelDialogAsync("The Entity has been changed in" +
                    "the meantime by someone else. click Ok to save your changes anyway,click Cancel" +
                    "to reload the entity from the database", "Question");
                if (result == MessageDialogResult.Ok)
                {
                    //update the original values with database values:
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    await saveFunc();

                }
                else
                {
                    //reload entity from database
                    await ex.Entries.Single().ReloadAsync();
                    await LoadAsync(Id);
                }
            };
            afterSaveAction();
        }


    }
}
