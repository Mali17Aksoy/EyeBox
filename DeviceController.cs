using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CognitiveServicesBot.Services
{
    public class DeviceController
    {
        const string deviceConnectionString = "HostName=malipi.azure-devices.net;DeviceId=malipi;SharedAccessKey=KwFpc4Xy7V2g=";
        static DeviceClient deviceClient = null;

        private static void CreateClient()
        {
            if (deviceClient == null)
            {
                // create Azure IoT Hub client from embedded connection string
                deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
            }
        }
        public static async Task<string> ReceiveCloudToDeviceMessageAsync()
        {
            CreateClient();

            while (true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    await deviceClient.CompleteAsync(receivedMessage);
                    return messageData;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public static async Task SendDeviceToCloudMessageAsync(string message)
        {
            CreateClient();
#if WINDOWS_UWP
        var str = "{\"deviceId\":\"malipi\",\"messageId\":1,\"text\":\"Hello, Cloud from a UWP C# app!\"}";
#else

#endif
            var eventmessage = new Message(Encoding.ASCII.GetBytes(message));

            await deviceClient.SendEventAsync(eventmessage);
        }


    }
}