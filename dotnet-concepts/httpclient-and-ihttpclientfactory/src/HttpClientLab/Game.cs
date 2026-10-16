namespace HttpClientLab;

public sealed record Game(int Id, string Title);

public sealed class GameServiceTimeoutException(Exception inner)
    : Exception("The game service did not answer in time.", inner);
