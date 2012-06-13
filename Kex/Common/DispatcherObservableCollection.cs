using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Threading;

namespace Kex.Common
{
    public class DispatcherObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Dispatcher _dispatcher;
        public DispatcherObservableCollection(Dispatcher dispatcher)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");
            _dispatcher = dispatcher;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(
                    new Action<PropertyChangedEventArgs>(base.OnPropertyChanged), e);
            }
            else
            {
                base.OnPropertyChanged(e);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(
                    new Action<NotifyCollectionChangedEventArgs>(base.OnCollectionChanged), e);
            }
            else
            {
                base.OnCollectionChanged(e);
            }
        }
    }
}
