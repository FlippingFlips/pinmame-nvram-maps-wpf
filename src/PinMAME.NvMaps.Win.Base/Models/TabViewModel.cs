using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace WPFPrismTemplate.Base.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class TabViewModel : BindableBase
    {
        public string Header { get; set; }

        public bool IsEnabled { get; set; } = true;

        public ObservableCollection<NvMapObjectEntry> NvMapObjectViewModels { get; set; }

        public TabViewModel()
        {
            NvMapObjectViewModels = new ObservableCollection<NvMapObjectEntry>();
        }
    }
}
