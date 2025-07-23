using System;
using GameStore.api.Data;
using GameStore.api.Dtos;
using GameStore.api.Entities;
using GameStore.api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.api.Endpoints;

public static class GamesEndPoints
{
    const string GetGameEndpointName = "GetGame";

//     private static readonly List<GameSummaryDto> games = [
//        new (
//         1,
//         "Street Fighter II",
//         "Fighting",
//         19.99M,
//         new DateOnly(1992,7,15)),

//         new (
//         2,
//         "Final Fantasy XIV",
//         "RolePlaying",
//         59.99M,
//         new DateOnly(2010,9,30)),

//         new (
//         3,
//         "FIFA 23",
//         "Sports",
//         69.99M,
//         new DateOnly(2022,9,27))


//    ];

    public static RouteGroupBuilder MapGamesEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
                        .WithParameterValidation();
        //GET /games
        //group.MapGet("/", () => games);

        group.MapGet("/", async (GamesStoreContext dbContext) =>
           await  dbContext.Games
                     .Include(game => game.Genre)
                     .Select(game => game.ToGameSummaryDto())
                     .AsNoTracking()
                     .ToListAsync());


        //GET /games/1
        //         group.MapGet("/{id}", async (int id, GamesStoreContext dbContext) =>
        // {
        //     Game? game = await dbContext.Games.FindAsync(id);

        //     return game is null ?
        //      Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        // })
        // .WithName(GetGameEndpointName);

        app.MapGet("/games/{id}", async (int id, GamesStoreContext dbContext) =>
        {
            var game = await dbContext.Games
                .Include(g => g.Genre) // Important to include the Genre
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game is null)
                return Results.NotFound();

            return Results.Ok(game.ToGameSummaryDto());
        })
        .WithName(GetGameEndpointName);

        //POST /games

        group.MapPost("/", async (CreateGameDto newGame, GamesStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();
            //game.Genre = dbContext.Genres.Find(newGame.GenreId);

            // GameDto game = new(
            //     games.Count + 1,
            //     newGame.Name,
            //     newGame.Genre,
            //     newGame.Price,
            //     newGame.ReleaseDate

            // );
            // it came form GameMapping.cs
            // Game game = new()
            // {
            //     Name = newGame.Name,
            //     Genre = dbContext.Genres.Find(newGame.GenreId),
            //     GenreId = newGame.GenreId,
            //     Price = newGame.Price,
            //     ReleaseDate = newGame.ReleaseDate

            // };
            dbContext.Games.Add(game);
           await dbContext.SaveChangesAsync();

             // it also came form GameMapping.cs
            // GameDto gameDto = new( 
            //     game.Id,
            //     game.Name,
            //     game.Genre!.Name,
            //     game.Price,
            //     game.ReleaseDate


            // );

            return Results.CreatedAtRoute(
                GetGameEndpointName,
                 new { id = game.Id },
                  game.ToGameDetailsDto());

        });
       


// PUT /games
group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GamesStoreContext dbContext) =>
{
    //var index = games.FindIndex(game => game.Id == id);

    var existingGame = await dbContext.Games.FindAsync(id);

    if (existingGame is null)
    {
        return Results.NotFound();
    }

    dbContext.Entry(existingGame)
              .CurrentValues
              .SetValues(updatedGame.ToEntity(id));

    await dbContext.SaveChangesAsync();


    // games[index] = new GameSummaryDto(
    //     id,
    //     updatedGame.Name,
    //     updatedGame.Genre,
    //     updatedGame.Price,
    //     updatedGame.ReleaseDate
    // );

    return Results.NoContent();
});


        // DELETE /games
       // group.MapDelete("/{id}", (int id) =>

        group.MapDelete("/{id}", async (int id, GamesStoreContext dbContext) =>
        {
          await  dbContext.Games
                     .Where(game => game.Id == id)
                     .ExecuteDeleteAsync();

            return Results.NoContent();

        });

        return group;
   }


}
