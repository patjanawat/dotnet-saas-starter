namespace SaaS.Application.Common;

public sealed record ApplicationError(string Code, string Message);

public sealed class ApplicationResult<T>
{
    private ApplicationResult(bool succeeded, T? value, ApplicationError? error)
    {
        Succeeded = succeeded;
        Value = value;
        Error = error;
    }

    public bool Succeeded { get; }
    public T? Value { get; }
    public ApplicationError? Error { get; }

    public static ApplicationResult<T> Success(T value) => new(true, value, null);

    public static ApplicationResult<T> Failure(string code, string message) =>
        new(false, default, new ApplicationError(code, message));
}
