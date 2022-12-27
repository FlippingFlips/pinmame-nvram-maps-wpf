using PinMAME.NvMaps.Win.Base.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using WPFPrismTemplate.Base.Events;

namespace PinMAME.NvMaps.Win.Module.ViewModels
{
    public class HighScoresViewModel : BindableBase
    {
        private DelegateCommand _findHighScoresCommand;
        private readonly IEventAggregator eventAggregator;

        public HighScoresViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Mapping chars to bytes for initials
        /// </summary>
        public string CharMap { get; set; }

        public ObservableCollection<HighScore> HighScores { get; set; } = new ObservableCollection<HighScore>();

        public DelegateCommand FindHighScoresCommand => _findHighScoresCommand ?? (_findHighScoresCommand =
            new DelegateCommand(OnFindHighScoresCmd));

        private void OnFindHighScoresCmd() => eventAggregator.GetEvent<FindHighScoresEvent>().Publish();
    }
}
