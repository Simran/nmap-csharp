using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;


namespace nmapcsharp
{
	class MainClass
	{
		static CountdownEvent countdown;
        static int upCount = 0;
        static object lockObj = new object();
        const bool resolveNames = true;

        static void Main(string[] args)
        {
			Functions.InitColors();
			Functions.log ("Starting nmap-csharp ( github.com/Simran/nmap-csharp )", 4);
			
			checkArgs(args);
        }
		
		static void checkArgs(string[] args)
		{
			if (args[0] == "-local")
			{
				startScan(Regex.Match (defGateway(), "(\\d+.\\d+.\\d+.)").Groups[0].Value);
			}
			else
			{
				startScan(Regex.Match (args[0], "(\\d+.\\d+.\\d+.)").Groups[0].Value);
			}
		}
		
		static void startScan(string ipBase)
		{
				countdown = new CountdownEvent(1);
	            Stopwatch sw = new Stopwatch();
	            sw.Start();
	            for (int i = 1; i < 255; i++)
	            {
	                string ip = ipBase + i.ToString();
					new Thread(delegate()
					{       
					try
					{
						Ping p = new Ping();
		                p.PingCompleted += new PingCompletedEventHandler(pingDone);
		                countdown.AddCount();
		                p.SendAsync(ip, 100, ip);
					}
					catch (SocketException ex)
					{
						Functions.log (string.Format("Could not contact {0}", ip), 3);
					}
					}).Start();
	            }
	            countdown.Signal();
	            countdown.Wait();
	            sw.Stop();
	            //TimeSpan span = new TimeSpan(sw.ElapsedTicks);
	            Functions.log(string.Format("Took {0} milliseconds. {1} hosts active.", sw.ElapsedMilliseconds, upCount), 1);
			}
		
        static void pingDone(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                if (resolveNames)
                {
                    string name = null;
                    try
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                        name = hostEntry.HostName;
                    }
                    catch (SocketException ex)
                    {
                        name = "?";
                    }
                    Functions.log(string.Format("{0} ({1}) is up: ({2} ms)", ip, name, e.Reply.RoundtripTime), 2);
                }
                else
                { //but it's reachable doe.
                    Functions.log(string.Format("{0} is up: ({1} ms)", ip, e.Reply.RoundtripTime), 2);
                }
                lock(lockObj)
                {
                    upCount++;
                }
            }
            else if (e.Reply == null)
            {
                Functions.log(string.Format("Pinging {0} failed. (Null Reply object?)", ip), 3);
            }
            countdown.Signal();
        }
		
		static string defGateway()
		{
			string ip = null;
			foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
            if (f.OperationalStatus == OperationalStatus.Up)
			{
            foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
				{
					ip = d.Address.ToString();
				}
			}
			Functions.log (string.Format ("Network Gateway: {0}", ip), 5);
			return ip;
		}
    }
}
