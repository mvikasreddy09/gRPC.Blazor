using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using gRPC.Blazor.Server.Proto;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace gRPC.Blazor.Server.Controllers
{
	public class WeatherService : WeatherForecasts.WeatherForecastsBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};
        public override async Task<WeatherReply> GetWeather(WeatherRequest request, ServerCallContext context)
        {
			var reply = new WeatherReply();
			var rng = new Random();
			for (int i = 0; i < request.Loops; i++)
			{
				context.CancellationToken.ThrowIfCancellationRequested();
				await Task.Delay(request.TimeOut);
				reply.Forecasts.Add(Enumerable.Range(1, request.NoOfRecordsPerLoop).Select(index => new WeatherForecast
				{
					DateTimeStamp = new Timestamp() { Seconds = DateTime.Now.AddDays(index).Second },
					TemperatureC = rng.Next(20, 55),
					Summary = Summaries[rng.Next(Summaries.Length)]
				}));
			}
			return reply;
		}
        public override async Task GetWeatherStream(WeatherRequest request, IServerStreamWriter<WeatherReply> responseStream, ServerCallContext context)
        {
			var rng = new Random();
			for (int i = 0; i < request.Loops; i++)
			{
				context.CancellationToken.ThrowIfCancellationRequested();
				await Task.Delay(request.TimeOut);
				var reply = new WeatherReply();
				reply.Forecasts.Add(Enumerable.Range(1, request.NoOfRecordsPerLoop).Select(index => new WeatherForecast
				{
					DateTimeStamp = new Timestamp() { Seconds = DateTime.Now.AddDays(index).Second },
					TemperatureC = rng.Next(20, 55),
					Summary = Summaries[rng.Next(Summaries.Length)]
				}));
				await responseStream.WriteAsync(reply);
			}
		}
    }
}
