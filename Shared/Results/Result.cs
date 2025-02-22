namespace Shared.Results;

public class Result
{
    public bool IsSuccess { get; }
    public string[] Errors { get; }

    protected Result(bool isSuccess, string[] errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Failure(string[] errors) => new Result(false, errors);
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(bool isSuccess, T? data, string[] errors)
        : base(isSuccess, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new Result<T>(true, data, null);
    public static new Result<T> Failure(string[] errors) => new Result<T>(false, default, errors);
}
