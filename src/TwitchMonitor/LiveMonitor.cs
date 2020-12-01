using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;


namespace TwitchMonitor
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

			var snapshot = new StreamSnapshot(e.Stream, API);

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

	}

}