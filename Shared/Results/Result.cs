namespace Shared.Results;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Data { get; }
    public string[] Errors{ get; }

    private Result(bool isSuccess, T data, string[] errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data;
        Errors = errorMessage;
    }

    public static Result<T> Success(T data) => new Result<T>(true, data, null);
    public static Result<T> Failure(string[] errors) => new Result<T>(false, default, errors);
}

