namespace GameStore.api.Dtos;

public record class GameSummaryDto (
    int Id,
    string Name,
    string Genre,
    Decimal Price,
    DateOnly ReleaseDate);
