using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class HttpGetService : IHttpGetService, IDisposable {
	public event Action<string> OnRequestStarted;
	public event Action<string> OnRequestCompleted;

	private readonly Queue<RequestData> _requestQueue = new();
	private readonly Dictionary<string, CancellationTokenSource> _activeRequests = new();
	private readonly HttpGetConfig config;	

	private class RequestData {
		public string Url;
		public Action OnStart;
		public Action OnComplete;
		public UniTaskCompletionSource<string> CompletionSource;
	}

	public HttpGetService(HttpGetConfig config) {
		this.config = config;
		
	}

	
	public void Dispose() {
		foreach (var cts in _activeRequests.Values) {
			cts.Cancel();
			cts.Dispose();
		}
		_activeRequests.Clear();
	}

	public UniTask<string> EnqueueRequest(string url, Action onStart = null, Action onComplete = null) {
		// 1. Удаляем предыдущий запрос с этим URL из очереди
		_requestQueue.RemoveWhere(r => r.Url == url);

		// 2. Отменяем активный запрос на этот URL, если есть
		if (_activeRequests.TryGetValue(url, out var cts)) {
			cts.Cancel();
			cts.Dispose();
			_activeRequests.Remove(url);
		}

		// 3. Создаем новый запрос
		var tcs = new UniTaskCompletionSource<string>();
		_requestQueue.Enqueue(new RequestData {
			Url = url,
			OnStart = onStart,
			OnComplete = onComplete,
			CompletionSource = tcs
		});

		if (_requestQueue.Count==1) {
			ProcessQueue().Forget();
		}

		return tcs.Task;
	}

	public void CancelRequest(string url) {
		// 1. Если в очереди — удаляем
		_requestQueue.RemoveWhere(r => r.Url == url);

		// 2. Если в процессе — отменяем
		if (_activeRequests.TryGetValue(url, out var cts)) {
			cts.Cancel();
			cts.Dispose();
			_activeRequests.Remove(url);
		}
	}

	private async UniTaskVoid ProcessQueue() {
		

		while (_requestQueue.Count > 0) {
			var request = _requestQueue.Dequeue();
			var cts = new CancellationTokenSource();
			_activeRequests[request.Url] = cts;

			request.OnStart?.Invoke();
			OnRequestStarted?.Invoke(request.Url);

			try {
				using var webRequest = UnityWebRequest.Get(request.Url);

				foreach (var kvp in config.DefaultHeaders) {
					webRequest.SetRequestHeader(kvp.Key, kvp.Value);
				}

				webRequest.timeout = Mathf.CeilToInt(config.TimeoutSeconds);

				await webRequest.SendWebRequest().ToUniTask(cancellationToken: cts.Token);

				if (webRequest.result != UnityWebRequest.Result.Success) {
					request.CompletionSource.TrySetException(new Exception(webRequest.error));
				}
				else {
					request.CompletionSource.TrySetResult(webRequest.downloadHandler.text);
				}
			}
			catch (OperationCanceledException) {
				request.CompletionSource.TrySetCanceled();
			}
			catch (Exception ex) {
				request.CompletionSource.TrySetException(ex);
			}
			finally {
				request.OnComplete?.Invoke();
				OnRequestCompleted?.Invoke(request.Url);

				_activeRequests.Remove(request.Url);
				cts.Dispose();
			}
		}

		
	}
}
