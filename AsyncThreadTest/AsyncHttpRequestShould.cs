using FluentAssertions;
using Moq;
using Moq.Protected;

namespace AsyncThreadTest;

public class AsyncHttpRequestShould
{
    private async Task<string> GetDataFromJsonPlaceholder(HttpClient client)
    {
        var response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts/1");
        var responseData = await response.Content.ReadAsStringAsync();
        return responseData;
    }

    [Fact]
    public async Task ReturnExpectedData()
    {
        const string expectedResponse = "{ 'userId': 1, 'id': 1, 'title': 'Test title', 'body': 'Test body' }";
        
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

        var result = await GetDataFromJsonPlaceholder(httpClient);

        result.Should().Contain("Test title");
        result.Should().Contain("Test body");
    }
}