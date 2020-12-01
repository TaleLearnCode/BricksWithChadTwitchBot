using Azure;
using Azure.Data.Tables;
using System;
using System.Linq;

namespace TwitchBot
{

	public class ProjectTracking : ITableEntity
	{
		public string PartitionKey { get; set; }  // ChannelName
		public string RowKey { get; set; } // ProjectName
		public DateTimeOffset? Timestamp { get; set; }
		public ETag ETag { get; set; }

		public int BricksDropped { get; set; }
		public int Oofs { get; set; }

		public ProjectTracking() { }

		public ProjectTracking(string channelName, string projectName)
		{
			PartitionKey = channelName;
			RowKey = projectName;
		}

		private static TableClient GetTableClient()
		{
			TableClient tableClient;
			tableClient = new TableClient(new Uri(Settings.StorageUri),
					"BricksWithChad",
					new TableSharedKeyCredential(Settings.StorageAccountName, Settings.StroageAccountKey));
			return tableClient;
		}

		public static ProjectTracking Retrieve(string channelName, string projectName)
		{

			ProjectTracking projectTracking = GetTableClient().Query<ProjectTracking>(s => s.PartitionKey == channelName && s.RowKey == projectName).FirstOrDefault();

			return projectTracking;

		}

		public void Save()
		{
			if (!string.IsNullOrWhiteSpace(PartitionKey) && !string.IsNullOrWhiteSpace(RowKey))
				GetTableClient().UpsertEntity(this);
		}

	}
}