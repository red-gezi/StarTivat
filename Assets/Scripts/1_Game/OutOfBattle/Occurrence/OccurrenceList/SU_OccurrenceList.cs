using System.Collections.Generic;
using System.Threading.Tasks;
public class SU_OccurrenceList : BaseOccurrenceList
{
    public new static List<Occurrence> Occurrences { get; set; } = new();
    public static void Init()
    {
        Occurrences  = new()
        {
            new Occurrence()
                .RegisterName(OccurrenceName.test1)
                .RegisterTag(OccurrenceTag.Occurrence, OccurrenceTag.Positive)
                .RegisterData("1_1")
                .RegisterAction("S1",  async ()=>
                {
                    //解锁某成就
                    await Task.Delay(1000);
                })
        };
    }
}
