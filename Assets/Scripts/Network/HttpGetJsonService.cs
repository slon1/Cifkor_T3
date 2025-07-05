using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Threading;

public class HttpGetJsonService : IHttpGetJsonService {
	private readonly IHttpGetService httpGetService;
	private CancellationTokenSource cts;

	public HttpGetJsonService(IHttpGetService http) {
		this.httpGetService = http;
	}

	public void CancelRequest() {
		{
			if (cts != null && !cts.IsCancellationRequested) {
				cts.Cancel();
				cts.Dispose();
				cts = null;
			}
		}
	}
	public void CancelRequest(string url) {
		httpGetService.CancelRequest(url);
	}
	public async UniTask<HttpResult<T>> GetJsonAsync<T>(string url, Action onStart = null, Action onComplete = null) {
		string raw;

		try {
			raw = await httpGetService.EnqueueRequest(url, onStart, onComplete);
		}
		catch (OperationCanceledException) {
			return HttpResult<T>.Failure("Request was canceled.", HttpResultErrorType.NetworkError);
		}
		catch (Exception ex) {
			return HttpResult<T>.Failure(ex.Message, HttpResultErrorType.NetworkError);
		}
		
		try {
			
			var obj = JsonConvert.DeserializeObject<T>(raw);

			if (obj == null)
				return HttpResult<T>.Failure("Deserialized object is null.", HttpResultErrorType.DeserializeError);

			return HttpResult<T>.Success(obj);
		}
		catch (Exception ex) {
			return HttpResult<T>.Failure($"JSON error: {ex.Message}", HttpResultErrorType.DeserializeError);
		}
	}
}
