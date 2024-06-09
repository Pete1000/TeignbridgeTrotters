using Microsoft;
using Microsoft.Extensions.Configuration;
using TeignbridgeTrotters.ResultDownloader;

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json");

ProgramSettings programSettings = Requires.NotNull(configuration.Build().GetRequiredSection("ProgramSettings")!.Get<ProgramSettings>()!);
string message = string.Empty;

while (1 == 1)
{
    using (var client = new HttpClient())
    {
        try
        {
            var response = await client.GetStreamAsync(programSettings.SourceFileName);
            using FileStream outputFileStream = new(programSettings.DestinationFileName!, FileMode.Create);
            await response.CopyToAsync(outputFileStream);
            message = $"Downloaded {programSettings.SourceFileName} to {programSettings.DestinationFileName}";
        } 
        catch (Exception ex)
        {
            message = $"Error: {ex.Message}";
        }
    }
    
    Console.WriteLine($"{DateTime.Now:T} - {message} - Waiting for {programSettings.WaitTimeSeconds} seconds");
    Thread.Sleep(programSettings.WaitTimeSeconds * 1000);
};
