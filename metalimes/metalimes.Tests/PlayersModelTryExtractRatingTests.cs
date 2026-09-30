using System.Reflection;
using System.Text.Json;
using metalimes.Pages;
using Xunit;

namespace metalimes.Tests;

public class PlayersModelTryExtractRatingTests
{
    [Fact]
    public void TryExtractRating_Parses_StringStandardRating_FromParseBotPayload()
    {
        // Arrange
        const string json = """
        {
          "status":"success",
          "data":{
            "players":[
              {
                "fide_id":"1023675",
                "name":"Boittin, Pascal",
                "title":"",
                "federation":"NED",
                "standard_rating":"1777",
                "rapid_rating":"",
                "blitz_rating":"1792",
                "birth_year":"1968"
              }
            ],
            "query":"1023675"
          }
        }
        """;

        using var doc = JsonDocument.Parse(json);
        var method = typeof(PlayersModel).GetMethod(
            "TryExtractRating",
            BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        object?[] args = [doc.RootElement, 0];

        // Act
        var success = (bool)method!.Invoke(null, args)!;
        var rating = (int)args[1]!;

        // Assert
        Assert.True(success);
        Assert.Equal(1777, rating);
    }
}
