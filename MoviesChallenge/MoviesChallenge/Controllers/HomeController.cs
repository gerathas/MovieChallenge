using Microsoft.AspNetCore.Mvc;
using MoviesChallenge.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Diagnostics;

namespace MoviesChallenge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var client = new RestClient("https://moviesdatabase.p.rapidapi.com");

            // API call to get Actor Info
            var actorsRequest = new RestRequest("actors?limit=10");
            actorsRequest.AddHeader("X-RapidAPI-Key", "b1c960a40emsh0793366e5909fabp1c766cjsn2b60cb37886c");
            actorsRequest.AddHeader("X-RapidAPI-Host", "moviesdatabase.p.rapidapi.com");
            var actorResponse = client.Execute(actorsRequest);
            var actorsContent = JObject.Parse(actorResponse.Content);

            JArray actorsJArray = (JArray)actorsContent["results"];
            List<ActorModel> actors = new List<ActorModel>();

            for (int i = 0; i < actorsJArray.Count; i++)
            {
                // Another API Call to convert movie codes into title name
                string movieCodes = (string)actorsJArray[i]["knownForTitles"];
                var moviesRequest = new RestRequest("titles/x/titles-by-ids?idsList=" + movieCodes);
                moviesRequest.AddHeader("X-RapidAPI-Key", "b1c960a40emsh0793366e5909fabp1c766cjsn2b60cb37886c");
                moviesRequest.AddHeader("X-RapidAPI-Host", "moviesdatabase.p.rapidapi.com");
                var moviesResponse = client.Execute(moviesRequest);
                var moviesContent = JObject.Parse(moviesResponse.Content);
                JArray moviesJArray = (JArray)moviesContent["results"];              
                string movies = "";

                for (int j = 0; j < moviesJArray.Count; j++)
                {
                    movies += moviesJArray[j]["titleText"]["text"] + ", ";
                }

                movies = movies.Remove(movies.Length - 2);
                // Sending data to model
                actors.Add(new ActorModel { Id = i, Name = (string)actorsJArray[i]["primaryName"], Movies = movies });
            }
          
            return View(actors);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult StarWars()
        {
            var client = new RestClient("https://swapi.dev/api");
            var request = new RestRequest("people");
            var response = client.Execute(request);
            var content = JObject.Parse(response.Content);
            JArray jArray = (JArray)content["results"];
            List<StarWarsModel> people = new List<StarWarsModel>();
            
            for (int i = 0; i < jArray.Count; i++)
            {
                people.Add(new StarWarsModel
                {
                    name = (string)jArray[i]["name"],
                    birth_year = (string)jArray[i]["birth_year"],
                    eye_color = (string)jArray[i]["eye_color"],
                    gender = (string)jArray[i]["gender"],
                    hair_color = (string)jArray[i]["hair_color"],
                    height = (string)jArray[i]["height"],
                    mass = (string)jArray[i]["mass"],
                    homeworld = (string)jArray[i]["homeworld"],
                    url = (string)jArray[i]["url"],
                    created = (string)jArray[i]["created"],
                    edited = (string)jArray[i]["edited"]
                });
            }

            return View(people);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}