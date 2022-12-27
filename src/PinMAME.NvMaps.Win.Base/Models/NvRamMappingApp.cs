using PinMAME.NvMaps.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WPFPrismTemplate.Base.Models;

namespace PinMAME.NvMaps.Win.Base.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class NvRamMappingApp
    {
        public string Notes { get; set; }
        public string Copyright { get; set; }
        public string License { get; set; }
        public string Endian { get; set; }
        public float FileFormat { get; set; }
        public float Version { get; set; }
        public long RamSize { get; set; }
        public NvRamFile NvRamFile { get; set; } = new NvRamFile();
        public ObservableCollection<HighScoreBase> LastScores { get; set; }
        public ObservableCollection<HighScore> HighScores { get; set; }
        public string Roms { get; set; }
        public ObservableCollection<TabViewModel> Adjustments { get; set; }
        public ObservableCollection<TabViewModel> Audits { get; set; }
        public ObservableCollection<HighScore> ModeChampions { get; set; }

        /// <summary>
        /// Creates a mapping for the NvParser json mappings with the properties of this model
        /// </summary>
        /// <returns></returns>
        public NvRamMap CreateNewMap()
        {
            var roms = Roms?.Split(',');

            return new NvRamMap
            {
                _copyright = Copyright,
                _endian = Endian,
                _license = License,
                _notes = new List<string> { Notes },
                _roms = roms.ToList(),
                _version = Version,
                _fileformat = FileFormat,
                _ramsize = RamSize,
                last_game = new List<LastGame>(),
                high_scores = new List<Model.HighScore>(),
                mode_champions = new List<Model.HighScore>(),
                adjustments = new Dictionary<string, Dictionary<string, NvMapObject>>(),
                audits = new Dictionary<string, Dictionary<string, NvMapObject>>()
            };
        }

        public void Reset()
        {
            Notes = "Compiled by NV Mapping Helper (Replace with your name)";
            Copyright = "Copyright (C) 2022 by YourName your email>";
            License = "GNU Lesser General Public License v3.0";
            Endian = "big";
            FileFormat = 0.1f;
            Version = 0.1f;
        }
    }
}
