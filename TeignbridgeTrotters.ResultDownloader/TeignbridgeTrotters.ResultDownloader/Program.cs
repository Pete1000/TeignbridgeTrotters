using Microsoft;
using Microsoft.Extensions.Configuration;
using System.Text;
using TeignbridgeTrotters.ResultDownloader;

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json");

ProgramSettings programSettings = Requires.NotNull(configuration.Build().GetRequiredSection("ProgramSettings")!.Get<ProgramSettings>()!);
string message = string.Empty;
long previousStreamLength = 0;

while (1 == 1)
{
    using (var client = new HttpClient())
    {
        try
        {
            var responseStream = await client.GetStreamAsync(programSettings.SourceFileName);
            var memoryStream = new MemoryStream();
            responseStream.CopyTo(memoryStream);
            if (memoryStream.Length != previousStreamLength)
            {
                memoryStream.Position = 0;
                using (FileStream outputFileStream = new(programSettings.DestinationFileName!, FileMode.Create))
                {
                    await memoryStream.CopyToAsync(outputFileStream);
                }
                previousStreamLength = memoryStream.Length;
                message = $"Downloaded {programSettings.SourceFileName} to {programSettings.DestinationFileName}";
            }
            else
            {
                message = $"Not downloading {programSettings.SourceFileName} as length has not changed ({previousStreamLength})";
            }
        }
        catch (Exception ex)
        {
            message = $"Error: {ex.Message}";
        }
    }

    Console.WriteLine($"{DateTime.Now:T} - {message} - Waiting for {programSettings.WaitTimeSeconds} seconds");
    Thread.Sleep(programSettings.WaitTimeSeconds * 1000);
};
