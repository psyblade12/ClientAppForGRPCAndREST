using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Mvc;
using MyApi.Grpc;
using System.Diagnostics;
using System.Text;

namespace TestAppForGRPCAndREST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Test")]
        public async Task<string> Get(string host = "")
        {
            var returnStringBuilder = new StringBuilder();

            //REST API....
            returnStringBuilder.Append($"Rest API..... \r\n");

            using var client = new HttpClient(new HttpClientHandler());

            List<string> listName = new List<string>() { "Beijing", "Tokyo", "New York", "London", "Seoul", "Munich", "Berlin", "Bacu", "Bangcock", "A", "B", "C", "D", "E", "F" };
            foreach (var name in listName)
            {
                Stopwatch sw = Stopwatch.StartNew();
                sw.Start();
                var response2 = await client.GetStringAsync($"{host}/weatherForecast/ReturnHello?name={name}");
                sw.Stop();

                returnStringBuilder.Append($"{response2}. Elapsed time: {sw.ElapsedMilliseconds} \r\n");
            }


            //GRPC Native...
            returnStringBuilder.Append($"Native GRPC..... \r\n");

            using var channel = GrpcChannel.ForAddress(host, new GrpcChannelOptions
            {
                HttpHandler = new HttpClientHandler()
            });

            var clientGRPC = new WeatherService.WeatherServiceClient(channel);

            List<string> listCity = new List<string>() { "Beijing", "Tokyo", "New York", "London", "Seoul", "Munich", "Berlin", "Bacu", "Bangcock", "A", "B", "C", "D", "E", "F" };
            foreach (var city in listCity)
            {
                Stopwatch sw = Stopwatch.StartNew();
                sw.Start();
                var response2 = await clientGRPC.GetWeatherAsync(new WeatherRequest { City = city });
                sw.Stop();

                returnStringBuilder.Append($"{response2.Forecast}. Elapsed time: {sw.ElapsedMilliseconds}  \r\n");
            }

            //GRPC-web...
            returnStringBuilder.Append($"GRPC-web..... \r\n");

            using var channelGRPCWeb = GrpcChannel.ForAddress(host, new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler())
            });

            var clientGRPCweb = new WeatherService.WeatherServiceClient(channelGRPCWeb);

            List<string> listCity2 = new List<string>() { "Beijing", "Tokyo", "New York", "London", "Seoul", "Munich", "Berlin", "Bacu", "Bangcock", "A", "B", "C", "D", "E", "F" };
            foreach (var city in listCity2)
            {
                Stopwatch sw = Stopwatch.StartNew();
                sw.Start();
                var response2 = await clientGRPCweb.GetWeatherAsync(new WeatherRequest { City = city });
                sw.Stop();

                returnStringBuilder.Append($"{response2.Forecast}. Elapsed time: {sw.ElapsedMilliseconds}  \r\n");
            }


            return returnStringBuilder.ToString();
        }
    }
}
