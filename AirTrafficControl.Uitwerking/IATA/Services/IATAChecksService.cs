using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace IATA
{
    public class IATAChecksService : IATAChecks.IATAChecksBase
    {
        private static readonly string[] mainType = { "747", "727", "787" };
        private static readonly string[] subType = { "200", "200B", "300", "400" };
        private readonly List<Airline> airLines;
        private readonly List<AirTrafficController> trafficControllers;
        private readonly ILogger<IATAChecksService> _logger;
        public IATAChecksService(ILogger<IATAChecksService> logger)
        {
            _logger = logger;
            if (trafficControllers == null)
            {
                var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"{"Data\\controllers.json"}");
                var JSON = System.IO.File.ReadAllText(folderDetails);
                trafficControllers = JsonSerializer.Deserialize<List<AirTrafficController>>(JSON);
            }

            if (airLines == null)
            {
                var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"{"Data\\airlines.json"}");
                var JSON = System.IO.File.ReadAllText(folderDetails);
                airLines = JsonSerializer.Deserialize<List<Airline>>(JSON);
            }

        }

        public override Task<CheckCallsignReply> CheckCallsign(CheckCallsignRequest request, ServerCallContext context)
        {
            var found = trafficControllers.Find(p => p.Callsign == request.Callsign);
            if (found != null)
            {
                return Task.FromResult(new CheckCallsignReply
                {
                    Callsign = request.Callsign,
                    FirstName = found?.FirstName,
                    LastName = found?.LastName
                });
            }
            else
            return Task.FromResult(new CheckCallsignReply
            {
                Callsign = request.Callsign,
            }); 
        }

        public override Task<AirTrafficControllersReply> GetAirTrafficControllers(AirTrafficControllersRequest request, ServerCallContext context)
        {
            var result = new AirTrafficControllersReply();
            result.AirTrafficController.AddRange(trafficControllers.Select(al => new AirTrafficController { Callsign = al.Callsign, FirstName = al.FirstName, LastName = al.LastName}));
            return Task.FromResult(result);
        }

        public override Task<FlightInfoReply> GetFlightInformation(FlightInfoRequest request, ServerCallContext context)
        {
            return Task.FromResult(new FlightInfoReply
            {
                AircraftType = RandomAircraftType(),
                Airline = GetAirlineName(request.AirlineCode),
                IATAFlightNumber = request.IATAFlightNumber
            });
        }

        public override Task<AirlinesReply> GetAirlines(AirlinesRequest request, ServerCallContext context)
        {
           
            var result = new AirlinesReply();
            result.Airline.AddRange(airLines.Select(al => new Airline { Code = al.Code, Name = al.Name }));
            return Task.FromResult(result);
        }

        private string RandomAircraftType()
        {
            var randMain = new Random();
            return $"Boeing {mainType[randMain.Next(0, mainType.Length - 1)]}-{subType[randMain.Next(0, subType.Length - 1)]}";
        }

        private string RandomAirlineCode()
        {
            var randMain = new Random();
            var randomPosition = randMain.Next(0, airLines.Count - 1);
            return airLines[randomPosition].Code;
        }

        private string GetAirlineName(string codePartial)
        {
            string result = airLines.FirstOrDefault(a => a.Code.Contains(codePartial))?.Name;
            return result ?? "Unknown Airline";
        }

    }
}
