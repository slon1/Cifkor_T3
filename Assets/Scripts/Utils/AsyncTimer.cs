using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public interface IAsyncTimer {
	void Start(Func<UniTask> action, TimeSpan interval);
	void Stop();
	bool IsRunning { get; }
}



public class AsyncTimer : IAsyncTimer {
	private CancellationTokenSource cts;
	private bool isRunning;

	public bool IsRunning => isRunning;

	public void Start(Func<UniTask> action, TimeSpan interval) {
		Stop(); 
		cts = new CancellationTokenSource();
		isRunning = true;

		RunLoop(action, interval, cts.Token).Forget();
	}

	public void Stop() {
		if (!isRunning)
			return;

		cts?.Cancel();
		cts?.Dispose();
		cts = null;
		isRunning = false;
	}

	private async UniTaskVoid RunLoop(Func<UniTask> action, TimeSpan interval, CancellationToken token) {
		try {
			while (!token.IsCancellationRequested) {
				await action();

				await UniTask.Delay(interval, cancellationToken: token);
			}
		}
		catch (OperationCanceledException) {
			
		}
		finally {
			isRunning = false;
		}
	}
}

