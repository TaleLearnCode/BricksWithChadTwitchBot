using Azure.Data.Tables;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace TwitchBot
{
	static class TableSearch
	{

		public static void PerformSearch()
		{

			TableClient tableClient;
			tableClient = new TableClient(new Uri(Settings.StorageUri),
					"StreamSnapshot",
					new TableSharedKeyCredential(Settings.StorageAccountName, Settings.StroageAccountKey));
			var lastSnapshot = Search(tableClient, "TaleLearnCode");
			Console.WriteLine(lastSnapshot.Id);

		}

		public static StreamSnapshot Search(TableClient tableClient, string partitionKey)
		{

			//Pageable<StreamSnapshot> queryResultsFilter = tableClient.Query<StreamSnapshot>(filter: $"PartitionKey eq '{partitionKey}'");

			//// Iterate the <see cref="Pageable"> to access all queried entities.

			//foreach (StreamSnapshot qEntity in queryResultsFilter)
			//{
			//	Console.WriteLine($"{qEntity.GetString("Product")}: {qEntity.GetDouble("Price")}");
			//}

			//Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");





			// Use the <see cref="TableClient"> to query the table using a filter expression.

			//double priceCutOff = 6.00;
			//Pageable<OfficeSupplyEntity> queryResultsLINQ = tableClient.Query<OfficeSupplyEntity>(ent => ent.Price >= priceCutOff);


			//Pageable<StreamSnapshot> queryResultsLINQ = tableClient.Query<StreamSnapshot>(s => s.PartitionKey == partitionKey);


			StreamSnapshot results = tableClient.Query<StreamSnapshot>(s => s.PartitionKey == partitionKey).OrderBy(s => s.RowKey).LastOrDefault();

			//StreamSnapshot betterResults = tableClient.Query<StreamSnapshot>(s => s.PartitionKey == partitionKey).Max(x => x.RowKey);

			return results;


		}

	}
}
