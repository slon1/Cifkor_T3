using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public interface IHttpQueueService {
	/// <summary>
	/// Асинхронно выполняет GET-запрос из очереди.
	/// </summary>
	/// <param name="url">URL для GET-запроса.</param>
	/// <param name="onStart">Каллбэк, вызываемый перед началом фактической загрузки.</param>
	/// <param name="onComplete">Каллбэк, вызываемый после завершения загрузки.</param>
	/// <returns>UniTask<string>, который завершится со строковым результатом ответа.</returns>
	UniTask<string> GetRawAsync(string url, Action onStart = null, Action onComplete = null);
}

public interface IHttpService {
	UniTask<T> GetAsync<T>(string url, Action onStart = null, Action onComplete = null) where T : class;
}
public class HttpService : IHttpService {
	private readonly IHttpQueueService _queueService;

	public HttpService(IHttpQueueService queueService) {
		_queueService = queueService;
	}

	public async UniTask<T> GetAsync<T>(string url, Action onStart = null, Action onComplete = null) where T : class {
		try {
			string jsonResponse = await _queueService.GetRawAsync(url, onStart, onComplete);

			if (string.IsNullOrEmpty(jsonResponse)) {
				return null;
			}

			return JsonUtility.FromJson<T>(jsonResponse);
		}
		catch (OperationCanceledException) {
			// Просто передаем отмену дальше
			throw;
		}
		catch (Exception ex) {
			Debug.LogError($"[HttpService] Failed to get or parse data for type {typeof(T).Name} from URL {url}. Reason: {ex.Message}");
			// Перебрасываем исключение, чтобы вызывающий код мог его обработать
			throw;
		}
	}
}
public class HttpQueueService : IHttpQueueService, IDisposable {
	private class RequestInfo {
		public string Url { get; }
		public Func<CancellationToken, UniTask> TaskRunner { get; }
		public Action OnStart { get; }
		public Action OnComplete { get; }

		public RequestInfo(string url, Func<CancellationToken, UniTask> taskRunner, Action onStart, Action onComplete) {
			Url = url;
			TaskRunner = taskRunner;
			OnStart = onStart;
			OnComplete = onComplete;
		}
	}

	private readonly int _timeoutSeconds;
	private readonly CancellationTokenSource _destroyCts = new CancellationTokenSource();
	private readonly object _queueLock = new object();
	private readonly LinkedList<RequestInfo> _requestQueue = new LinkedList<RequestInfo>();

	private CancellationTokenSource _currentRequestCts;
	private bool _isProcessing;

	public HttpQueueService(int timeoutSeconds) {
		_timeoutSeconds = timeoutSeconds;
	}

	public UniTask<string> GetRawAsync(string url, Action onStart = null, Action onComplete = null) {
		var tcs = new UniTaskCompletionSource<string>();

		lock (_queueLock) {
			var existingNode = _requestQueue.FirstOrDefault(node => node.Url == url);
			if (existingNode != null) {
				_requestQueue.Remove(existingNode);
				Debug.Log($"[HttpQueueService] Removed duplicate request from queue: {url}");
			}

			if (_isProcessing && _currentRequestCts != null && !_currentRequestCts.IsCancellationRequested) {
				Debug.Log($"[HttpQueueService] A new request came in. Cancelling the current running request.");
				_currentRequestCts.Cancel();
			}

			Func<CancellationToken, UniTask> taskRunner = async (ct) => {
				try {
					using var uwr = UnityWebRequest.Get(url);
					uwr.timeout = _timeoutSeconds;

					await uwr.SendWebRequest().WithCancellation(ct);

					if (uwr.result == UnityWebRequest.Result.Success) {
						tcs.TrySetResult(uwr.downloadHandler.text);
					}
					else {
						tcs.TrySetException(new Exception($"Request failed: {uwr.error}"));
					}
				}
				catch (OperationCanceledException) {
					tcs.TrySetCanceled();
					Debug.Log($"[HttpQueueService] Request was cancelled: {url}");
				}
				catch (Exception ex) {
					tcs.TrySetException(ex);
				}
			};

			var requestInfo = new RequestInfo(url, taskRunner, onStart, onComplete);
			_requestQueue.AddLast(requestInfo);
			Debug.Log($"[HttpQueueService] Queued request: {url}. Queue size: {_requestQueue.Count}");

			ProcessQueueAsync().Forget();
		}

		return tcs.Task;
	}

	private async UniTaskVoid ProcessQueueAsync() {
		lock (_queueLock) {
			if (_isProcessing) return;
			_isProcessing = true;
		}

		Debug.Log("[HttpQueueService] Starting queue processing...");

		while (true) {
			RequestInfo requestToProcess;
			lock (_queueLock) {
				if (_requestQueue.Count == 0) {
					_isProcessing = false;
					break;
				}
				requestToProcess = _requestQueue.First.Value;
				_requestQueue.RemoveFirst();
			}

			_currentRequestCts = CancellationTokenSource.CreateLinkedTokenSource(_destroyCts.Token);

			Debug.Log($"[HttpQueueService] Starting request: {requestToProcess.Url}");
			requestToProcess.OnStart?.Invoke();

			try {
				await requestToProcess.TaskRunner(_currentRequestCts.Token);
			}
			catch (Exception ex) {
				Debug.LogError($"[HttpQueueService] Unhandled exception during request processing: {ex.Message}");
			}
			finally {
				requestToProcess.OnComplete?.Invoke();
				Debug.Log($"[HttpQueueService] Finished request: {requestToProcess.Url}");

				_currentRequestCts.Dispose();
				_currentRequestCts = null;
			}
		}

		Debug.Log("[HttpQueueService] Queue processing finished.");
	}

	public void Dispose() {
		_destroyCts.Cancel();
		_destroyCts.Dispose();
	}
}
