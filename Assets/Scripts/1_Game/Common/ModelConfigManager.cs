using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class ModelConfigManager : InstanceBehaviour<ModelConfigManager>
{
    public bool ConfigMode;
    //调整人物的招式曲线
    public GameObject triggerModel;
    public GameObject targetModel;
    public List<CharaName> playerNames = new List<CharaName>();
    public List<EnemyName> enemyNames = new List<EnemyName>();
    private void Start()
    {
        if (ConfigMode)
        {
            GameManager.Instance.SwitchBattleMode(new OutBattleEnemy()
            {
                enemyDatas = enemyNames.Select(enemyName => new OutBattleEnemyData() { CurrentEnemyName = enemyName }).ToList()
            });
        }
    }

}