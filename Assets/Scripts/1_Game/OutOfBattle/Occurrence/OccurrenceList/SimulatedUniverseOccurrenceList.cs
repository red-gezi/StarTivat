using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Threading.Tasks;
public class SimulatedUniverseOccurrenceList : IBaseOccurrenceList
{
    public static List<OccurrenceData> occurrenceList = new()
    {
        new OccurrenceData()
            .RegisterName(OccurrenceName.test1)
            .RegisterTag(OccurrenceTag.Occurrence, OccurrenceTag.Positive)
            .RegisterStory("s-1")
            .RegisterAction("S1",  async ()=>
            {
                //解锁某成就
                await Task.Delay(1000);
            })
    };
}
