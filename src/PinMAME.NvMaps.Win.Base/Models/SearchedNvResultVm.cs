using Prism.Mvvm;
using System;

namespace PinMAME.NvMaps.Win.Base.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class SearchedNvResultVm
    {
        /// <summary>
        /// Index value was found
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// Length, given by search value
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// The value used on the search
        /// </summary>
        public string SearchVal { get; set; }
    }
}
