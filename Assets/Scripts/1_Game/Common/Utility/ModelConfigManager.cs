using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class ModelConfigManager : InstanceBehaviour<ModelConfigManager>
{
    public bool ConfigMode;
    //当前配置的是否为怪物
    public bool isConfigEnemy;
    //调整人物的招式曲线
    public GameObject triggerModel;
    public GameObject targetModel;
    [Header("当前配置的玩家角色编号，范围1-4")]
    public int defaultPlayerIndex;
    public List<PlayerName> playerNames = new List<PlayerName>();
    [Header("当前配置的敌方角色编号，范围1-5")]
    public int defaultEnemyIndex;
    public List<EnemyName> enemyNames = new List<EnemyName>();
    private void Start()
    {
        if (ConfigMode)
        {
            GameFlowSystem.Instance.SwitchInBattleMode(
                playerNames,
                new OutOfBattleEnemyDatas()
                {
                    enemyDatas = enemyNames.Select(enemyName => new EnemyData() { CurrentEnemyName = enemyName }).ToList()
                });
            if (!isConfigEnemy)
            {
                triggerModel = InBattleSystem.Instance.PlayerList[defaultPlayerIndex].gameObject;
                targetModel = InBattleSystem.Instance.EnemyList[defaultEnemyIndex].gameObject;
            }
            else
            {
                triggerModel = InBattleSystem.Instance.EnemyList[defaultEnemyIndex].gameObject;
                targetModel = InBattleSystem.Instance.PlayerList[defaultPlayerIndex].gameObject;
            }

        }
    }

}