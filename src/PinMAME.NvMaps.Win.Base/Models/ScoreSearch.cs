using Newtonsoft.Json;
using Prism.Mvvm;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace PinMAME.NvMaps.Win.Base.Models
{
    [AddINotifyPropertyChangedInterface]
    public class ScoreSearch
    {
        public string ScoreEntry { get; set; }
        public ObservableCollection<SearchedNvResultVm> SearchResults { get; set; } = new ObservableCollection<SearchedNvResultVm>();
        public SearchedNvResultVm SelectedResult { get; set; }
        public ObservableCollection<SearchedNvResultVm> SearchInitalsResults { get; set; } = new ObservableCollection<SearchedNvResultVm>();
        public SearchedNvResultVm SelectedInitialsResult { get; set; }

    }
}