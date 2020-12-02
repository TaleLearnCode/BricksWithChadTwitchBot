using Azure;
using Azure.Data.Tables;
using System;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams;


namespace TwitchMonitor
{
	public class StreamSnapshot : ITableEntity
	{

		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }


		public string StreamId { get; set; }
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string GameId { get; set; }
		public string Type { get; set; }
		public string Title { get; set; }
		public int ViewerCount { get; set; }
		public DateTime StartedAt { get; set; }
		public string Language { get; set; }

		public StreamSnapshot() { }

		public StreamSnapshot(Stream stream, TwitchAPI twitchAPI)
		{

			StreamId = stream.Id;
			UserId = stream.UserId;
			UserName = stream.UserName;
			GameId = stream.GameId;
			Type = stream.Type;
			Title = stream.Title;
			ViewerCount = stream.ViewerCount;
			StartedAt = stream.StartedAt;
			Language = stream.Language;

			PartitionKey = stream.UserName;
			RowKey = DateTime.UtcNow.Ticks.ToString();

		}

	}

}