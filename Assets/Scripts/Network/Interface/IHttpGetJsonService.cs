using Cysharp.Threading.Tasks;
using System;

public interface IHttpGetJsonService {
	UniTask<HttpResult<T>> GetJsonAsync<T>(string url, Action onStart = null, Action onComplete = null);
	void CancelRequest();
	void CancelRequest(string url);
}
