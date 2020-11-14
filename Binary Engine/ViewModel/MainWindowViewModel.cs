using Binary_Engine.HexBox;
using Binary_Engine.Views;
using Dragablz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Binary_Engine.ViewModel
{
    public class MainWindowViewModel
    {
        private static MainWindowViewModel instance;
        public static MainWindowViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainWindowViewModel();
                }

                return instance;
            }
        }

        public IInterTabClient InterTabClient { get; }
        public ObservableCollection<TearableTabItem> TearableTabItems { get; }
        public Dictionary<TearableTabItem, BinaryEngineView> BinaryEngineViewDictionary;
        private static uint Id = 0;
        public static Func<object> NewBinaryEngineView => () => new TearableTabItem(null, ++Id, new BinaryEngineView());
        public ItemActionCallback OnClosingTabItem => (args) =>
        {
            TearableTabItem key = args.DragablzItem.DataContext as TearableTabItem;
            if (BinaryEngineViewDictionary[key].ByteViewer.ByteProvider != null)
            {
                BinaryEngineViewDictionary[key].ByteViewer.Dispose();
                (BinaryEngineViewDictionary[key].ByteViewer.ByteProvider as DynamicFileByteProvider).Dispose();
            }

            BinaryEngineViewDictionary.Remove(key);
        };

        private MainWindowViewModel()
        {
            InterTabClient = new DefaultInterTabClient();
            TearableTabItems = new ObservableCollection<TearableTabItem>();
            BinaryEngineViewDictionary = new Dictionary<TearableTabItem, BinaryEngineView>();
        }


        public static MainWindowViewModel Initialize()
        {
            Instance.TearableTabItems.Add(NewBinaryEngineView() as TearableTabItem);
            return Instance;
        }
    }
}
