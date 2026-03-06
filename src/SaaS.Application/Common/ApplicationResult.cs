namespace SaaS.Application.Common;

public sealed class ApplicationResult<T>
{
    private ApplicationResult(bool succeeded, T? value, ErrorModel? error)
    {
        Succeeded = succeeded;
        Value = value;
        Error = error;
    }

    public bool Succeeded { get; }
    public T? Value { get; }
    public ErrorModel? Error { get; }

    public static ApplicationResult<T> Success(T value) => new(true, value, null);

    public static ApplicationResult<T> Failure(string code, string message, int statusCode = 400) =>
        new(false, default, new ErrorModel(code, message, statusCode));
}
