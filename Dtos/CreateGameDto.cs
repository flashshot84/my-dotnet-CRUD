using System.ComponentModel.DataAnnotations;

namespace GameStore.api.Dtos;

public record class CreateGameDto(
    
     [Required][StringLength(50)] string Name,
   int GenreId,
    [Range(1,100)] Decimal Price,
    DateOnly ReleaseDate);


