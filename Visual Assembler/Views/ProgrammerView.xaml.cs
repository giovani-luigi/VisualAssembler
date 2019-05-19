using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Visual_Assembler.Device;

namespace Visual_Assembler.Views {
    /// <summary>
    /// Interaction logic for ProgrammerView.xaml
    /// </summary>
    public partial class ProgrammerView : Window {

        public ProgrammerView(FpgaBoard device, MemoryWord[] program) {
            InitializeComponent();
            this.DataContext = new ViewModels.ProgrammerViewModel(device, program);

            // subscribe to events whenever an item is added/removed from the collection bound to the log list view
            ((INotifyCollectionChanged)logList.Items).CollectionChanged += OnListViewCollectionChanged;

        }

        private void OnListViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            // whenever we add an item, make it selected/visible
            if (e.Action == NotifyCollectionChangedAction.Add) logList.ScrollIntoView(e.NewItems[0]);
        }
    }
}
