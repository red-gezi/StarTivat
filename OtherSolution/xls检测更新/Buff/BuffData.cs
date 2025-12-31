using System.Collections.Generic;

public class BuffData
{
    public string Tag { get; set; }
    public string IconName { get; set; }
    public int Type { get; set; }
    public Dictionary<string, string> Name { get; set; } = new();
    public Dictionary<string, string> Text { get; set; } = new();
}