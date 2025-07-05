using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;

public class AnimatorManager : IDisposable {
	private readonly ObjectPool<GameObject> pool;	
	private CancellationTokenSource lifetimeCts;

	public AnimatorManager(GameObject prefab, Transform parent = null, int initialCapacity = 10, int maxSize = 100) {
		pool = new ObjectPool<GameObject>(
			createFunc: () => UnityEngine.Object.Instantiate(prefab, parent),
			actionOnGet: go => go.SetActive(true),
			actionOnRelease: go => {
				if (go != null) go.SetActive(false);
			},
			actionOnDestroy: UnityEngine.Object.Destroy,			
			collectionCheck: true,
			defaultCapacity: initialCapacity,
			maxSize: maxSize
		);		
		lifetimeCts = new CancellationTokenSource();
	}

	
	public async UniTask PlayAsync(Vector3 position, float durationSeconds = 1f) {
	
		if (lifetimeCts.IsCancellationRequested) return;
		var obj = pool.Get();
		try {
			obj.transform.position = position;			
			await UniTask.Delay(TimeSpan.FromSeconds(durationSeconds), cancellationToken: lifetimeCts.Token);
		}
		finally {			
			if (obj != null) {
				pool.Release(obj);
			}
		}
	}

	public void StopAll() {
		if (!lifetimeCts.IsCancellationRequested) {
			lifetimeCts.Cancel();
			lifetimeCts.Dispose();
		}
		lifetimeCts = new CancellationTokenSource();
		pool.Clear();
	}

	public void Dispose() {
		if (!lifetimeCts.IsCancellationRequested) {
			lifetimeCts.Cancel();
		}
		lifetimeCts.Dispose();
		pool.Dispose();
	}
}