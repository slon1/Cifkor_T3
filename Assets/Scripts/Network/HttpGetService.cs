using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class HttpGetService : IHttpGetService, IDisposable {
	public event Action<string> OnRequestStarted;
	public event Action<string> OnRequestCompleted;

	private readonly Queue<RequestData> requestQueue = new();
	private readonly Dictionary<string, CancellationTokenSource> activeRequests = new();
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
		foreach (var cts in activeRequests.Values) {
			cts.Cancel();
			cts.Dispose();
		}
		activeRequests.Clear();
	}

	public UniTask<string> EnqueueRequest(string url, Action onStart = null, Action onComplete = null) {
		
		requestQueue.RemoveWhere(r => r.Url == url);
		
		if (activeRequests.TryGetValue(url, out var cts)) {
			cts.Cancel();
			cts.Dispose();
			activeRequests.Remove(url);
		}

		
		var tcs = new UniTaskCompletionSource<string>();
		requestQueue.Enqueue(new RequestData {
			Url = url,
			OnStart = onStart,
			OnComplete = onComplete,
			CompletionSource = tcs
		});

		if (requestQueue.Count==1) {
			ProcessQueue().Forget();
		}

		return tcs.Task;
	}

	public void CancelRequest(string url) {
		
		requestQueue.RemoveWhere(r => r.Url == url);
		
		if (activeRequests.TryGetValue(url, out var cts)) {
			cts.Cancel();
			cts.Dispose();
			activeRequests.Remove(url);
		}
	}

	private async UniTaskVoid ProcessQueue() {
		

		while (requestQueue.Count > 0) {
			var request = requestQueue.Dequeue();
			var cts = new CancellationTokenSource();
			activeRequests[request.Url] = cts;

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

				activeRequests.Remove(request.Url);
				cts.Dispose();
			}
		}

		
	}
}
