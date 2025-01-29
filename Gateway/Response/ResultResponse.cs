namespace Gateway.Response;

public record ResultResponse(bool IsSuccess, Data? Data, Error[]? Errors);

public record Data(string Content);

public record Error(string Problem, string Details);

