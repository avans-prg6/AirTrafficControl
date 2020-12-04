using AirTrafficControl.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;

namespace AirTrafficControl.Web.Controllers
{
    public class AirportController : Controller
    {
        private readonly ILogger<AirportController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private IATA.IATAChecks.IATAChecksClient _service;
        private IConfiguration _configuration;
        public AirportController(ILogger<AirportController> logger, IWebHostEnvironment hostingEnvironment, IATA.IATAChecks.IATAChecksClient IATAService, IConfiguration configuration)
        {
            _logger = logger;
            _service = IATAService;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var model = new IncomingArrivalViewModel();
            ViewBag.Airlines = _service.GetAirlines(new IATA.AirlinesRequest {Filter = "" }).Airline;
            ViewBag.TrafficControllers = _service.GetAirTrafficControllers(new IATA.AirTrafficControllersRequest { Filter = "" }).AirTrafficController.Select(atc => new { callsign = atc.Callsign, displayname = $"{atc.FirstName} {atc.LastName} ({atc.Callsign})" }).ToList();
            return View("RegisterLandingRequest", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult HandleLandingRequest(IncomingArrivalViewModel incomingArrival)
        {
            _logger.LogInformation("Controller is processing a LandingRequest");

            var airportData = (Lanes: Convert.ToInt32(_configuration["Airport:Lanes"]), TowerFrequency: $"{_configuration["Airport:TowerFrequency"]} Mhz");
            if (ModelState.IsValid)
            {
                var randomLane = new Random();
                var result = new DirectToLaneViewModel();
                result.PermissionTime =  incomingArrival.InitialArrivalTime;
                result.Handler = incomingArrival.TowerCallsign;
                result.AircraftCode = $"{incomingArrival.AirlineCode}{incomingArrival.IATAFlightNumber}";
                result.Airline = incomingArrival.AirlineName;
                // direct aircraft to a random Lane
                result.Lane = randomLane.Next(1, airportData.Lanes).ToString();
                result.TowerContactFrequency = airportData.TowerFrequency;
                result.AircraftType = incomingArrival.AircraftType;
                return View("RequestApproved", result);
            }
            ViewBag.Airlines = _service.GetAirlines(new IATA.AirlinesRequest { Filter = "" }).Airline;
            ViewBag.TrafficControllers = _service.GetAirTrafficControllers(new IATA.AirTrafficControllersRequest { Filter = "" }).AirTrafficController.Select(atc => new { callsign = atc.Callsign, displayname = $"{atc.FirstName} {atc.LastName} ({atc.Callsign})" }).ToList();
            return View("RegisterLandingRequest", incomingArrival);
        }
    }
}
