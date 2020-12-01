using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace TwitchBot
{

	public class Bot
	{

		// TwitchTokenGenerator.com
		ConnectionCredentials credentials = new ConnectionCredentials(Settings.ChannelName, Settings.AccessToken);
		TwitchClient twitchClient = new TwitchClient();

		int _BricksDroped = 0;
		int _Oofs = 0;
		private string _StreamKey;

		internal void Connect(bool logEvents)
		{

			twitchClient.Initialize(credentials, Settings.ChannelName);

			if (logEvents)
				twitchClient.OnLog += TwitchClient_OnLog;

			twitchClient.OnConnected += TwitchClient_OnConnected;
			twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
			twitchClient.OnDisconnected += TwitchClient_OnDisconnected;

			twitchClient.Connect();

		}

		private void TwitchClient_OnDisconnected(object sender, OnDisconnectedEventArgs e)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Bot Disconnected");
			Console.ForegroundColor = ConsoleColor.White;
		}

		internal void Disconnect()
		{
			twitchClient.Disconnect();
		}

		private void TwitchClient_OnLog(object sender, OnLogArgs e)
		{
			Console.WriteLine(e.Data);
		}

		private void TwitchClient_OnConnected(object sender, OnConnectedArgs e)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Bot Connected");
			Console.ForegroundColor = ConsoleColor.White;
		}

		private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{

			var mostRecentStreamKey = StreamSnapshot.GetMostRecentStreamId("TaleLearnCode");
			if (mostRecentStreamKey != _StreamKey)
			{
				_StreamKey = mostRecentStreamKey;
				_BricksDroped = 0;
				_Oofs = 0;
			}

			switch (e.Command.CommandText.ToLower())
			{
				case "dropbrick":
					DropBrick(e);
					break;
				case "oof":
					Oof(e);
					break;
				case "stats":
					Stats();
					break;
			}

		}

		private void DropBrick(OnChatCommandReceivedArgs commandArgs)
		{

			if (commandArgs.Command.ChatMessage.IsBroadcaster || commandArgs.Command.ChatMessage.IsModerator)
			{
				if (commandArgs.Command.ArgumentsAsList.Count > 0)
				{
					if (int.TryParse(commandArgs.Command.ArgumentsAsList[0], out var numberOfBricksDropped))
						_BricksDroped += numberOfBricksDropped;
				}
				else
					_BricksDroped++;
			}

			twitchClient.SendMessage(Settings.ChannelName, $"Chad has dropped {_BricksDroped} {(_BricksDroped == 1 ? "brick" : "bricks")} so far this stream.");

		}

		private void Oof(OnChatCommandReceivedArgs commandArgs)
		{

			if (commandArgs.Command.ChatMessage.IsBroadcaster || commandArgs.Command.ChatMessage.IsModerator)
				_Oofs++;

			twitchClient.SendMessage(Settings.ChannelName, $"Chad has had {_Oofs} {(_Oofs == 1 ? "oof" : "oofs")} so far this stream.");

		}

		private void Stats()
		{
			twitchClient.SendMessage(Settings.ChannelName, $"Chad has dropped {_BricksDroped} {(_BricksDroped == 1 ? "brick" : "bricks")} and had {_Oofs} {(_Oofs == 1 ? "oof" : "oofs")} so far this stream.");
		}


	}

}
