using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[Header("Enemy Spawn Info")]
	public bool spawnEnemies = true; //生成敌人
	public bool useECS = false;
	public float enemySpawnRadius = 10f;
	public GameObject enemyPrefab;

	[Header("Enemy Spawn Timing")]
	[Range(1, 100)] public int spawnsPerInterval = 1; //控制出生数量
	[Range(.1f, 2f)] public float spawnInterval = 1f;  //波次频率
	
	EntityManager manager;
	Entity enemyEntityPrefab;

	float cooldown;


	void Start()
	{
		if (useECS)
		{
			manager = World.Active.EntityManager;
            //将Enemy实体转成Entity实体
			enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, World.Active);
		}
	}

	void Update()
    {
		if (!spawnEnemies || Settings.IsPlayerDead()) //如果不生成或者主角死亡则不生成Enemy
			return;

		cooldown -= Time.deltaTime;

        //冷却时间到生成一波怪物
		if (cooldown <= 0f)
		{
			cooldown += spawnInterval;
			Spawn();
		}
    }

	void Spawn()
	{
		for (int i = 0; i < spawnsPerInterval; i++)
		{
			Vector3 pos = Settings.GetPositionAroundPlayer(enemySpawnRadius); //获取生成的坐标位置

			if (!useECS) //不使用ECS
			{
				Instantiate(enemyPrefab, pos, Quaternion.identity); //根据坐标实例化怪物
			}
			else //使用ECS
			{
				Entity enemy = manager.Instantiate(enemyEntityPrefab);
				manager.SetComponentData(enemy, new Translation { Value = pos }); //给实体设置组件数据，设置怪物的出生坐标
			}
		}
	}
}
