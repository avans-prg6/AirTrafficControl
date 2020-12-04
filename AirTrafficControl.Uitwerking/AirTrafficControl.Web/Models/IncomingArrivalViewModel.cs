using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AirTrafficControl.Web.Models
{
    public class IncomingArrivalViewModel : IValidatableObject
    {
        [RegularExpression(@"\d{1,4}\b")]
        [Display(Name = "Incoming Flight Details")]
        [Required]
        public string IATAFlightNumber { get; set; }

        public string AircraftType { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }

        [HandlerValidation]
        [DisplayName("Assigned Handler")]
        //[StringLength(15, MinimumLength = 3)]
        [Required]
        public string TowerCallsign { get; set; }


        [DisplayName("Requesting Landing Permission at")]
        [Required(ErrorMessage = "Please report your expected arrival time." )]
        public DateTime InitialArrivalTime { get; set; }

        /// <summary>
        /// Checks validation rules that involve multiple attributes of the model
        /// </summary>
        /// <returns> zero or more validation results</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
                var _service = (IATA.IATAChecks.IATAChecksClient)validationContext.GetService<IATA.IATAChecks.IATAChecksClient>();
                var foundflight = _service.GetFlightInformation(new IATA.FlightInfoRequest { IATAFlightNumber = this.IATAFlightNumber, AirlineCode = AirlineCode });
                if (string.IsNullOrWhiteSpace(foundflight.AircraftType))
                    yield return new ValidationResult($"Flight number '{IATAFlightNumber}' for airline '{AirlineCode}' is not registered with airport.");
                else
                {
                    AirlineName = foundflight.Airline;
                    AircraftType = foundflight.AircraftType;
                    // Boeing 747 and subtypes are not allowed to land after 14.00 due to noise constrictions
                    if (AircraftType.Contains("747") && InitialArrivalTime.TimeOfDay > new System.TimeSpan(14, 0, 0))
                        yield return new ValidationResult(
                       $"Flight 'Aircrafts of the {AircraftType} are not allowed to land after 14.00.",
                       new[] { nameof(AircraftType) });

                }

                // Handlers with a callsign that contains part of the name of an airline
                // are not allowed to process landing requests for that airline
                if (TowerCallsign.Split(' ').Any(p => foundflight.Airline.Contains(p)))
                    yield return new ValidationResult(
                   $"Assigned Handler (Air Traffic Controller with callsign {TowerCallsign} is not allowed to handle arrival of {IATAFlightNumber}. Airline name {foundflight.Airline} contains part of his callsign and might cause confusion.",
                   new[] { nameof(TowerCallsign) });
        }
    }

    /// <summary>
    /// Checks if TowerCallSign is a registered callsign
    /// </summary>
    public class HandlerValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _service = (IATA.IATAChecks.IATAChecksClient)validationContext.GetService<IATA.IATAChecks.IATAChecksClient>();
            if (value == null || string.IsNullOrWhiteSpace((string)value))
                return new ValidationResult($"No handler selected");
            else
            {
                 var handler = _service.CheckCallsign(new IATA.CheckCallsignRequest { Callsign = (string)value});
                    if (string.IsNullOrEmpty(handler.LastName))
                     return new ValidationResult($"Callsign for handler '{value}' is not registered with IATA.");
            }
            return ValidationResult.Success;
        }
    }



}

