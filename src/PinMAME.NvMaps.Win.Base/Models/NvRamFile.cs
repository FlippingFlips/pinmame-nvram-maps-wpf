using Newtonsoft.Json;
using PropertyChanged;

namespace PinMAME.NvMaps.Win.Base.Models
{
    [AddINotifyPropertyChangedInterface]
    public class NvRamFile
    {
        [JsonIgnore]
        public string FileName { get; set; }
        public long FileSize { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string HexString { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public byte[] Bytes { get; set; }
        public string FileNoExt { get; set; }
    }
}