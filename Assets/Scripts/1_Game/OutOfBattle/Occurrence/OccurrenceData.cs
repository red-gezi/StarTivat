using System.Collections.Generic;

public class OccurrenceData
{
    public string Tag { get; set; }
    public string ImageName { get; set; }
    public string SideColor { get; set; }
    public Dictionary<string, string> Name { get; set; } = new();
    public Dictionary<string, string> Dialogue { get; set; } = new();
    public string ShowName => Name[TranslateSystem.CurrentLanguage];
    public string ShowDialogue => Dialogue[TranslateSystem.CurrentLanguage];
    public OccurrenceData()
    {
    }
}