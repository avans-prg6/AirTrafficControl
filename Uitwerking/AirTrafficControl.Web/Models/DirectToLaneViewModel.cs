using System;
using System.ComponentModel.DataAnnotations;

namespace AirTrafficControl.Web.Models
{
    public class DirectToLaneViewModel
    {
        [Display(Name = "You are redirected to runway :")]
        public string Lane { get; set; }
        [Display(Name = "Your estimated time of arrival is :")]
        public DateTime PermissionTime { get; set; }
        [Display(Name = "Change frequency (in Mhz) to :")]
        public string TowerContactFrequency { get; set; }
        [Display(Name = "Welcome flight :")]
        public string AircraftCode { get; set; }
        [Display(Name = "Please contact :")]
        public string Handler { get; set; }
        [Display(Name = "Landing fees will be charged to :")]
        public string Airline { get; set; }
        [Display(Name = "Your registered aircraft type :")]
        public string AircraftType { get; set; }
    }
}
