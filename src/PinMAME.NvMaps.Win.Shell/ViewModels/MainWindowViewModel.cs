using GongSolutions.Wpf.DragDrop;
using MahApps.Metro.Controls.Dialogs;
using PinMAME.NvMaps.Model;
using PinMAME.NvMaps.Win.Base.Models;
using PinMAME.NvMaps.Win.Module.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using Unity;
using WPFPrismTemplate.Base.Events;

namespace PinMAME.NvMaps.Win.Shell.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : BindableBase, IDropTarget
    {
        private readonly IDialogCoordinator dialog;
        private readonly IUnityContainer unityContainer;
        private string _file;
        private DelegateCommand _generateJsonCommand;
        private DelegateCommand _loadNvDataCommand;
        private DelegateCommand _mapCommand;
        private DelegateCommand _previewOutputCommand;
        private bool _viewModelsResolved;
        private HighScoresViewModel highscoresVm;
        private LatestScoresViewModel latestScoresVm;
        private MappingViewModel mappingVm;
        private AdjustmentsViewModel adjustmentsVm;
        private AuditsViewModel auditsVm;
        private ModeChampionsViewModel modeChampsVm;

        public MainWindowViewModel(IDialogCoordinator dialog, IUnityContainer unityContainer, IEventAggregator eventAggregator)
        {
            this.dialog = dialog;
            this.unityContainer = unityContainer;

            //init the mapping model to save state and pass to the NVParser
            NvMapModel = new NvRamMappingApp() { };

            eventAggregator.GetEvent<FindHighScoresEvent>().Subscribe(() => OnFindHighScoresCommand());
            eventAggregator.GetEvent<FindLatestScoresEvent>().Subscribe(() => OnFindLatestScoresCommand());
            eventAggregator.GetEvent<PreviewOutputEvent>().Subscribe(x => OnPreviewOutputCommand(x));
            eventAggregator.GetEvent<FindModeChampsEvent>().Subscribe(() => OnFindModeChampsCommand());
        }

        public bool CanExecuteMappingJson { get; set; }
        public string FileDropHeader { get; set; } = "NVRAM FILE DROP";
        public DelegateCommand GenerateJsonCommand => _generateJsonCommand ?? (_generateJsonCommand = new DelegateCommand(OnCreateMappingJson));
        public bool HasDroppedFile { get; set; }
        public DelegateCommand LoadNvDataCommand => _loadNvDataCommand ?? (_loadNvDataCommand = new DelegateCommand(OnLoadNvDataCommand));
        public DelegateCommand MapCommand => _mapCommand ?? (_mapCommand = new DelegateCommand(OnExecuteSaveMapCommand));
        public NvRamMappingApp NvMapModel { get; set; } = new NvRamMappingApp();

        public DelegateCommand PreviewOutputCommand => _previewOutputCommand ?? (_previewOutputCommand =
                    new DelegateCommand(() => OnPreviewOutputCommand(null)));
        public string Title { get; set; } = "PinMAME.NvMaps Mapping Helper";

        /// <summary>
        /// Create a mapping from a users selected search results
        /// </summary>
        void OnCreateMappingJson()
        {
            //create new map to serialize
            var nvMap = NvMapModel.CreateNewMap();

            //latest scores
            if (latestScoresVm?.HighScores?.Count > 0)
            {
                foreach (var score in latestScoresVm.HighScores)
                {
                    if (score?.Score.SelectedResult == null) break;
                    var lastGame = new LastGame();
                    nvMap.last_game.Add(lastGame);
                    lastGame.start = score.Score.SelectedResult?.Offset ?? 0;
                    lastGame.length = score.Length;
                    lastGame.encoding = score.Encoding ?? "bcd";
                    lastGame.packed = score.Packed;
                }
            }
            else { nvMap.last_game = null; }

            //high scores
            if (highscoresVm?.HighScores?.Count > 0)
            {
                foreach (var score in highscoresVm.HighScores)
                {
                    if (score.Score?.SelectedResult == null) break;

                    var hiScore = new Model.HighScore() { label = score.Label, short_label = score.ShortLabel };
                    nvMap.high_scores.Add(hiScore);

                    hiScore.Initials.start = score.Score.SelectedInitialsResult?.Offset ?? 0;
                    hiScore.Initials.length = score.LengthInitials <= 0 ? 3 : score.LengthInitials;
                    hiScore.Initials.encoding = "ch";
                    hiScore.Initials.mask = score.Mask;
                    nvMap._char_map = highscoresVm.CharMap;

                    hiScore.score.start = score.Score.SelectedResult?.Offset ?? 0;
                    hiScore.score.length = score.Length;
                    hiScore.score.packed = score.Packed;
                    hiScore.score.encoding = score.Encoding ?? "bcd";
                }
            }
            else { nvMap.high_scores = null; }

            if(modeChampsVm?.HighScores?.Count > 0)
            {
                foreach (var score in modeChampsVm.HighScores)
                {
                    if (score.Score?.SelectedResult == null) break;

                    var hiScore = new Model.HighScore() { label = score.Label, short_label = score.ShortLabel };
                    nvMap.mode_champions.Add(hiScore);

                    hiScore.Initials.start = score.Score.SelectedInitialsResult?.Offset ?? 0;
                    hiScore.Initials.length = score.LengthInitials <= 0 ? 3 : score.LengthInitials;
                    hiScore.Initials.encoding = "ch";

                    hiScore.score.start = score.Score.SelectedResult?.Offset ?? 0;
                    hiScore.score.length = score.Length;
                    hiScore.score.packed = score.Packed;
                    hiScore.score.encoding = score.Encoding ?? "bcd";
                }
            }
            else { nvMap.mode_champions = null; }

            //adjustments
            if (adjustmentsVm?.TabViewModels?.Count > 0)
            {
                foreach (var tab in adjustmentsVm.TabViewModels)
                {
                    if (tab.Header == null) break;

                    var dict = new Dictionary<string, NvMapObject>();
                    foreach (var item in tab.NvMapObjectViewModels.OrderBy(x => x.Key))
                    {
                        //conver the special values to <int, string>
                        Dictionary<int, string> special = null;
                        try
                        {
                            if (item.special_values != null)
                            {
                                special = new Dictionary<int, string>();
                                //check if can convert to key / value
                                var sValues = item.special_values.Split(',');
                                if (sValues.Length % 2 == 0)
                                {
                                    for (int i = 0; i < sValues.Length; i += 2)
                                    {
                                        int.TryParse(sValues[i], out var key);
                                        special.Add(key, sValues[i + 1]);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error while trying to convert special values: " + ex.Message);
                            return;
                        }

                        //save the json start as as integer or string
                        int startInt = 0;
                        if (!item.start?.Contains("x") ?? false)
                            int.TryParse(item.start, out startInt);

                        if (dict.ContainsKey(item.Key))
                            continue;
                        //create new object leaving out the key
                        dict.Add(item.Key, new NvMapObject
                        {
                            @default = item.@default,
                            multiple_of = item.multiple_of,
                            encoding = item.encoding,
                            label = item.label,
                            length = item.length,
                            mask = item.mask,
                            max = item.max,
                            min = item.min,
                            offset = item.offset,
                            scale = item.scale,
                            short_label = item.short_label,
                            start = startInt > 0 ? (object)startInt : (object)item.start,
                            suffix = item.suffix,
                            units = item.units,
                            special_values = special,
                            values = item.values?.Split(',')
                        });
                    }
                    nvMap.adjustments.Add(tab.Header, dict);
                }
            }
            else { nvMap.adjustments = null; }

            //audits
            if (auditsVm?.TabViewModels?.Count > 0)
            {
                foreach (var tab in auditsVm.TabViewModels)
                {
                    if (tab.Header == null) break;

                    var dict = new Dictionary<string, NvMapObject>();
                    foreach (var item in tab.NvMapObjectViewModels.OrderBy(x=>x.Key))
                    {
                        //convert the special values to <int, string>
                        Dictionary<int, string> special = null;
                        try
                        {
                            if (item.special_values != null)
                            {
                                special = new Dictionary<int, string>();
                                //check if can convert to key / value
                                var sValues = item.special_values.Split(',');
                                if (sValues.Length % 2 == 0)
                                {
                                    for (int i = 0; i < sValues.Length; i += 2)
                                    {
                                        int.TryParse(sValues[i], out var key);
                                        special.Add(key, sValues[i + 1]);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error while trying to convert special values: " + ex.Message);
                            return;
                        }

                        //save the json start as as integer or string
                        int startInt = 0;
                        if (!item.start.Contains("x"))
                            int.TryParse(item.start, out startInt);

                        if (dict.ContainsKey(item.Key))
                            continue;
                        //create new object leaving out the key
                        dict.Add(item.Key, new NvMapObject
                        {
                            @default = item.@default,
                            encoding = item.encoding,
                            label = item.label,
                            length = item.length,
                            mask = item.mask,
                            max = item.max,
                            min = item.min,
                            offset = item.offset,
                            scale = item.scale,
                            short_label = item.short_label,
                            start = startInt > 0 ? (object)startInt : (object)item.start,
                            suffix = item.suffix,
                            units = item.units,
                            special_values = special,
                            values = item.values?.Split(',')
                        });
                    }
                    nvMap.audits.Add(tab.Header, dict);
                }
            }
            else { nvMap.audits = null; }

            if (nvMap.last_game?.Count <= 0)
                nvMap.last_game = null;

            //populate mapping view
            if (mappingVm != null)
            {
                mappingVm.NvRamMapJson = ParseNVRAM.SerializeWithCustomIndenting(nvMap, 3);
                mappingVm.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Saves app state, nvram and json mapping
        /// </summary>
        void OnExecuteSaveMapCommand()
        {
            ResolveViewModels();

            //create directory under rom name
            var dir = NvMapModel.NvRamFile.FileNoExt;
            Directory.CreateDirectory(dir);

            //var mappingVm = unityContainer.Resolve(typeof(MappingViewModel)) as MappingViewModel;
            if (!string.IsNullOrWhiteSpace(mappingVm?.NvRamMapJson) && !string.IsNullOrWhiteSpace(NvMapModel.NvRamFile.HexString))
            {
                //save nv mapping
                File.WriteAllText(Path.Combine(dir, $"{NvMapModel.NvRamFile.FileNoExt}.nv.json"), mappingVm.NvRamMapJson);
            }

            //save the nv ram file
            File.Copy(NvMapModel.NvRamFile.FileName, Path.Combine(dir, $"{NvMapModel.NvRamFile.FileNoExt}.nv"), true);

            //application state
            NvMapModel.HighScores = highscoresVm.HighScores;
            NvMapModel.LastScores = latestScoresVm.HighScores;
            NvMapModel.ModeChampions = modeChampsVm.HighScores;

            //reorder the rows before saving
            foreach (var tab in adjustmentsVm.TabViewModels)
            {
                tab.NvMapObjectViewModels = new ObservableCollection<WPFPrismTemplate.Base.Models.NvMapObjectEntry>(tab.NvMapObjectViewModels.OrderBy(x => x.Key).ToList());
            }
            foreach (var tab in auditsVm.TabViewModels)
            {
                tab.NvMapObjectViewModels = new ObservableCollection<WPFPrismTemplate.Base.Models.NvMapObjectEntry>(tab.NvMapObjectViewModels.OrderBy(x => x.Key).ToList());
            }

            NvMapModel.Adjustments = adjustmentsVm.TabViewModels;
            NvMapModel.Audits = auditsVm.TabViewModels;
            var appJson = ParseNVRAM.SerializeWithCustomIndenting(NvMapModel, 2);
            File.WriteAllText(Path.Combine(dir, $"{NvMapModel.NvRamFile.FileNoExt}.mapsave"), appJson);
        }

        private void OnFindLatestScoresCommand()
        {
            if (NvMapModel.NvRamFile?.HexString?.Length > 0)
            {
                ResolveViewModels();

                //last score search
                foreach (var score in latestScoresVm?.HighScores)
                {
                    if (string.IsNullOrWhiteSpace(score.Score.ScoreEntry))
                    {
                        continue;
                    }

                    //get results from hex and set a default selected result.
                    var results = ParseNVRAM.SearchNvRamHexString(NvMapModel.NvRamFile.HexString, score.Score.ScoreEntry, score.Length, score.Encoding);
                    if (results?.Count() > 0)
                    {
                        score.Score.SearchResults = new ObservableCollection<SearchedNvResultVm>(
                            results.Select(x => new SearchedNvResultVm
                            {
                                Offset = x.Offset,
                                SearchVal = x.SearchVal,
                                Length = x.Length
                            }));
                        score.Score.SelectedResult = score.Score.SearchResults.Last();
                    }
                }

                //run this here because users can forget to and you can't preview output until you do this
                OnCreateMappingJson();
                CanExecuteMappingJson = true;
            }
        }

        private void OnFindHighScoresCommand()
        {
            if (NvMapModel.NvRamFile?.HexString?.Length > 0)
            {
                ResolveViewModels();

                //high score search
                foreach (var score in highscoresVm?.HighScores)
                {
                    if (string.IsNullOrWhiteSpace(score.Score.ScoreEntry))
                    {
                        continue;
                    }

                    SearchedNvResult[] scoreResults = null;
                    if (highscoresVm.CharMap?.Length > 0)
                    {
                        //Find player initials
                        scoreResults = ParseNVRAM.SearchForStringValueCharMap(NvMapModel.NvRamFile.Bytes, score.Initials, highscoresVm.CharMap.ToCharArray());
                    }
                    else if(score.Mask.HasValue)
                    {
                        scoreResults = ParseNVRAM.SearchForStringValueMasked(NvMapModel.NvRamFile.Bytes, score.Initials, score.Mask.Value);
                    }
                    else
                        scoreResults = ParseNVRAM.SearchForStringValue(NvMapModel.NvRamFile.Bytes, score.Initials);

                    if (scoreResults?.Count() > 0)
                    {
                        score.Score.SearchInitalsResults = new ObservableCollection<SearchedNvResultVm>(scoreResults.Select(x => new SearchedNvResultVm
                        {
                            Offset = x.Offset,
                            SearchVal = x.SearchVal,
                            Length = x.Length
                        }));
                        score.Score.SelectedInitialsResult = score.Score.SearchInitalsResults.Last();
                    }

                    //Find player scores
                    var results = ParseNVRAM.SearchNvRamHexString(NvMapModel.NvRamFile.HexString, score.Score.ScoreEntry, score.Length, score.Encoding);
                    if (results?.Count() > 0)
                    {
                        score.Score.SearchResults = new ObservableCollection<SearchedNvResultVm>(results.Select(x => new SearchedNvResultVm
                        {
                            Offset = x.Offset,
                            SearchVal = x.SearchVal,
                            Length = x.Length
                        }));
                        score.Score.SelectedResult = score.Score.SearchResults.Last();
                    }
                }

                //run this here because users can forget to and you can't preview output until you do this
                OnCreateMappingJson();
                CanExecuteMappingJson = true;
            }
        }

        private void OnFindModeChampsCommand()
        {
            if (NvMapModel.NvRamFile?.HexString?.Length > 0)
            {
                ResolveViewModels();
                foreach (var score in modeChampsVm?.HighScores)
                {
                    if (string.IsNullOrWhiteSpace(score.Score.ScoreEntry))
                    {
                        continue;
                    }

                    //Find player initials
                    var scoreResults = ParseNVRAM.SearchForStringValue(NvMapModel.NvRamFile.Bytes, score.Initials);
                    if (scoreResults?.Count() > 0)
                    {
                        score.Score.SearchInitalsResults = new ObservableCollection<SearchedNvResultVm>(scoreResults.Select(x => new SearchedNvResultVm
                        {
                            Offset = x.Offset,
                            SearchVal = x.SearchVal,
                            Length = x.Length
                        }));
                        score.Score.SelectedInitialsResult = score.Score.SearchInitalsResults.Last();
                    }

                    //Find player scores
                    var results = ParseNVRAM.SearchNvRamHexString(NvMapModel.NvRamFile.HexString, score.Score.ScoreEntry, score.Length, score.Encoding);
                    if (results?.Count() > 0)
                    {
                        score.Score.SearchResults = new ObservableCollection<SearchedNvResultVm>(results.Select(x => new SearchedNvResultVm
                        {
                            Offset = x.Offset,
                            SearchVal = x.SearchVal,
                            Length = x.Length
                        }));
                        score.Score.SelectedResult = score.Score.SearchResults.Last();
                    }
                }

                //run this here because users can forget to and you can't preview output until you do this
                OnCreateMappingJson();
                CanExecuteMappingJson = true;
            }
        }

        private void OnLoadNvDataCommand()
        {
            if (!string.IsNullOrWhiteSpace(NvMapModel.NvRamFile?.FileName))
            {
                var appNvDir = $"./{NvMapModel.NvRamFile.FileNoExt}";
                var droppedFileName = NvMapModel.NvRamFile.FileName;
                Directory.CreateDirectory(appNvDir);

                //save the NV file because user could be dragging from their NvRam directory, so we always want the latest
                //skip copy if using same directory file
                var fileToCopy = Path.Combine(appNvDir, $"{NvMapModel.NvRamFile.FileNoExt}.nv");
                if(fileToCopy !=droppedFileName)
                    File.Copy(droppedFileName, fileToCopy, true);

                //reset the view models and metadata
                NvMapModel.Reset();
                highscoresVm?.HighScores?.Clear();
                latestScoresVm?.HighScores?.Clear();
                adjustmentsVm?.TabViewModels?.Clear();
                auditsVm?.TabViewModels?.Clear();
                modeChampsVm?.HighScores.Clear();
                if(mappingVm != null)
                {
                    mappingVm.NvRamMapJson = string.Empty;
                }                

                var appStateFile = $"{appNvDir}/{NvMapModel.NvRamFile.FileNoExt}.mapsave";
                if (File.Exists(appStateFile))
                {
                    var appState = File.ReadAllText(appStateFile);
                    if (string.IsNullOrWhiteSpace(appState)) return;

                    var obj = JsonSerializer.Deserialize<NvRamMappingApp>(appState);
                    if (obj != null)
                    {
                        NvMapModel = obj;
                        NvMapModel.NvRamFile.FileName = droppedFileName;
                        
                        ResolveViewModels();

                        if (NvMapModel.HighScores != null)
                        {
                            foreach (var item in NvMapModel.HighScores)
                            {
                                highscoresVm.HighScores.Add(item);
                            }
                        }

                        if (NvMapModel.LastScores != null)
                        {
                            foreach (var item in NvMapModel.LastScores)
                            {
                                latestScoresVm.HighScores.Add(item);
                            }
                        }

                        if (NvMapModel.Adjustments != null)
                        {                            
                            foreach (var item in NvMapModel.Adjustments)
                            {
                                adjustmentsVm.TabViewModels.Add(item);
                            }
                        }
                        
                        if (NvMapModel.Audits != null)
                        {                            
                            foreach (var item in NvMapModel.Audits)
                            {
                                auditsVm.TabViewModels.Add(item);
                            }
                        }

                        if (NvMapModel.ModeChampions != null)
                        {
                            foreach (var item in NvMapModel.ModeChampions)
                            {
                                modeChampsVm.HighScores.Add(item);
                            }
                        }

                        NvMapModel.HighScores = null;
                        NvMapModel.LastScores = null;
                    }
                }

                NvMapModel.NvRamFile.Bytes = ParseNVRAM.NvRamFileToBytes(NvMapModel.NvRamFile.FileName);
                NvMapModel.NvRamFile.HexString = ParseNVRAM.NvRamFileToHexString(NvMapModel.NvRamFile.FileName);
            }
        }

        private async void OnPreviewOutputCommand(string section = null)
        {
            //var mappingVm = unityContainer.Resolve(typeof(MappingViewModel)) as MappingViewModel;
            if (!string.IsNullOrWhiteSpace(mappingVm?.NvRamMapJson))
            {
                try
                {
                    var mapped = ParseNVRAM.DeserializeNvJsonMapping(mappingVm?.NvRamMapJson);
                    var parser = new ParseNVRAM(mapped, NvMapModel.NvRamFile.Bytes);
                    string results = string.Empty;

                    if (section != null)
                    {
                        switch (section)
                        {
                            case "Audits":
                                results = parser.ExportAudits();
                                break;
                            case "Adjustments":
                                results = parser.ExportAdjustments();
                                break;
                            default:
                                break;
                        }

                        await dialog?.ShowMessageAsync(this, section, results);
                    }
                    else
                    {
                        results = parser.ExportScoreResults();
                        await dialog?.ShowMessageAsync(this, "Scores", results);
                    }
                }
                catch (Exception ex)
                {
                    await dialog?.ShowMessageAsync(this, "ERROR", ex.Message);
                }
            }
        }

        /// <summary>
        /// Resolves all view models used, only need to do this once but after they have been created
        /// </summary>
        void ResolveViewModels()
        {
            if (!_viewModelsResolved)
            {
                //get the data from the other viewmodels
                highscoresVm = unityContainer.Resolve(typeof(HighScoresViewModel)) as HighScoresViewModel;
                latestScoresVm = unityContainer.Resolve(typeof(LatestScoresViewModel)) as LatestScoresViewModel;
                modeChampsVm = unityContainer.Resolve(typeof(ModeChampionsViewModel)) as ModeChampionsViewModel;
                mappingVm = unityContainer.Resolve(typeof(MappingViewModel)) as MappingViewModel;
                adjustmentsVm = unityContainer.Resolve(typeof(AdjustmentsViewModel)) as AdjustmentsViewModel;
                auditsVm = unityContainer.Resolve(typeof(AuditsViewModel)) as AuditsViewModel;
                _viewModelsResolved = true;
            }
        }
        #region Drag Drop Implementations
        public void DragEnter(IDropInfo dropInfo) { }
        public void DragLeave(IDropInfo dropInfo) { }

        public void DragOver(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as IDataObject;
            // look for drag&drop new files
            if (dataObject != null && dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            // look for drag&drop new files
            if (dataObject != null && dataObject.ContainsFileDropList())
            {
                this.HandleDropActionAsync(dropInfo, dataObject.GetFileDropList());
            }
            else
            {
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
                var data = dropInfo.Data;
                // do something with the data
            }
        }

        private void HandleDropActionAsync(IDropInfo dropInfo, StringCollection stringCollection)
        {
            _file = stringCollection[0];
            var ext = Path.GetExtension(_file);
            if (ext == ".nv")
            {
                FileDropHeader = $"NVRAM - {Path.GetFileName(_file)}";
                if (NvMapModel.NvRamFile == null)
                {
                    NvMapModel.NvRamFile = new NvRamFile();
                }
                NvMapModel.NvRamFile.FileName = _file;
                NvMapModel.NvRamFile.FileSize = new FileInfo(_file)?.Length ?? 0;
                NvMapModel.NvRamFile.FileNoExt = Path.GetFileNameWithoutExtension(_file);

                //get the rom name or if already there keep it
                NvMapModel.Roms = !string.IsNullOrWhiteSpace(NvMapModel.Roms) ? NvMapModel.Roms : NvMapModel.NvRamFile.FileNoExt;

                NvMapModel.NvRamFile.HexString = ParseNVRAM.NvRamFileToHexString(_file);
                NvMapModel.NvRamFile.Bytes = ParseNVRAM.NvRamFileToBytes(_file);
                NvMapModel.RamSize = NvMapModel.NvRamFile.FileSize;

                HasDroppedFile = true;
                OnLoadNvDataCommand();
            }
            else if (_file.EndsWith("nv.json"))
            {

            }
            else
            {
                _file = string.Empty;
                FileDropHeader = "NVRAM FILE DROP";
            }
        }
        #endregion
    }
}
