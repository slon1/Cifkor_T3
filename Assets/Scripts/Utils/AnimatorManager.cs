using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;

public class AnimatorManager : IDisposable {
	private readonly ObjectPool<GameObject> pool;
	private readonly Transform parent;
	private readonly List<CancellationTokenSource> activeCtsList = new();	

	public AnimatorManager(GameObject prefab, Transform parent = null, int initialCapacity = 10, int maxSize = 100) {		

		this.parent = parent;

		pool = new ObjectPool<GameObject>(
			createFunc: () => {
				var go = UnityEngine.Object.Instantiate(prefab, parent);
				go.SetActive(false);
				return go;
			},
			actionOnGet: obj => obj.SetActive(true),
			actionOnRelease: obj => obj.SetActive(false),
			actionOnDestroy: UnityEngine.Object.Destroy,
			collectionCheck: true,
			defaultCapacity: initialCapacity,
			maxSize: maxSize
		);
	}

	public async UniTask PlayAsync(Vector3 position, float timeoutSeconds = 1f) {		

		if (timeoutSeconds <= 0f)
			timeoutSeconds = 1f;

		var obj = pool.Get();
		obj.transform.position = position;

		var cts = new CancellationTokenSource();
		lock (activeCtsList)
			activeCtsList.Add(cts);

		var token = cts.Token;

		try {
			await UniTask.Delay(TimeSpan.FromSeconds(timeoutSeconds), cancellationToken: token);

			if (!token.IsCancellationRequested && obj != null)
				pool.Release(obj);
		}
		catch (OperationCanceledException) {
			if (obj != null)
				pool.Release(obj);
		}
		finally {
			lock (activeCtsList)
				activeCtsList.Remove(cts);

			cts.Dispose();
		}
	}

	public void StopAll() {
		lock (activeCtsList) {
			foreach (var cts in activeCtsList)
				cts.Cancel();

			activeCtsList.Clear();
		}

		if (parent != null) {
			foreach (Transform child in parent) {
				if (child.gameObject.activeSelf)
					pool.Release(child.gameObject);
			}
		}
	}

	public void Dispose() {
		StopAll();
		// Принудительно уничтожим все объекты в пуле
		pool.Clear();
	}
}
