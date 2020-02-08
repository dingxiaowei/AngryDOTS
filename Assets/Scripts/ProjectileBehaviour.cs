using Unity.Entities;
using UnityEngine;
 
/// <summary>
/// 子弹组件
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ProjectileBehaviour : MonoBehaviour, IConvertGameObjectToEntity
{
	[Header("Movement")]
	public float speed = 50f;

	[Header("Life Settings")]
	public float lifeTime = 2f;

	Rigidbody projectileRigidbody;


	void Start()
	{
		projectileRigidbody = GetComponent<Rigidbody>();
		Invoke("RemoveProjectile", lifeTime);
	}

	void Update()
	{
		Vector3 movement = transform.forward * speed * Time.deltaTime;
		projectileRigidbody.MovePosition(transform.position + movement); //刚体移动
	}

	void OnTriggerEnter(Collider theCollider)
	{

		if (theCollider.CompareTag("Enemy") || theCollider.CompareTag("Environment"))  //碰到敌人就销毁
			RemoveProjectile();
	}

	void RemoveProjectile()
	{
		Destroy(gameObject);
	}

    //将GameObject转换为实体
	public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem) 
	{
		manager.AddComponent(entity, typeof(MoveForward)); //实体添加MoveForeward组件

		MoveSpeed moveSpeed = new MoveSpeed { Value = speed };	
		manager.AddComponentData(entity, moveSpeed);  //给实体Entity添加MoveSpeed组件数据

		TimeToLive timeToLive = new TimeToLive { Value = lifeTime };
		manager.AddComponentData(entity, timeToLive);  //给实体Entity添加TimeToLive组件
	}
}
