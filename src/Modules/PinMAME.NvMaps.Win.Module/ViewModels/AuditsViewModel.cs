using Prism.Events;

namespace PinMAME.NvMaps.Win.Module.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class AuditsViewModel : TabViewModelBase
    {
        public AuditsViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            Section = "Audits";
        }
    }
}
