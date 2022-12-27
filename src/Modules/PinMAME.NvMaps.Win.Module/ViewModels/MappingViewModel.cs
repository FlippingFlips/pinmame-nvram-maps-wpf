using Prism.Mvvm;
using PropertyChanged;

namespace PinMAME.NvMaps.Win.Module.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MappingViewModel : BindableBase
    {
        public string NvRamMapJson { get; set; }

        public void RaisePropertyChanged()
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(NvRamMapJson)));
        }
    }
}
