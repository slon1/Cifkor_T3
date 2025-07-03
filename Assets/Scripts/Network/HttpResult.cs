public class HttpResult<T> {
	public T Value { get; }
	public string ErrorMessage { get; }
	public HttpResultErrorType ErrorType { get; }
	public bool IsSuccess => ErrorType == HttpResultErrorType.None;

	private HttpResult(T value) {
		Value = value;
		ErrorType = HttpResultErrorType.None;
	}

	private HttpResult(string errorMessage, HttpResultErrorType errorType) {
		ErrorMessage = errorMessage;
		ErrorType = errorType;
	}

	public static HttpResult<T> Success(T value) => new(value);
	public static HttpResult<T> Failure(string message, HttpResultErrorType type) => new(message, type);
}
