namespace Gateway.Response;

public record ResultResponse(bool IsSuccess, object? Data, Error[]? Errors);

public record Data(object Content);

public record Error(string Problem, string Details);

