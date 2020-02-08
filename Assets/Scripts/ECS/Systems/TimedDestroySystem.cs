using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// 时间到销毁物体
/// </summary>
[UpdateAfter(typeof(MoveForwardSystem))]
public class TimedDestroySystem : JobComponentSystem
{
	EndSimulationEntityCommandBufferSystem buffer;

	protected override void OnCreateManager()
	{
		buffer = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}

	struct CullingJob : IJobForEachWithEntity<TimeToLive>
	{
        [WriteOnly]
		public EntityCommandBuffer.Concurrent commands;
		public float dt;

        /// <summary>
        /// 每帧执行，如果寿命 < 0, 则摧毁实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="jobIndex">任务索引器</param>
        /// <param name="timeToLive">寿命</param>
		public void Execute(Entity entity, int jobIndex, ref TimeToLive timeToLive)
		{
			timeToLive.Value -= dt;
			if (timeToLive.Value <= 0f)
				commands.DestroyEntity(jobIndex, entity); //如果时间到则进入销毁队列
		}
    }

     // OnUpdate runs on the main thread.
    /// <summary>
    /// 在主线程上每帧运行OnUpdate
    /// </summary>
    /// <param name="inputDependencies">输入依赖</param>
    /// <returns>任务</returns>
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		var job = new CullingJob
		{
			commands = buffer.CreateCommandBuffer().ToConcurrent(),
			dt = Time.deltaTime
		};

		var handle = job.Schedule(this, inputDeps);
		buffer.AddJobHandleForProducer(handle); //将Job添加到队列中，等待主线程去执行

		return handle;
	}
}

