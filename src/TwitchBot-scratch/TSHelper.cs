using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot
{
	public static class TSHelper
	{

		public static void WriteData()
		{

			// Construct a new <see cref="TableClient" /> using a <see cref="TableSharedKeyCredential" />.
			var tableClient = new TableClient(
					new Uri(Settings.StorageUri),
					"StreamSnapshot",
					new TableSharedKeyCredential(Settings.StorageAccountName, Settings.StroageAccountKey));

			// Create the table in the service.
			tableClient.Create();

			// Make a dictionary entity by defining a <see cref="TableEntity">.

			//var entity = new TableEntity(partitionKey, rowKey)
			//{
			//	{ "Product", "Marker Set" },
			//	{ "Price", 5.00 },
			//	{ "Quantity", 21 }
			//};

			//Console.WriteLine($"{entity.RowKey}: {entity["Product"]} costs ${entity.GetDouble("Price")}.");
		}


	}
}