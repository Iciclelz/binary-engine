using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Binary_Engine.ViewModel
{
    public class BinaryEngineViewModel : INotifyPropertyChanged
    {
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }

        private string searchOption;
        public string SearchOption
        {
            get { return searchOption; }
            set
            {
                searchOption = value;
                OnPropertyChanged();
            }
        }

        public BinaryEngineViewModel()
        {
            FilePath = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
