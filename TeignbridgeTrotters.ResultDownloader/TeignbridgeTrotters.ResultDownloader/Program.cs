using Microsoft;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using TeignbridgeTrotters.ResultDownloader;

var configuration = new ConfigurationBuilder()
     .AddJsonFile($"appsettings.json");

ProgramSettings programSettings = Requires.NotNull(configuration.Build().GetRequiredSection("ProgramSettings")!.Get<ProgramSettings>()!);
string message = string.Empty;
MD5 md5 = MD5.Create();
string previousMd5string = string.Empty;

while (1 == 1)
{
    using (var client = new HttpClient())
    {
        try
        {
            var responseStream = await client.GetStreamAsync(programSettings.SourceFileName);
            var memoryStream = new MemoryStream();
            responseStream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            var md5string = Convert.ToHexString(md5.ComputeHash(memoryStream));
            if (md5string != previousMd5string)
            {
                memoryStream.Position = 0;
                using (FileStream outputFileStream = new(programSettings.DestinationFileName!, FileMode.Create))
                {
                    await memoryStream.CopyToAsync(outputFileStream);
                }
                previousMd5string = md5string;
                message = $"Downloaded {programSettings.SourceFileName} to {programSettings.DestinationFileName}";
            }
            else
            {
                message = $"Not downloading {programSettings.SourceFileName} as md5 has not changed ({previousMd5string})";
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
