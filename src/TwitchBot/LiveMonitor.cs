using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace TwitchBot
{

	public class LiveMonitor
	{

		private LiveStreamMonitorService Monitor;
		private TwitchAPI API;

		private TableClient tableClient;

		public LiveMonitor()
		{
			tableClient = new TableClient(new Uri(Settings.StorageUri),
					"StreamSnapshot",
					new TableSharedKeyCredential(Settings.StorageAccountName, Settings.StroageAccountKey));

			Task.Run(() => ConfigLiveMonitorAsync());
		}

		public void StopMonitoring()
		{
			Monitor.Stop();
		}

		private async Task ConfigLiveMonitorAsync()
		{

			API = new TwitchAPI();

			API.Settings.ClientId = Settings.ClientId;
			API.Settings.AccessToken = Settings.AccessToken;

			Monitor = new LiveStreamMonitorService(API, 10);

			Monitor.OnServiceStarted += Monitor_OnServiceStarted;
			Monitor.OnServiceStopped += Monitor_OnServiceStopped;
			Monitor.OnServiceTick += Monitor_OnServiceTick;
			Monitor.OnChannelsSet += Monitor_OnChannelsSet;

			List<string> lst = new List<string> { Settings.ChannelName, "TaleLearnCode" };
			Monitor.SetChannelsByName(lst);

			Monitor.OnStreamOnline += Monitor_OnStreamOnline;
			Monitor.OnStreamOffline += Monitor_OnStreamOffline;
			Monitor.OnStreamUpdate += Monitor_OnStreamUpdate;

			Monitor.Start();
			await Task.Delay(-1);

		}

		private void Monitor_OnServiceTick(object sender, OnServiceTickArgs e)
		{
			Console.WriteLine($">> Service Tick");
		}

		private void Monitor_OnServiceStopped(object sender, OnServiceStoppedArgs e)
		{
			Console.WriteLine($">> Stopping the monitor service");
		}

		private void Monitor_OnChannelsSet(object sender, OnChannelsSetArgs e)
		{
			Console.WriteLine($">> Monitoring {e.Channels.Count} channels");
		}

		private void Monitor_OnServiceStarted(object sender, OnServiceStartedArgs e)
		{
			Console.WriteLine(">> Starting the monitor service");
		}

		private void Monitor_OnStreamUpdate(object sender, OnStreamUpdateArgs e)
		{

			//Console.WriteLine($">> Stream Update");
			//Console.WriteLine($"\tGameId:\t{e.Stream.GameId}");
			//Console.WriteLine($"\tId:\t{e.Stream.Id}");
			//Console.WriteLine($"\tLanguage:\t{e.Stream.Language}");
			//Console.WriteLine($"\tStartedAt:\t{e.Stream.StartedAt}");
			//Console.WriteLine($"\tThumbnailUrl\t{e.Stream.ThumbnailUrl}");
			//Console.WriteLine($"\tTitle\t{e.Stream.Title}");
			//Console.WriteLine($"\tType\t{e.Stream.Type}");
			//Console.WriteLine($"\tUserId\t{e.Stream.UserId}");
			//Console.WriteLine($"\tUserName\t{e.Stream.UserName}");
			//Console.WriteLine($"\tViewerCount\t{e.Stream.ViewerCount}");

			//_ = GetGameAsync(e.Stream.GameId);

			var snapshot = new StreamSnapshot(e.Stream, API);


			var entity = new TableEntity(e.Stream.UserName, Guid.NewGuid().ToString())
			{
				{ "Product", "Marker Set" },
				{ "Price", 5.00 },
				{ "Quantity", 21 }
			};

			tableClient.AddEntity(snapshot);
			Console.WriteLine($"Stored Snapshop {snapshot.RowKey}");

		}

		private void Monitor_OnStreamOffline(object sender, OnStreamOfflineArgs e)
		{
			Console.WriteLine($">> Stream {e.Stream.Id} has gone offline");
		}

		private void Monitor_OnStreamOnline(object sender, OnStreamOnlineArgs e)
		{
			Console.WriteLine($">> Stream {e.Stream.Id} has gone online");
		}

		private async Task GetGameAsync(string gameId)
		{
			List<string> lst = new List<string> { gameId };
			var x = await API.Helix.Games.GetGamesAsync(lst);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(x.Games[0].BoxArtUrl);
			Console.ForegroundColor = ConsoleColor.White;

		}



	}
}