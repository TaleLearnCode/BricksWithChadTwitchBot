using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams;

namespace TwitchBot
{
	public class StreamSnapshot : Stream, ITableEntity
	{

		//public string Id { get; set; }
		//public string UserId { get; set; }
		//public string UserName { get; set; }
		//public string GameId { get; set; }
		//public string CommunityId { get; set; }
		//public string[] CommunityIds { get; set; }
		//public string Type { get; set; }
		//public string Title { get; set; }
		//public int ViewerCount { get; set; }
		//public DateTime StartedAt { get; set; }
		//public string Language { get; set; }
		//public string ThumbnailUrl { get; set; }

		public DateTime DateTime { get; set; }
		public string GameName { get; set; }
		public string GameBoxArtUrl { get; set; }
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }

		public StreamSnapshot() { }

		public StreamSnapshot(Stream stream, TwitchAPI twitchAPI)
		{

			DateTime = DateTime.UtcNow;

			Id = stream.Id;
			UserId = stream.UserId;
			UserName = stream.UserName;
			GameId = stream.GameId;
			CommunityIds = stream.CommunityIds;
			Type = stream.Type;
			Title = stream.Title;
			ViewerCount = stream.ViewerCount;
			StartedAt = stream.StartedAt;
			Language = stream.Language;
			ThumbnailUrl = stream.ThumbnailUrl;

			var t = Task.Run(async () => await GetGameAsync(twitchAPI, stream.GameId));
			t.Wait();

			PartitionKey = stream.UserName;
			RowKey = DateTime.Ticks.ToString();

		}

		private async Task GetGameAsync(TwitchAPI twitchAPI, string gameId)
		{
			List<string> lst = new List<string> { gameId };
			var x = await twitchAPI.Helix.Games.GetGamesAsync(lst);
			GameName = x.Games[0].Name;
			GameBoxArtUrl = x.Games[0].BoxArtUrl;
		}


	}
}