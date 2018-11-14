using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Q42.HueApi;
using Q42.HueApi.Interfaces;

namespace PhilipsHueScript
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            var assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            Config config;
            using (StreamReader r = new StreamReader(Path.Combine(assemblyPath,"config.json")))
            {
                string json = r.ReadToEnd();
                config = JsonConvert.DeserializeObject<Config>(json);
            }

            var configProperties = config.GetType().GetProperties();
            foreach (var property in configProperties)
            {
                var propertyName = property.Name;
                var propertyValue = config.GetType().GetProperty(propertyName).GetValue(config, null);

                if (propertyValue == null)
                {
                    var exceptionMessage = $"{propertyName} has not been defined in config.json.";
                    throw new Exception(exceptionMessage);
                }
            }

            var lightingService = new LightningService();
            var lightningData = lightingService.ReadCSVData(Path.Combine(assemblyPath,config.PathToLightData), config.Seconds);

            Console.WriteLine("Connecting to: " + config.IPAddress);
            ILocalHueClient client = new LocalHueClient(config.IPAddress);

            if (client != null)
            {
                client.Initialize(config.BridgeKey);
                Console.WriteLine("Connected to Bridge");
                var runIndefinitely = true;
                try
                {
                    while (runIndefinitely)
                    {
                        foreach (var color in lightningData)
                        {
                            double[] colorCoordinates = {color.XyColor.X, color.XyColor.Y};
                            var colorCommand = new LightCommand
                            {
                                ColorCoordinates = colorCoordinates,
                                Saturation = color.Saturation,
                                Brightness = Convert.ToByte(color.Brightness)
                            };

                            Console.WriteLine(color.XyColor.X+";"+color.XyColor.Y);

                            await client.SendCommandAsync(colorCommand);
                            Thread.Sleep(config.Seconds * 1000);
                        }

                        runIndefinitely = config.RunIndefinitely;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }
}