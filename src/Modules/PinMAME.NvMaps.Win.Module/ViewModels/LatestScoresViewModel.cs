using PinMAME.NvMaps.Win.Base.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using WPFPrismTemplate.Base.Events;

namespace PinMAME.NvMaps.Win.Module.ViewModels
{
    public class LatestScoresViewModel : BindableBase
    {
        private DelegateCommand _findHighScoresCommand;
        private readonly IEventAggregator eventAggregator;

        public LatestScoresViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public ObservableCollection<HighScoreBase> HighScores { get; set; } = new ObservableCollection<HighScoreBase>();

        public DelegateCommand FindHighScoresCommand => _findHighScoresCommand ?? (_findHighScoresCommand =
            new DelegateCommand(OnFindHighScoresCmd));

        private void OnFindHighScoresCmd() => eventAggregator.GetEvent<FindLatestScoresEvent>().Publish();
    }
}
