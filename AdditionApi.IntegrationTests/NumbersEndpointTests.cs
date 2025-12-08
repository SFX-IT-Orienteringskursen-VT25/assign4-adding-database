using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdditionApi.IntegrationTests;

public class NumbersEndpointTests : BaseIntegrationTest
{
    public NumbersEndpointTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Get_Root_ReturnsHelloWorld()
    {
        // Act
        var response = await Client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Hello World!");
    }

    [Fact]
    public async Task Get_StoredNumbers_ReturnsEmpty_WhenNoData()
    {
        // Arrange
        // Ensure database is clean for this test (or rely on test isolation if we had it, but here we share the container)
        // Since we share the container, we might need to clean up. 
        // For now, let's assume sequential execution or just check the structure.
        // Actually, xUnit runs tests in parallel by default for different classes, but here we only have one class.
        // Within the class, they are sequential? No, xUnit collections.
        // Let's clear the table before the test to be safe.
        DbContext.StorageItems.RemoveRange(DbContext.StorageItems);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync("/storage/enteredNumbers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<NumbersResponse>();
        content.Should().NotBeNull();
        content!.Numbers.Should().BeEmpty();
    }

    [Fact]
    public async Task Put_StoredNumbers_SavesData()
    {
        // Arrange
        var numbers = new[] { 1, 2, 3 };
        var request = new NumbersRequest(numbers);

        // Act
        var response = await Client.PutAsJsonAsync("/storage/enteredNumbers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<NumbersResponse>();
        content!.Numbers.Should().BeEquivalentTo(numbers);

        // Verify in DB
        var storedItem = await DbContext.StorageItems.FirstOrDefaultAsync(x => x.Key == "enteredNumbers");
        storedItem.Should().NotBeNull();
        storedItem!.Value.Should().Be("[1,2,3]");
    }

    [Fact]
    public async Task Put_StoredNumbers_UpdatesData()
    {
        // Arrange
        var initialNumbers = new[] { 1, 2, 3 };
        await Client.PutAsJsonAsync("/storage/enteredNumbers", new NumbersRequest(initialNumbers));

        var newNumbers = new[] { 4, 5, 6 };

        // Act
        var response = await Client.PutAsJsonAsync("/storage/enteredNumbers", new NumbersRequest(newNumbers));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<NumbersResponse>();
        content!.Numbers.Should().BeEquivalentTo(newNumbers);

        // Verify in DB
        // We need to detach or create a new scope to see updates if we were using the same context, 
        // but here we are using the context from the constructor which is created once per test.
        // However, the API uses its own scope. So the DB should be updated.
        // We might need to reload the entity or query again.
        // Since we are querying `FirstOrDefaultAsync` again, it should hit the DB or local cache.
        // To be safe, let's reload the change tracker or just query.
        DbContext.ChangeTracker.Clear();
        var storedItem = await DbContext.StorageItems.FirstOrDefaultAsync(x => x.Key == "enteredNumbers");
        storedItem.Should().NotBeNull();
        storedItem!.Value.Should().Be("[4,5,6]");
    }

    [Fact]
    public async Task Put_StoredNumbers_ReturnsBadRequest_WhenBodyIsInvalid()
    {
        // Act
        // Sending a string instead of the expected object structure
        var response = await Client.PutAsJsonAsync("/storage/enteredNumbers", "InvalidBody");

        // Assert
        // The API expects [FromBody] NumbersRequest. If we send a string, it might fail binding.
        // Actually PutAsJsonAsync serializes the string "InvalidBody" as a JSON string.
        // The endpoint expects a JSON object { "numbers": [...] }.
        // So sending a JSON string should result in 400 Bad Request.
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

public record NumbersResponse(int[] Numbers);
