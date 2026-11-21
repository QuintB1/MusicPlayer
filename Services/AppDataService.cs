using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using MusicPlayer.Models;

namespace MusicPlayer.Services;

public class AppDataService
{
    private readonly string _projectDirectory =
    Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "../../../")
    );
    private readonly string _jsonPath;
    public AppDataService()
    {
        _jsonPath =
        Path.Combine(
            _projectDirectory,
            "Resources",
            "appData.json"
        );
    }

    public async Task<AppData> LoadAppDataAsync()
    {
        if (!File.Exists(_jsonPath))
        {
            throw new FileNotFoundException(
                "AppData.json could not be found.",
                _jsonPath
            );
        }

        string json = await File.ReadAllTextAsync(_jsonPath);
        Console.WriteLine("loaded JSON AppData");
        return JsonSerializer.Deserialize<AppData>(json)
            ?? throw new InvalidDataException(
                "AppData.json could not be deserialized."
            );
    }

    public async Task SaveAppDataAsync(AppData appData)
    {
        string json = JsonSerializer.Serialize(
            appData,
            new JsonSerializerOptions { WriteIndented = true }
        );

        await File.WriteAllTextAsync(_jsonPath, json);
    }
}