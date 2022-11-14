using DynamicWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using The_Web_Server.Models;

namespace DynamicWebApp.Controllers
{
    public class HomeController : Controller
    { 
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Clients()
        {
            string URL = "https://localhost:44381/";
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("api/clientsTables", Method.Get);
            RestResponse response = client.Execute(request);
            List<clientsTable> clients = JsonConvert.DeserializeObject<List<clientsTable>>(response.Content);
            
            return Ok(clients);
        }

        [HttpGet]
        public IActionResult Jobs()
        {
            string URL = "https://localhost:44381/";
            RestClient client = new RestClient(URL);
            RestRequest request = new RestRequest("api/jobsTables", Method.Get);
            RestResponse response = client.Execute(request);
            List<jobsTable> jobs = JsonConvert.DeserializeObject<List<jobsTable>>(response.Content);

            return Ok(jobs);
        }
    }
}