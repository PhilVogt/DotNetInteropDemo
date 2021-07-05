using Openfin.Desktop;
using Openfin.Desktop.Messaging;
using System;
using System.Diagnostics;

namespace ChannelApiDemo
{
    public class OpenFinProxy
	{
		public const string SymbolTopic = "SymbolTopic";
		public const string TradeIdTopic = "TradeIdTopic";
		public const string BroadcastTopic = "BroadcastTopic";
		public const string ViewSecInfoTopic = "ViewSecInfoTopic";

		public const string EzeOMSUuid = "E09233B3-B929-4B10-ADBD-EAC1CA5F6528";
		private const string NonEzeOMSUuid = "8651D4BB-5B58-4AE6-9984-3E6DB1641E7D";

		private readonly Runtime _runtime;
		//private ChannelProvider _channel;
		private ChannelClient _channelClient;
		public volatile bool IsConnected;
		private ChannelProvider _channelProvider;

		public static OpenFinProxy Instance { get; } = new OpenFinProxy();

		private OpenFinProxy()
		{
			string unique = Guid.NewGuid().ToString();

			_runtime = Runtime.GetRuntimeInstance(new RuntimeOptions
			{
				//Version = "19.89.60.5",
				Version = "18.87.55.19"
				//UUID = unique //ProcessHelper.IsOms ? EzeOMSUuid : NonEzeOMSUuid
			});

			// get command line args, check to see if we’re running as the “server”
			string[] commandLine = Environment.GetCommandLineArgs();
			
			bool isChannelProvider = bool.Parse(commandLine[1]);

			_runtime.Connect(async () =>
			{
				
				Console.WriteLine("----------------------- Application connected to OpenFin -----------------------");

				if (isChannelProvider)
				{
					_channelProvider = _runtime.InterApplicationBus.Channel.CreateProvider("1");
					_channelProvider.RegisterTopic<string>("SayHello", (payload) => { return $"Hello {payload}!"; });
					Console.WriteLine("----------------------- Channel 1 created  -----------------------");
				}
				else
				{
					// "may" be an issue
					//_channelClient = _runtime.InterApplicationBus.Channel.CreateClient(new ChannelConnectOptions("1")
					//{
					//	Wait = true
					//});
					_channelClient = _runtime.InterApplicationBus.Channel.CreateClient("1");

					await _channelClient.ConnectAsync(); // await here never finishes, and without connection, the sub/pub fails
					Debugger.Launch();
					Console.WriteLine("----------------------- ChannelClient created  -----------------------");
				}

				IsConnected = true;
			});
		}

		public void Broadcast(object data, string topic = BroadcastTopic)
		{
			if (_runtime.IsConnected)
			{
				_runtime.InterApplicationBus.Publish(topic, data);
			}
		}

		public void SubscribeToBroadcast(string topic, EventHandler<object> responseHandler)
		{
			if (_runtime.IsConnected)
			{
				InterApplicationBus.Subscription<object>(_runtime, topic).MessageReceived += (s, data) => responseHandler(null, data.Message);
			}
		}

		public async void PublishToChannel(object data, string topic = SymbolTopic)
		{
			if (_runtime.IsConnected)
			{
				await _channelClient.DispatchAsync<object>(topic, data);

				//_channel.Broadcast(topic, data);
			}
		}

		public void SubscribeToChannel<T>(string topic, Action<T> responseHandler)
		{
			if (_runtime.IsConnected)
			{
				_channelClient.RegisterTopic(topic, responseHandler);
			}
		}

		public bool IsFirstInstance()
		{
			Process[] appInstances = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

			return appInstances.Length == 1;
		}
	}
}
