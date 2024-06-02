using Microsoft;
using Microsoft.Extensions.Configuration;
using TeignbridgeTrotters.ResultDownloader;

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json");

ProgramSettings programSettings = Requires.NotNull(configuration.Build().GetRequiredSection("ProgramSettings")!.Get<ProgramSettings>()!);

while (1 == 1)
{
    using (var client = new HttpClient())
    {
        var response = await client.GetStreamAsync(programSettings.SourceFileName);
        using FileStream outputFileStream = new(programSettings.DestinationFileName!, FileMode.Create);
        await response.CopyToAsync(outputFileStream);
    }
    
    Console.WriteLine($"{DateTime.Now:T} - Downloaded file. Waiting for {programSettings.WaitTimeSeconds} seconds");
    Thread.Sleep(programSettings.WaitTimeSeconds * 1000);
};
