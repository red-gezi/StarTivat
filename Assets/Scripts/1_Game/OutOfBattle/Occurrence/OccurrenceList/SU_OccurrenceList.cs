using System.Collections.Generic;
using System.Threading.Tasks;
public class SU_OccurrenceList : BaseOccurrenceList
{
    public new static List<Occurrence> Occurrences { get; set; } = new();
    public static void Init()
    {
        Occurrences = new()
        {
            new Occurrence()
                .RegisterData("1_1")
                .RegisterName(OccurrenceName.test1)
                .RegisterTag(OccurrenceTag.Occurrence, OccurrenceTag.Positive)
                .RegisterAction("S1",  async ()=>
                {
                    //解锁某成就
                    await Task.Delay(1000);
                    Log.Show("触发了S1效果");
                }),
            new Occurrence()
                .RegisterData("1_2")
                .RegisterName(OccurrenceName.test2)
                .RegisterTag(OccurrenceTag.Occurrence, OccurrenceTag.Positive)
                .RegisterAction("S1",  async ()=>
                {
                    //解锁某成就
                    await Task.Delay(1000);
                    Log.Show("触发了S1效果");
                })
        };
    }
}
