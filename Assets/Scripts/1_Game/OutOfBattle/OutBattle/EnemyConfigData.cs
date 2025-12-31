using System.Collections.Generic;
using System.Linq;
public class EnemyConfigData
{
    public Dictionary<EnemyConfigDataType, float> EnemyTypeWeight { get; set; } = new()
    {
        { EnemyConfigDataType.Enemy1Wave, 1f },
        { EnemyConfigDataType.Enemy1Wave, 1f },

    };
    static List<(EnemyConfigDataType, OutOfBattleEnemyDatas)> outOfBattleEnemyDatas = new()
    {
        (EnemyConfigDataType.Enemy1Wave,new ()
        {
            enemyDatas= new()
            {
                 new EnemyData(){  CurrentEnemyName= EnemyName.Qiuqiu  },
                 new EnemyData(){  CurrentEnemyName= EnemyName.Qiuqiu  },
                 new EnemyData(){  CurrentEnemyName= EnemyName.Qiuqiu  },
            }
        }),
        (EnemyConfigDataType.Enemy1Wave,new ()
        {
            enemyDatas= new()
            {
                 new EnemyData(){  CurrentEnemyName= EnemyName.Qiuqiu  },
                 new EnemyData(){  CurrentEnemyName= EnemyName.Qiuqiu  },
                 new EnemyData(){  CurrentEnemyName= EnemyName.Qiuqiu  },
                 new EnemyData(){  CurrentEnemyName= EnemyName.Qiuqiu  },
                 new EnemyData(){  CurrentEnemyName= EnemyName.Qiuqiu  },
            }
        }),
        (EnemyConfigDataType.Enemy1Wave,new ()
        {
            enemyDatas= new()
            {
                 new EnemyData(){  CurrentEnemyName= EnemyName.Qiuqiu  },
                 new EnemyData(){  CurrentEnemyName= EnemyName.Qiuqiu  },
            }
        }),
    };
    //从指定敌人类型随机一个配置数据
    public static OutOfBattleEnemyDatas GetRandomEnemyConfigData(EnemyConfigDataType enemyType)
    {
        // 获取所有指定类型的配置数据
        var enemyConfigs = outOfBattleEnemyDatas
            .Where(pair => pair.Item1 == enemyType)
            .Select(pair => pair.Item2)
            .ToList();
        return RandSystem.GetRandomValue(enemyConfigs);
    }
}
