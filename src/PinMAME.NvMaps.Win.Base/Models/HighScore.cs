using PropertyChanged;

namespace PinMAME.NvMaps.Win.Base.Models
{
    [AddINotifyPropertyChangedInterface]
    public class HighScoreBase
    {
        public bool Packed { get; set; } = true;
        public string Encoding { get; set; } = "bcd";
        /// <summary>
        /// Set length of high score
        /// </summary>
        public byte Length { get; set; } = 4;
        public byte LengthInitials { get; set; } = 3;
        public int? Mask { get; set; }
        public ScoreSearch Score { get; set; } = new ScoreSearch();
    }

    public class HighScore : HighScoreBase
    {
        public string Label { get; set; }
        public string ShortLabel { get; set; }
        public string Initials { get; set; }
    }
}