using Prism.Events;

namespace PinMAME.NvMaps.Win.Module.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class AdjustmentsViewModel : TabViewModelBase
    {
        public AdjustmentsViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            Section = "Adjustments";
        }
    }
}
