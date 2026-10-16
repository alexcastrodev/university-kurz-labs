namespace ProblemLab;

public class GameNotFoundException(int id) : Exception($"Game {id} not found");
