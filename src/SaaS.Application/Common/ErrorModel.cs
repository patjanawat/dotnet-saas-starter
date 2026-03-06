namespace SaaS.Application.Common;

public sealed record ErrorModel(string Code, string Message, int StatusCode);
