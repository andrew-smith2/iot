using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;

namespace IOT
{
    
    public class MetricsPayload
    {

        public int counter { get; set; }
        public DateTime dateTime { get; set; }
        public string deviceName { get; set; }
        public MetricsPayload() { }
    }


    class Caller
    {

        static string connectionString = "{ Azure Hub connection string }";
        static ServiceClient serviceClient;

        public static MetricsPayload payload = new MetricsPayload()
        {
            counter = 0,
            deviceName = null
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Call Direct Method by pressing enter\n");
            //Connection to hub
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            Console.ReadLine();

            payload.deviceName = "{ Azure device ID }";
            
            //invokes direct method on device 10 times
            for (int i = 0; i < 10; i++)
            {
                InvokeMethod(payload).Wait();
            }
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

        }

        private static async Task InvokeMethod(MetricsPayload dataToSend)
        {
            string output = JsonConvert.SerializeObject(dataToSend);
            var methodInvocation = new CloudToDeviceMethod("calculate") { ResponseTimeout = TimeSpan.FromSeconds(30) };

            methodInvocation.SetPayloadJson(output);

            var response = await serviceClient.InvokeDeviceMethodAsync(dataToSend.deviceName, methodInvocation);
            payload = JsonConvert.DeserializeObject<MetricsPayload>(response.GetPayloadAsJson());
            Console.WriteLine("Response status: {0}, payload:", response.Status);
            Console.WriteLine(response.GetPayloadAsJson());
            
        }

    }
}
