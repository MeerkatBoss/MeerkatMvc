using System.Net;
using MeerkatMvc.Models;
using MeerkatMvc.Repositories;
using MeerkatMvc.Services;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace MeerkatMvc.Tests;

[TestFixture]
public class UserApiServiceTests
{
    protected readonly Mock<HttpMessageHandler> _messageHandlerMock;
    protected readonly Mock<ITokensRepository> _repositoryMock;

    public UserApiServiceTests()
    {
        _messageHandlerMock = new();
        _repositoryMock = new();
    }

    [TearDown]
    public void ClearMocks()
    {
        _messageHandlerMock.Reset();
        _repositoryMock.Reset();
    }

    [TestFixture]
    public class SignUpTests : UserApiServiceTests
    {
        [Test]
        public async Task TestSignUp()
        {
            string sessionId = "abc123";
            string jwt = "access";
            string refresh = "refresh";
            var model = new SignUpModel("test", "testtest");
            var result = new LoginResultModel(
                    jwt, refresh,
                    new UserModel(1, "test"));
            HttpRequestMessage sent = null!;
            _messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                .Callback((HttpRequestMessage request, CancellationToken token)
                        => sent = request)
                .Returns(Task.FromResult(new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.Created,
                            Content = JsonContent.Create(result),
                        }))
                .Verifiable();
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object),
                    _repositoryMock.Object);

            ProblemModel<LoginResultModel> userSignUp = await userApi.SignUp(sessionId, model);

            Assert.AreEqual(result, userSignUp.Model);
            Assert.False(userSignUp.HasErrors);
            Assert.NotNull(sent);
            Assert.AreEqual(HttpMethod.Post, sent.Method);
            JsonContent? json = sent.Content as JsonContent;
            Assert.NotNull(json);
            SignUpModel? sentModel = await json!.ReadFromJsonAsync<SignUpModel>();
            Assert.NotNull(sentModel);
            Assert.AreEqual(model, sentModel);
            _repositoryMock
                .Verify(x => x.AddTokensAsync(sessionId, new(jwt, refresh)),
                        Times.Once());
        }

    }

    [TestFixture]
    public class LogInTests : UserApiServiceTests
    {
        [Test]
        public async Task TestLogIn()
        {
            string jwt = "access";
            string refresh = "refresh";
            string sessionId = "abc123";
            var model = new LoginModel("test", "testtest");
            var result = new LoginResultModel(
                    jwt, refresh,
                    new UserModel(1, "test"));
            HttpRequestMessage sent = null!;
            _messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                .Callback((HttpRequestMessage request, CancellationToken token)
                        => sent = request)
                .Returns(Task.FromResult(new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = JsonContent.Create(result),
                        }))
                .Verifiable();
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object),
                    _repositoryMock.Object);

            ProblemModel<LoginResultModel> userLogIn = await userApi.LogIn(sessionId, model);

            Assert.AreEqual(result, userLogIn.Model);
            Assert.False(userLogIn.HasErrors);
            Assert.NotNull(sent);
            Assert.AreEqual(HttpMethod.Post, sent.Method);
            JsonContent? json = sent.Content as JsonContent;
            Assert.NotNull(json);
            LoginModel? sentModel = await json!.ReadFromJsonAsync<LoginModel>();
            Assert.NotNull(sentModel);
            Assert.AreEqual(model, sentModel);
            _repositoryMock
                .Verify(x => x.AddTokensAsync(sessionId, new(jwt, refresh)),
                        Times.Once());

        }
    }

    [TestFixture]
    public class GetUserTests : UserApiServiceTests
    {
        [Test]
        public async Task TestGetUser()
        {
            string sessionId = "abc123";
            string jwt = "access";
            string refresh = "refresh";
            var result = new UserModel(1, "test");
            HttpRequestMessage sent = null!;
            _messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                .Callback((HttpRequestMessage request, CancellationToken token)
                        => sent = request)
                .Returns(Task.FromResult(new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = JsonContent.Create(result),
                        }))
                .Verifiable();
            _repositoryMock
                .Setup(x => x.GetTokensAsync(sessionId))
                .ReturnsAsync(new UserTokensModel(jwt, refresh));
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object),
                    _repositoryMock.Object);

            ProblemModel<UserModel> userGet = await userApi.GetUser(sessionId);

            Assert.AreEqual(result, userGet.Model);
            Assert.False(userGet.HasErrors);
            Assert.NotNull(sent);
            Assert.AreEqual(HttpMethod.Get, sent.Method);
            Assert.AreEqual($"Bearer: {jwt}", sent.Headers.Authorization);
        }

    }

    [TestFixture]
    public class UpdateUserTests : UserApiServiceTests
    {
        [Test]
        public async Task TestUpdateUser()
        {
            string sessionId = "abc123";
            string jwt = "access";
            string refresh = "refresh";
            var result = new UserModel(1, "test_new");
            var updateModel = new UpdateModel(
                    OldPassword: "testtest",
                    Username: "test_new");
            HttpRequestMessage sent = null!;
            _messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                .Callback((HttpRequestMessage request, CancellationToken token)
                        => sent = request)
                .Returns(Task.FromResult(new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.OK,
                            Content = JsonContent.Create(result),
                        }))
                .Verifiable();
            _repositoryMock
                .Setup(x => x.GetTokensAsync(sessionId))
                .ReturnsAsync(new UserTokensModel(jwt, refresh));
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object),
                    _repositoryMock.Object);

            ProblemModel<UserModel> userUpdate = await userApi.UpdateUser(sessionId, updateModel);

            Assert.AreEqual(result, userUpdate.Model);
            Assert.False(userUpdate.HasErrors);
            Assert.NotNull(sent);
            Assert.AreEqual(HttpMethod.Put, sent.Method);
            Assert.AreEqual($"Bearer: {jwt}", sent.Headers.Authorization);
            JsonContent? json = sent.Content as JsonContent;
            Assert.NotNull(json);
            SignUpModel? sentModel = await json!.ReadFromJsonAsync<SignUpModel>();
            Assert.NotNull(sentModel);
            Assert.AreEqual(updateModel, sentModel);
        }

    }

    [TestFixture]
    public class DeleteUserTests : UserApiServiceTests
    {
        [Test]
        public async Task TestDeleteUser()
        {
            string sessionId = "abc123";
            string jwt = "access";
            string refresh = "refresh";
            var deleteModel = new DeleteModel("testtest");
            HttpRequestMessage sent = null!;
            _messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                .Callback((HttpRequestMessage request, CancellationToken token)
                        => sent = request)
                .Returns(Task.FromResult(new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.NoContent
                        }))
                .Verifiable();
            _repositoryMock
                .Setup(x => x.GetTokensAsync(sessionId))
                .ReturnsAsync(new UserTokensModel(jwt, refresh));
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object),
                    _repositoryMock.Object);

            await userApi.DeleteUser(sessionId, deleteModel);

            Assert.NotNull(sent);
            Assert.AreEqual(HttpMethod.Put, sent.Method);
            Assert.AreEqual($"Bearer: {jwt}", sent.Headers.Authorization);
            JsonContent? json = sent.Content as JsonContent;
            Assert.NotNull(json);
            SignUpModel? sentModel = await json!.ReadFromJsonAsync<SignUpModel>();
            Assert.NotNull(sentModel);
            Assert.AreEqual(deleteModel, sentModel);
            _repositoryMock.Verify(x => x.DeleteTokensAsync(sessionId), Times.Once());

        }

    }

}
