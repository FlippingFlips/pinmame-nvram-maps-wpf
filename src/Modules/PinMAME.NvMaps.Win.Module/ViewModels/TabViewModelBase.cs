using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using WPFPrismTemplate.Base.Events;
using WPFPrismTemplate.Base.Models;

namespace PinMAME.NvMaps.Win.Module.ViewModels
{
    public class TabViewModelBase : BindableBase
    {
        public ObservableCollection<TabViewModel> TabViewModels { get; set; }

        private DelegateCommand _addMenuCommand;
        private DelegateCommand _previewOutputCommand;
        public string Section { get; set; }
        private readonly IEventAggregator eventAggregator;

        public DelegateCommand AddMenuCommand => _addMenuCommand ?? (_addMenuCommand =
            new DelegateCommand(OnAddMenuCommand));

        public DelegateCommand PreviewOutputCommand => _previewOutputCommand ?? (_previewOutputCommand =
            new DelegateCommand(PreviewOutput));

        public string NewMenuName { get; set; }
        public int RowGenerateCount { get; set; } = 44;
        public int RowGenerateStartOffset { get; set; } = 7025;
        public int RowGenerateLength { get; set; } = 2;
        public string DefaultEncodingType { get; set; } = "int";

        public TabViewModelBase(IEventAggregator eventAggregator)
        {
            TabViewModels = new ObservableCollection<TabViewModel>();
            this.eventAggregator = eventAggregator;
        }

        private void OnAddMenuCommand()
        {
            var newMenu = new TabViewModel { Header = NewMenuName, IsEnabled = true };
            string key = string.Empty;
            int startOffset = RowGenerateStartOffset;
            for (int i = 1; i < RowGenerateCount; i++)
            {
                key = i < 10 ? "0"+i : i.ToString();
                if(i!=1)
                    startOffset += 2;
                newMenu.NvMapObjectViewModels.Add(new NvMapObjectEntry { Key = key, start = startOffset.ToString(), encoding=DefaultEncodingType });
            }

            TabViewModels.Add(newMenu);
        }

        public void PreviewOutput()
        {
            eventAggregator.GetEvent<PreviewOutputEvent>().Publish(Section);
        }
    }
}
