using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TwitchBot
{

	public class Bot
	{

		// TwitchTokenGenerator.com
		ConnectionCredentials credentials = new ConnectionCredentials(Settings.ChannelName, Settings.AccessToken);
		TwitchClient twitchClient = new TwitchClient();

		int _BricksDroped = 0;


		internal void Connect(bool logEvents)
		{

			twitchClient.Initialize(credentials, Settings.ChannelName);

			if (logEvents)
				twitchClient.OnLog += TwitchClient_OnLog;

			twitchClient.OnConnected += TwitchClient_OnConnected;
			twitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;
			twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;

			twitchClient.Connect();

		}

		internal void Discconect()
		{
			twitchClient.Disconnect();
		}

		private void TwitchClient_OnLog(object sender, OnLogArgs e)
		{
			Console.WriteLine(e.Data);
		}

		private void TwitchClient_OnConnected(object sender, OnConnectedArgs e)
		{
			Console.WriteLine("[Bot]: Connected");
		}

		private void TwitchClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{
			Console.WriteLine($"[{e.ChatMessage.DisplayName}]: {e.ChatMessage.Message}");
		}

		private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{

			switch (e.Command.CommandText.ToLower())
			{
				case "dropbrick":
					if (e.Command.ChatMessage.IsBroadcaster)
					{
						if (e.Command.ArgumentsAsList.Count > 0)
						{
							if (int.TryParse(e.Command.ArgumentsAsList[0], out var numberOfBricksDropped))
								_BricksDroped += numberOfBricksDropped;
							else if (e.Command.ArgumentsAsList[0].ToLower() == "init")
								_BricksDroped = 0;
						}
						else
							_BricksDroped++;
					}

					twitchClient.SendMessage(Settings.ChannelName, $"Chad has dropped {_BricksDroped} bricks so far this stream.");
					break;
			}

			//Console.WriteLine($"[CommandText]: {e.Command.CommandText}");
			//Console.WriteLine($"[CommandIdentifier]: {e.Command.CommandIdentifier}");
			//Console.WriteLine($"[ArgumentsAsString]: {e.Command.ArgumentsAsString}");
			//Console.WriteLine($"[ArgumentsAsList.Count]: {e.Command.ArgumentsAsList.Count}");
		}

	}
}