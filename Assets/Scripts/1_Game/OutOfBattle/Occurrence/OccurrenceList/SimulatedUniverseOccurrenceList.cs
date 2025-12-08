using System.Collections.Generic;
using System.Threading.Tasks;
public class SimulatedUniverseOccurrenceList : BaseOccurrenceList
{

    public new static List<Occurrence> OccurrenceList = new()
    {
        new Occurrence()
            .RegisterName(OccurrenceName.test1)
            .RegisterTag(OccurrenceTag.Occurrence, OccurrenceTag.Positive)
            .RegisterData("s-1")
            .RegisterAction("S1",  async ()=>
            {
                //解锁某成就
                await Task.Delay(1000);
            })
    };
}
