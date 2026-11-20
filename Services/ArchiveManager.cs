
using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using MusicPlayer.Models;

namespace MusicPlayer.Services;

public class ArchiveManager
{
    private readonly AppData _appData;

        private readonly string _projectDirectory =
    Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "../../../../")
    );
    private String _archivePath;
    private String _playListPath;

    private readonly HashSet<String> _newPlaylists =
        new(StringComparer.OrdinalIgnoreCase);

    public ArchiveManager(AppData appData)
    {
        _appData = appData;
        _archivePath = Path.Combine(_projectDirectory, _appData.RelativeArchiveURL);
        _playListPath = Path.Combine(_projectDirectory, _appData.RelativePlayListsURL);
    }

    public void GetNewPlaylists()
    {
        Console.WriteLine("=== GetNewPlaylists started ===");
        Console.WriteLine($"Archive directory: {_archivePath}");

        if (!Directory.Exists(_archivePath))
        {
            throw new DirectoryNotFoundException(
                $"Archive directory not found: {_archivePath}"
            );
        }

        _newPlaylists.Clear();

        // Find ZIP files, including extensions with uppercase letters.
        var zipFiles = Directory
            .EnumerateFiles(_archivePath)
            .Where(file =>
                string.Equals(
                    Path.GetExtension(file),
                    ".zip",
                    StringComparison.OrdinalIgnoreCase
                )
            );

        foreach (string zipFile in zipFiles)
        {
            string name = Path.GetFileNameWithoutExtension(zipFile);

            Console.WriteLine($"Found ZIP: {name}");

            if (!_appData.StoredPlayLists.Contains(
                    name,
                    StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine($"New playlist found: {name}");
                _newPlaylists.Add(name);
            }
        }

        Console.WriteLine(
            $"New playlists found: {_newPlaylists.Count}"
        );

        Console.WriteLine("=== GetNewPlaylists finished ===");
    }

    public void AddNewPlaylists()
    {
        Console.WriteLine("=== AddNewPlaylists started ===");
        Console.WriteLine($"Playlist directory: {_archivePath}");

        if (!Directory.Exists(_archivePath))
        {
            throw new DirectoryNotFoundException(
                $"Archive directory not found: {_archivePath}"
            );
        }

        Directory.CreateDirectory(_archivePath);

        foreach (string name in _newPlaylists)
        {
            string zipFile = Path.Combine(
                _archivePath,
                name + ".zip"
            );

            string destination = Path.Combine(
                _playListPath,
                name
            );

            try
            {
                Console.WriteLine($"Extracting playlist: {name}");

                Directory.CreateDirectory(destination);

                ZipFile.ExtractToDirectory(
                    zipFile,
                    destination,
                    overwriteFiles: true
                );

                // Only register the playlist after successful extraction.
                _appData.StoredPlayLists.Add(name);

                Console.WriteLine(
                    $"Successfully added playlist: {name}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Failed to extract playlist {name}: {ex}"
                );
            }
        }

        _newPlaylists.Clear();

        Console.WriteLine("=== AddNewPlaylists finished ===");
    }
}
