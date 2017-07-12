﻿using Microsoft.Practices.Unity;
using Prism.Windows.Navigation;
using Prism.Commands;
using System.Collections.Generic;
using Prism.Windows.AppModel;

namespace GoG.WinRT.ViewModels
{
    public class PageViewModel : BaseViewModel
    {
        #region Data
        protected readonly IUnityContainer Container;
        protected readonly INavigationService NavService;
        protected readonly ISessionStateService SessionStateService;
        #endregion Data

        #region Ctor
        public PageViewModel(IUnityContainer container)
        {
            Container = container;
            NavService = container.Resolve<INavigationService>();
            SessionStateService = container.Resolve<ISessionStateService>();
        }

        #endregion Ctor

        #region Commands
        
        #region GoBackCommand
        DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand
        {
            get { if (_goBackCommand == null) _goBackCommand = new DelegateCommand(ExecuteGoBack, BaseCanGoBack); return _goBackCommand; }
        }
        private bool BaseCanGoBack()
        {
            return NavService.CanGoBack() && CanGoBack();
        }
        private void ExecuteGoBack()
        {
            NavService.GoBack();
        }
        #endregion GoBackCommand

        #endregion Commands

        #region Virtuals
        public override void OnNavigatingFrom(NavigatingFromEventArgs e, Dictionary<string, object> viewModelState, bool suspending)
        {
            base.OnNavigatingFrom(e, viewModelState, suspending);

            AbortOperation = true;
        }

        /// <summary>
        /// Override to place an additional restriction on the GoBackCommand.
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanGoBack()
        {
            return true;
        }
        #endregion Virtuals

        #region Properties
        #region AbortOperation
        private bool _abortOperation;
        public bool AbortOperation
        {
            get { return _abortOperation; }
            set { _abortOperation = value; OnPropertyChanged("AbortOperation"); }
        }
        #endregion AbortOperation

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    RaisePropertyChanged();
                    OnIsBusyChanged();
                }
            }
        }

        protected virtual void OnIsBusyChanged()
        {
            
        }

        private string _busyMessage;
        public string BusyMessage
        {
            get { return _busyMessage; }
            set { SetProperty(ref _busyMessage, value); }
        }
        #endregion Properties

        #region Helpers
        /// <summary>
        /// Goes back, but on the UI thread and after any current navigation action has completed.
        /// </summary>
        protected void GoBackDeferred()
        {
            // Note: Calling NavService.GoBack() in context of an existing navigation action causes
            // an exception.  Hence the need for this helper method.
            RunOnUIThread(() => NavService.GoBack());
        }
        #endregion Helpers
    }
}
