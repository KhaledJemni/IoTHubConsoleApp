using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleappIoT
{
    class Program
    {
        //You can get the DeviceConnectionString from your IoT Hub on the Azure Portal
        //Click on your IoTHub, then IoTHub Owner
        private const string DeviceConnectionString = "HostName=TutorialHub.azure-devices.net;SharedAccessKeyName=iothubowner;DeviceId=MyDevice;SharedAccessKey=1tnE4rnywvZO5updbmN7J8mNcz65H7yOLxlcUYQqD2o=";
        static ServiceClient serviceClient;

        static void Main(string[] args)
        {
            serviceClient = ServiceClient.CreateFromConnectionString(DeviceConnectionString);
            Program program = new Program();
        }

        public Program()
        {
            DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
            SendEvent().Wait();
            ReceiveCommands(deviceClient).Wait();
        }

        //This method is responsible for sending the Event to the IoT Hub
        static async Task SendEvent()
        {
            //This is a static message the we send to the IoT Hub once the application is launched
            string dataBuffer = "IoT in 90 Seconds";
            Microsoft.Azure.Devices.Message eventMessage = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(dataBuffer));
            await serviceClient.SendAsync("MyDevice", eventMessage);
        }

        //This method is responsible to receive the message on the IoT Hub
        async Task ReceiveCommands(DeviceClient deviceClient)
        {
            Console.WriteLine("\n Device waiting for IoT Hub command....\n");
            Microsoft.Azure.Devices.Client.Message receivedMessage;
            string messageData;
            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(1));
                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    Console.WriteLine("\t{0}> Message received: {1}", DateTime.Now.ToLocalTime(), messageData);
                    await deviceClient.CompleteAsync(receivedMessage);
                }
            }
        }
    }
}
