using FluentAssertions;
using Moq;
using Moq.Protected;

namespace AsyncThreadTest;

public class AsyncHttpRequestShould
{
    public async Task<string> GetDataFromJsonPlaceholder(HttpClient client)
    {
        HttpResponseMessage response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts/1");
        string responseData = await response.Content.ReadAsStringAsync();
        return responseData;
    }

    [Fact]
    public async Task ReturnExpectedData()
    {
        // Arrange
        var expectedResponse = "{ 'userId': 1, 'id': 1, 'title': 'Test title', 'body': 'Test body' }";
        
        var handlerMock = new Mock<HttpMessageHandler>();
        
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(expectedResponse),
            });

        var httpClient = new HttpClient(handlerMock.Object);

        // Act
        var result = await GetDataFromJsonPlaceholder(httpClient);

        // Assert
        result.Should().Contain("Test title");
        result.Should().Contain("Test body");
    }
}