using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TestTask.ProjectApi.Interfaces;

namespace TestTask.ProjectApi.Services;

public class UserApiClient : IUserApiClient
{
    private readonly HttpClient _httpClient;

    public UserApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<ICollection<string>> GetSubscriptionTypes(ICollection<int> userIds, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/user/subscriptionTypes")
        {
            Content = JsonContent.Create(new {userIds}),
        };

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<GetSubscriptionTypesResponse>(
                await response.Content.ReadAsStreamAsync(cancellationToken))
            .subscriptionTypes;
    }

    private class GetSubscriptionTypesResponse // TODO move request and response models to common library
    {
        public ICollection<string> subscriptionTypes { get; set; }
    }
}