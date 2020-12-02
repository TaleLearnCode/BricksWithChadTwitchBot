using System;

namespace TwitchBot
{
	class Program
	{
		static void Main()
		{
			WelcomeUser();

			Bot bot = new Bot();
			bot.Connect(false);
			Console.ReadLine();
			bot.Disconnect();

		}

		static void WelcomeUser()
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine(@"__________        .__        __              __      __.__  __  .__      _________ .__                .___");
			Console.WriteLine(@"\______   \_______|__| ____ |  | __  ______ /  \    /  \__|/  |_|  |__   \_   ___ \|  |__ _____     __| _/");
			Console.WriteLine(@" |    |  _/\_  __ \  |/ ___\|  |/ / /  ___/ \   \/\/   /  \   __\  |  \  /    \  \/|  |  \\__  \   / __ | ");
			Console.WriteLine(@" |    |   \ |  | \/  \  \___|    <  \___ \   \        /|  ||  | |   Y  \ \     \___|   Y  \/ __ \_/ /_/ | ");
			Console.WriteLine(@" |______  / |__|  |__|\___  >__|_ \/____  >   \__/\  / |__||__| |___|  /  \______  /___|  (____  /\____ | ");
			Console.WriteLine(@"        \/                \/     \/     \/         \/                \/          \/     \/     \/      \/ ");
			Console.ForegroundColor = ConsoleColor.White;
		}

	}
}