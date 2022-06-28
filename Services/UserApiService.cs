using MeerkatMvc.Models;
using MeerkatMvc.Repositories;

namespace MeerkatMvc.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _client;
    private readonly ITokensRepository _database;

    public UserApiService(HttpClient client, ITokensRepository database)
    {
        _client = client;
        _database = database;
    }

    public async Task<ProblemModel<UserModel>> SignUpAsync(string sessionId, SignUpModel user)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("sign_up", user);
        var json = response.Content as JsonContent;
        LoginResultModel? result = await json!.ReadFromJsonAsync<LoginResultModel>();
        await _database.AddTokensAsync(
                sessionId,
                new UserTokensModel(result!.AccessToken, result!.RefreshToken));
        return result!.User;
    }

    public async Task<ProblemModel<UserModel>> LogInAsync(string sessionId, LoginModel credentials)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync("log_in", credentials);
        var json = response.Content as JsonContent;
        LoginResultModel? result = await json!.ReadFromJsonAsync<LoginResultModel>();
        await _database.AddTokensAsync(
                sessionId,
                new UserTokensModel(result!.AccessToken, result!.RefreshToken));
        return result!.User;
    }

    public async Task<ProblemModel<UserModel>> GetUserAsync(string sessionId)
    {
        (string accessToken, string refreshToken) = await _database.GetTokensAsync(sessionId);
        var request = new HttpRequestMessage(HttpMethod.Get, "");
        request.Headers.Authorization = new("Bearer", accessToken);
        HttpResponseMessage response = await _client.SendAsync(request);
        var json = response.Content as JsonContent;
        UserModel? result = await json!.ReadFromJsonAsync<UserModel>();
        return result!;
    }

    public async Task<ProblemModel<UserModel>> UpdateUserAsync(string sessionId, UpdateModel model)
    {
        (string accessToken, string refreshToken) = await _database.GetTokensAsync(sessionId);
        var request = new HttpRequestMessage(HttpMethod.Put, "");
        request.Headers.Authorization = new("Bearer", accessToken);
        request.Content = JsonContent.Create(model);
        HttpResponseMessage response = await _client.SendAsync(request);
        var json = response.Content as JsonContent;
        UserModel? result = await json!.ReadFromJsonAsync<UserModel>();
        return result!;
    }

    public async Task<ProblemModel> DeleteUserAsync(string sessionId, DeleteModel model)
    {
        (string accessToken, string refreshToken) = await _database.GetTokensAsync(sessionId);
        HttpResponseMessage response;
        var request = new HttpRequestMessage(HttpMethod.Delete, "");
        request.Headers.Authorization = new("Bearer", accessToken);
        request.Content = JsonContent.Create(model);
        response = await _client.SendAsync(request);
        await _database.DeleteTokensAsync(sessionId);
        return new();
    }

}
