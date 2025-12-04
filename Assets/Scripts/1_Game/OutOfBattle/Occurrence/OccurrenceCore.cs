public class OccurrenceCore
{
    public static IBaseOccurrenceList CurrentOccurrenceList;
    //初始化,装载一个特定模式下事件的拷贝数据，可以在游戏过程中被修改
    public void Init(IBaseOccurrenceList targetOccurrenceList)
    {
        CurrentOccurrenceList= targetOccurrenceList.Clone();
    }
    //查询
    public OccurrenceData GetOccurrence()
    {
        CurrentOccurrenceList.GetOccurrence(this);
    }
}
