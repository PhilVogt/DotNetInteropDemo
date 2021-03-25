using Openfin.Desktop;
using Openfin.Desktop.Messaging;
using System;
using System.Windows;

namespace ChannelApiDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private RuntimeOptions runtimeOptions;
        private Runtime runtime;
        private ChannelClient client;

        public MainWindow()
        {
            InitializeComponent();
            InitialiseOpenFin();
            InitialiseEmbeddedView();
        }

        private void InitialiseOpenFin()
        {
            this.runtimeOptions = new RuntimeOptions
            {
                Version = "18.87.55.19"
            };

            this.runtime = Runtime.GetRuntimeInstance(runtimeOptions);
            runtime.Connect(async () =>
            {
                // Enable our buttons now that we're connected to the runtime
                this.Dispatcher.Invoke(() =>
                {
                    this.submitButton.IsEnabled = true;
                    this.gridButton.IsEnabled = true;
                    this.tabbedButton.IsEnabled = true;
                    this.columnsButton.IsEnabled = true;
                    this.rowsButton.IsEnabled = true;                    
                });

                runtime.System.getAllWindows((result) => {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (!this.windowsSelector.IsEnabled && result.isSuccessful())
                        {
                            var windows = result.getData();
                            foreach (var window in windows.Children())
                            {
                                if (window["uuid"].ToString() == "openfin-local-demo-platform")
                                {
                                    foreach (var odpWindow in window["childWindows"])
                                    {
                                        if (odpWindow.Value<bool>("isShowing"))
                                        {
                                            var windowName = odpWindow.Value<string>("name");
                                            this.windowsSelector.Items.Add(windowName);
                                            this.windowsSelector.IsEnabled = true;
                                        }
                                    }
                                }

                            }
                        }
                    });
                });

                try
                {
                    this.client = this.runtime.InterApplicationBus.Channel.CreateClient("pack-actions");
                    await client.ConnectAsync();
                    Console.WriteLine("Client channel connected");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });


        }

        private void InitialiseEmbeddedView()
        {
            var appOptions = new ApplicationOptions("channel-api-wpf-demo",
                "embedded-react-app",
                "http://localhost:3000");

            this.OpenFinEmbeddedView.Initialize(this.runtimeOptions, appOptions);
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            InterApplicationBus.Publish(this.runtime, "messages", new { name = "Eze WPF Demo", text = this.messageInput.Text });

        }

        private async void SendLayoutCommand(string command)
        {
            await this.client.DispatchAsync<object>("executeAction", new { id = command, target = "internal", targetIdentity = new { uuid = "openfin-local-demo-platform", name = this.windowsSelector.SelectedItem } });
        }

        private void gridButton_Click(object sender, RoutedEventArgs e)
        {
            SendLayoutCommand("grid");
        }

        private void columnsButton_Click(object sender, RoutedEventArgs e)
        {
            SendLayoutCommand("columns");
        }

        private void rowsButton_Click(object sender, RoutedEventArgs e)
        {
            SendLayoutCommand("rows");
        }

        private void tabbedButton_Click(object sender, RoutedEventArgs e)
        {
            SendLayoutCommand("tabbed");
        }
    }
}
