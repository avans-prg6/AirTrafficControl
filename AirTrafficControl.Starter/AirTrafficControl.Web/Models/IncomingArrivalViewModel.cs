using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AirTrafficControl.Web.Models
{
    //TODO: Add all the validation attributes
    public class IncomingArrivalViewModel : IValidatableObject
    {
        [Required]
        public string IATAFlightNumber { get; set; }

        public string TowerCallsign { get; set; }
        public DateTime InitialArrivalTime { get; set; }
        public string AircraftType { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }

      
        /// <summary>
        /// Checks validation rules that involve multiple attributes of the model
        /// </summary>
        /// <returns> zero or more validation results</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //TODO: ADD CUSTOM VALIDATION
            // regel hieronder verwijderen zodra je jouw validaties gaat toevoegen.
            yield return ValidationResult.Success;
        }
    }
}

