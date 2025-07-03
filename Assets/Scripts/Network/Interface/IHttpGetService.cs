using Cysharp.Threading.Tasks;
using System;

public interface IHttpGetService {
	UniTask<string> EnqueueRequest(string url, Action onStart = null, Action onComplete = null);
	void CancelRequest(string url);
}
