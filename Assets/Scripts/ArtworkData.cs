using System;
using System.Collections.Generic;

[Serializable]
public class ArtworkData
{
    public string id;
    public string title;
    public string subtitle;
    public string year;
    public string medium;
    public string description;
    public string imagePath;
    public string audioPath;
    public int width;
    public int height;
    public string orientation;
}

[Serializable]
public class ArtworkDataList
{
    public List<ArtworkData> artworks;
}