using Binary_Engine.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Binary_Engine.ViewModel
{
    public class TearableTabItem : INotifyPropertyChanged
    {
        private string _Header;
        public string Header
        {
            get { return _Header; }
            set
            {
                _Header = value;
                OnPropertyChanged();
            }
        }

        public uint Id { get; }
        public object Content { get; }

        public TearableTabItem(string header, uint id, object content)
        {
            Header = header;
            Id = id;
            Content = content;

            if (Header == null)
            {
                Header = "Untitled " + id;
            }

            MainWindowViewModel.Instance.BinaryEngineViewDictionary[this] = Content as BinaryEngineView;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
