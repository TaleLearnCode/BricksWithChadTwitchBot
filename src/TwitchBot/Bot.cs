using System;
using System.IO;
using System.Linq;
using System.Timers;
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

		private int _BricksDropped = 0;
		private int _Oofs = 0;
		private string _StreamKey;
		private ProjectTracking _ProjectTracking = new();

		private Timer _BotTimer = default;
		private int _BotTimerTotal;

		internal void Connect(bool logEvents)
		{

			twitchClient.Initialize(credentials, Settings.ChannelName);

			if (logEvents)
				twitchClient.OnLog += TwitchClient_OnLog;

			twitchClient.OnConnected += TwitchClient_OnConnected;
			twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
			twitchClient.OnDisconnected += TwitchClient_OnDisconnected;

			twitchClient.Connect();

			SetBotTimer();

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
			if (_BotTimer is not null)
				_BotTimer.Dispose();
		}

		private void TwitchClient_OnLog(object sender, OnLogArgs e)
		{
			Console.WriteLine(e.Data);
		}

		private void TwitchClient_OnConnected(object sender, OnConnectedArgs e)
		{
			PrintMessageToConsole("Bot Connected", ConsoleColor.Green, ConsoleColor.Black);
		}

		private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
		{

			try
			{
				var mostRecentStreamKey = StreamSnapshot.GetMostRecentStreamId("TaleLearnCode");
				if (mostRecentStreamKey != _StreamKey)
				{
					_StreamKey = mostRecentStreamKey;
					_BricksDropped = 0;
					_Oofs = 0;
				}
			}
			catch (Exception ex)
			{
				PrintException("GetMostRecentStreamId", ex);
			}


			PrintChatMessageToConsole(e.Command.ChatMessage);

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
				case "setproject":
					SetProject(e);
					break;
			}

			PrintStats();

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

		private void SetProject(OnChatCommandReceivedArgs commandArgs)
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
					twitchClient.SendMessage(Settings.ChannelName, $"Now working on the {commandArgs.Command.ArgumentsAsString} project");
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

		private void PrintChatMessageToConsole(ChatMessage chatMessage)
		{

			ConsoleColor currentBackground = Console.BackgroundColor;
			ConsoleColor currentForeground = Console.ForegroundColor;

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write(chatMessage.DisplayName);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write($": {chatMessage.Message}\n");

			Console.ForegroundColor = currentForeground;
			Console.BackgroundColor = currentBackground;

		}

		private void PrintMessageToConsole(string message, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
		{
			ConsoleColor currentBackground = Console.BackgroundColor;
			ConsoleColor currentForeground = Console.ForegroundColor;

			Console.BackgroundColor = backgroundColor;
			Console.ForegroundColor = foregroundColor;
			Console.WriteLine(message);

			Console.ForegroundColor = currentForeground;
			Console.BackgroundColor = currentBackground;

		}

		private void PrintStats()
		{
			Console.WriteLine($"\tBricks Dropped: {_BricksDropped}");
			Console.WriteLine($"\tOofs: {_Oofs}");
		}

		private void PrintException(string operation, Exception ex)
		{

			ConsoleColor currentBackground = Console.BackgroundColor;
			ConsoleColor currentForeground = Console.ForegroundColor;

			Console.BackgroundColor = ConsoleColor.Red;
			Console.ForegroundColor = ConsoleColor.White;

			Console.WriteLine($"{operation}: {ex.Message}");

			Console.ForegroundColor = currentForeground;
			Console.BackgroundColor = currentBackground;

		}

		private void SetBotTimer()
		{
			_BotTimer = new Timer(Settings.TimerInternval);
			_BotTimer.Elapsed += OnBotTimerElapsed;
			_BotTimer.AutoReset = true;
			_BotTimer.Enabled = true;
		}

		private void OnBotTimerElapsed(object sender, ElapsedEventArgs e)
		{

			ConsoleColor foregroundColor = ConsoleColor.Blue;
			ConsoleColor backgroundColor = ConsoleColor.Black;

			_BotTimerTotal++;

			if (_BotTimerTotal % Settings.ProjectReminderInterval == 0 && string.IsNullOrWhiteSpace(_ProjectTracking.RowKey))
			{
				twitchClient.SendMessage(Settings.ChannelName, $"Hey {Settings.ChannelName} don't forget to set the project that you are working on.");
				PrintMessageToConsole($"Set Project Reminder {DateTime.Now.ToLongTimeString()}", foregroundColor, backgroundColor);
			}

			if (_BotTimerTotal % Settings.WaterReminderInterval == 0)
			{
				PrintMessageToConsole($"Water Reminder {DateTime.Now.ToLongTimeString()}", foregroundColor, backgroundColor);
				twitchClient.SendMessage(Settings.ChannelName, $"Hey {Settings.ChannelName} don't forgot to drink some water!");
			}

		}
	}

}