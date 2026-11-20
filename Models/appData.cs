using System;
using System.Collections.Generic;

namespace MusicPlayer.Models;
public class AppData
{
    public String RelativeArchiveURL { get; set; } = "";

    public String RelativePlayListsURL { get; set; } = "";

    public HashSet<String> StoredPlayLists { get; set; } = new();
}