using System;

namespace TwitchMonitor
{
	class Program
	{
		static void Main(string[] args)
		{
			WelcomeUser();

			LiveMonitor liveMonitor = new LiveMonitor();
			Console.ReadLine();
			liveMonitor.StopMonitoring();

		}

		static void WelcomeUser()
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(@"___________       .__  __         .__         _____                .__  __                ");
			Console.WriteLine(@"\__    ___/_  _  _|__|/  |_  ____ |  |__     /     \   ____   ____ |__|/  |_  ___________ ");
			Console.WriteLine(@"  |    |  \ \/ \/ /  \   __\/ ___\|  |  \   /  \ /  \ /  _ \ /    \|  \   __\/  _ \_  __ \");
			Console.WriteLine(@"  |    |   \     /|  ||  | \  \___|   Y  \ /    Y    (  <_> )   |  \  ||  | (  <_> )  | \/");
			Console.WriteLine(@"  |____|    \/\_/ |__||__|  \___  >___|  / \____|__  /\____/|___|  /__||__|  \____/|__|   ");
			Console.WriteLine(@"                                \/     \/          \/            \/                       ");
			Console.ForegroundColor = ConsoleColor.White;
		}


	}
}