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
	private CancellationTokenSource _cts;
	private bool _isRunning;

	public bool IsRunning => _isRunning;

	public void Start(Func<UniTask> action, TimeSpan interval) {
		Stop(); // чтобы не запустилось дважды

		_cts = new CancellationTokenSource();
		_isRunning = true;

		RunLoop(action, interval, _cts.Token).Forget();
	}

	public void Stop() {
		if (!_isRunning)
			return;

		_cts?.Cancel();
		_cts?.Dispose();
		_cts = null;
		_isRunning = false;
	}

	private async UniTaskVoid RunLoop(Func<UniTask> action, TimeSpan interval, CancellationToken token) {
		try {
			while (!token.IsCancellationRequested) {
				await action();

				await UniTask.Delay(interval, cancellationToken: token);
			}
		}
		catch (OperationCanceledException) {
			// нормально при остановке
		}
		finally {
			_isRunning = false;
		}
	}
}

