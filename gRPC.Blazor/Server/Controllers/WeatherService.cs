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
		public override async Task GetWeather(IAsyncStreamReader<WeatherForecast> requestStream, IServerStreamWriter<WeatherReply> responseStream, ServerCallContext context)
		{
			while (await requestStream.MoveNext())
			{
				var request = requestStream.Current;
				var rng = new Random();
				for (int i = 0; i < 100; i++)
				{
					await Task.Delay(10000);
					var reply = new WeatherReply();
					reply.Forecasts.Add(Enumerable.Range(1, 1).Select(index => new WeatherForecast
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
}
