using System;
using System.IO;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace TwitchBot
{

	public class Bot
	{

		// TODO: Adding console logging
		// TODO: Add a RestProject stream message

		// TwitchTokenGenerator.com
		ConnectionCredentials credentials = new ConnectionCredentials(Settings.ChannelName, Settings.AccessToken);
		TwitchClient twitchClient = new TwitchClient();

		private int _BricksDropped = 0;
		private int _Oofs = 0;
		private string _StreamKey;
		private ProjectTracking _ProjectTracking = new();

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
				_BricksDropped = 0;
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
				case "resetproject":
					ResetProject(e);
					break;
			}

			_ProjectTracking.Save();

		}

		private void DropBrick(OnChatCommandReceivedArgs commandArgs)
		{

			if (commandArgs.Command.ChatMessage.IsBroadcaster || commandArgs.Command.ChatMessage.IsModerator)
			{
				int numberOfBricksDropped = 1;
				if (commandArgs.Command.ArgumentsAsList.Any())
					int.TryParse(commandArgs.Command.ArgumentsAsList[0], out numberOfBricksDropped);

				_BricksDropped += numberOfBricksDropped;
				UpdateProjectStats(bricksDropped: numberOfBricksDropped);
			}

			twitchClient.SendMessage(Settings.ChannelName, $"Chad has dropped {_BricksDropped} {(_BricksDropped == 1 ? "brick" : "bricks")} so far this stream.{(!string.IsNullOrWhiteSpace(_ProjectTracking.RowKey) ? $" During the {_ProjectTracking.RowKey} build, Chad has dropped {_ProjectTracking.BricksDropped} {(_ProjectTracking.BricksDropped == 1 ? "brick" : "bricks")}." : string.Empty)}");

			UpdateLocalStats(Settings.BrickDropOutputPath, _BricksDropped);

		}

		private void Oof(OnChatCommandReceivedArgs commandArgs)
		{

			if (commandArgs.Command.ChatMessage.IsBroadcaster || commandArgs.Command.ChatMessage.IsModerator)
			{
				_Oofs++;
				UpdateProjectStats(oofs: 1);
			}

			twitchClient.SendMessage(Settings.ChannelName, $"Chad has dropped {_Oofs} {(_Oofs == 1 ? "oof" : "oofs")} so far this stream.{(!string.IsNullOrWhiteSpace(_ProjectTracking.RowKey) ? $" During the {_ProjectTracking.RowKey} build, Chad has had {_ProjectTracking.Oofs} {(_ProjectTracking.Oofs == 1 ? "oof" : "oofs")}." : string.Empty)}");

			UpdateLocalStats(Settings.OofOutputPath, _Oofs);

		}

		private void Stats()
		{
			twitchClient.SendMessage(Settings.ChannelName, $"Chad has dropped {_BricksDropped} {(_BricksDropped == 1 ? "brick" : "bricks")} and had {_Oofs} {(_Oofs == 1 ? "oof" : "oofs")} so far this stream.");
		}

		private void ResetProject(OnChatCommandReceivedArgs commandArgs)
		{
			if (commandArgs.Command.ChatMessage.IsBroadcaster || commandArgs.Command.ChatMessage.IsModerator)
			{
				if (commandArgs.Command.ArgumentsAsList.Any())
				{
					_ProjectTracking = ProjectTracking.Retrieve(Settings.ChannelName, commandArgs.Command.ArgumentsAsString);
					if (_ProjectTracking is null)
					{
						_ProjectTracking = new ProjectTracking(Settings.ChannelName, commandArgs.Command.ArgumentsAsString);
						_ProjectTracking.Save();
					}
				}
			}
		}

		private void UpdateProjectStats(int bricksDropped = 0, int oofs = 0)
		{
			_ProjectTracking.BricksDropped += bricksDropped;
			_ProjectTracking.Oofs += oofs;
		}

		private void UpdateLocalStats(string path, int count)
		{
			using StreamWriter sw = File.CreateText(path);
			sw.Write(count);
		}

	}

}