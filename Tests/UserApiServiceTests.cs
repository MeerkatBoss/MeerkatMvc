using System.Net;
using System.Net.Http.Headers;
using System.Text;
using MeerkatMvc.Models;
using MeerkatMvc.Services;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace MeerkatMvc.Tests;

[TestFixture]
public class UserApiServiceTests
{
    protected readonly Mock<HttpMessageHandler> _messageHandlerMock;
    protected readonly Mock<ISession> _sessionMock;
    protected readonly string _baseAddress;

    public UserApiServiceTests()
    {
        _messageHandlerMock = new();
        _sessionMock = new();
        _baseAddress = "http://test.com/api/user/";
    }

    [TearDown]
    public void ClearMocks()
    {
        _messageHandlerMock.Reset();
        _sessionMock.Reset();
    }

    [TestFixture]
    public class SignUpTests : UserApiServiceTests
    {
        [Test]
        public async Task TestSignUp()
        {
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
                    new HttpClient(_messageHandlerMock.Object){BaseAddress = new Uri(_baseAddress)});

            ProblemModel<UserModel> userSignUp = await userApi.SignUpAsync(_sessionMock.Object, model);

            Assert.AreEqual(result.User, userSignUp.Model);
            Assert.False(userSignUp.HasErrors);
            Assert.NotNull(sent);
            Assert.AreEqual(HttpMethod.Post, sent.Method);
            Assert.AreEqual(new Uri(_baseAddress + "sign_up"), sent.RequestUri);
            JsonContent? json = sent.Content as JsonContent;
            Assert.NotNull(json);
            SignUpModel? sentModel = await json!.ReadFromJsonAsync<SignUpModel>();
            Assert.NotNull(sentModel);
            Assert.AreEqual(model, sentModel);
            _sessionMock
                .Verify(x => x.Set("AccessToken", Encoding.UTF8.GetBytes(jwt)), Times.Once());
            _sessionMock
                .Verify(x => x.Set("RefreshToken", Encoding.UTF8.GetBytes(refresh)), Times.Once());
            _sessionMock
                .Verify(x => x.Set("Username", Encoding.UTF8.GetBytes(model.Username)), Times.Once());
            _sessionMock
                .Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task TestSignUpBadRequest()
        {
            var model = new SignUpModel("@test", "test test");
            var problemList = new Dictionary<string, string[]>
            {
                {"Username", new[]{"Invalid characters"}},
                {"Password", new[]{"Invalid characters"}},
            };
            var result = new ValidationProblemModel(
                    Type: "Bad Request",
                    Title: "Validation Failed",
                    Status: 400,
                    Detail: "Invalid values provided",
                    Errors: problemList);
            _messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Content = JsonContent.Create(result),
                        }))
                .Verifiable();
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object){BaseAddress = new Uri(_baseAddress)});

            ProblemModel<UserModel> userSignUp = await userApi.SignUpAsync(_sessionMock.Object, model);

            Assert.True(userSignUp.HasErrors);
            Assert.AreEqual(problemList, userSignUp.Errors);
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
                    new HttpClient(_messageHandlerMock.Object){BaseAddress = new Uri(_baseAddress)});

            ProblemModel<UserModel> userLogIn = await userApi.LogInAsync(_sessionMock.Object, model);

            Assert.AreEqual(result.User, userLogIn.Model);
            Assert.False(userLogIn.HasErrors);
            Assert.NotNull(sent);
            Assert.AreEqual(HttpMethod.Post, sent.Method);
            Assert.AreEqual(new Uri(_baseAddress + "log_in"), sent.RequestUri);
            JsonContent? json = sent.Content as JsonContent;
            Assert.NotNull(json);
            LoginModel? sentModel = await json!.ReadFromJsonAsync<LoginModel>();
            Assert.NotNull(sentModel);
            Assert.AreEqual(model, sentModel);
            _sessionMock
                .Verify(x => x.Set("AccessToken", Encoding.UTF8.GetBytes(jwt)), Times.Once());
            _sessionMock
                .Verify(x => x.Set("RefreshToken", Encoding.UTF8.GetBytes(refresh)), Times.Once());
            _sessionMock
                .Verify(x => x.Set("Username", Encoding.UTF8.GetBytes(model.Login)), Times.Once());
            _sessionMock
                .Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task TestLogInFailed()
        {
            var model = new LoginModel("test", "testtest");
            _messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.Unauthorized
                        }))
                .Verifiable();
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object){BaseAddress = new Uri(_baseAddress)});

            ProblemModel<UserModel> userLogIn = await userApi.LogInAsync(_sessionMock.Object, model);

            Assert.True(userLogIn.HasErrors);
            Assert.AreEqual(1, userLogIn.Errors.Count());
            Assert.AreEqual(userLogIn.Errors["Detail"], new[] {UserApiService.LOGIN_FAILED});
        }

    }

    [TestFixture]
    public class GetUserTests : UserApiServiceTests
    {
        [Test]
        public async Task TestGetUser()
        {
            string jwt = "access";
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
            byte[] jwtBytes = Encoding.UTF8.GetBytes(jwt);
            _sessionMock
                .Setup(x => x.TryGetValue("AccessToken", out jwtBytes!));
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object){BaseAddress = new Uri(_baseAddress)});

            ProblemModel<UserModel> userGet = await userApi.GetUserAsync(_sessionMock.Object);

            Assert.AreEqual(result, userGet.Model);
            Assert.False(userGet.HasErrors);
            Assert.NotNull(sent);
            Assert.AreEqual(HttpMethod.Get, sent.Method);
            Assert.AreEqual(new Uri(_baseAddress), sent.RequestUri);
            Assert.AreEqual(new AuthenticationHeaderValue("Bearer", jwt), sent.Headers.Authorization);
        }

    }

    [TestFixture]
    public class UpdateUserTests : UserApiServiceTests
    {
        [Test]
        public async Task TestUpdateUser()
        {
            string jwt = "access";
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
            byte[] jwtBytes = Encoding.UTF8.GetBytes(jwt);
            _sessionMock
                .Setup(x => x.TryGetValue("AccessToken", out jwtBytes!));
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object){BaseAddress = new Uri(_baseAddress)});

            ProblemModel<UserModel> userUpdate = await userApi.UpdateUserAsync(_sessionMock.Object, updateModel);

            Assert.AreEqual(result, userUpdate.Model);
            Assert.False(userUpdate.HasErrors);
            Assert.NotNull(sent);
            Assert.AreEqual(HttpMethod.Put, sent.Method);
            Assert.AreEqual(new Uri(_baseAddress), sent.RequestUri);
            Assert.AreEqual(new AuthenticationHeaderValue("Bearer", jwt), sent.Headers.Authorization);
            JsonContent? json = sent.Content as JsonContent;
            Assert.NotNull(json);
            UpdateModel? sentModel = await json!.ReadFromJsonAsync<UpdateModel>();
            Assert.NotNull(sentModel);
            Assert.AreEqual(updateModel, sentModel);
            _sessionMock
                .Verify(x => x.Set("Username", Encoding.UTF8.GetBytes(updateModel.Username!)), Times.Once());
            _sessionMock
                .Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task TestUpdateUserBadRequest()
        {
            string jwt = "access";
            var model = new UpdateModel("@test", "test test");
            var problemList = new Dictionary<string, string[]>
            {
                {"Username", new[]{"Invalid characters"}},
                {"Password", new[]{"Invalid characters"}},
            };
            var result = new ValidationProblemModel(
                    Type: "Bad Request",
                    Title: "Validation Failed",
                    Status: 400,
                    Detail: "Invalid values provided",
                    Errors: problemList);
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
                            StatusCode = HttpStatusCode.BadRequest,
                            Content = JsonContent.Create(result),
                        }))
                .Verifiable();
            byte[] jwtBytes = Encoding.UTF8.GetBytes(jwt);
            _sessionMock
                .Setup(x => x.TryGetValue("AccessToken", out jwtBytes!));
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object){BaseAddress = new Uri(_baseAddress)});

            ProblemModel<UserModel> userUpdate = await userApi.UpdateUserAsync(_sessionMock.Object, model);

            Assert.True(userUpdate.HasErrors);
            Assert.AreEqual(problemList, userUpdate.Errors);
            Assert.NotNull(sent);
            Assert.AreEqual(HttpMethod.Put, sent.Method);
            Assert.AreEqual(new Uri(_baseAddress), sent.RequestUri);
            JsonContent? json = sent.Content as JsonContent;
            Assert.NotNull(json);
            UpdateModel? sentModel = await json!.ReadFromJsonAsync<UpdateModel>();
            Assert.NotNull(sentModel);
            Assert.AreEqual(model, sentModel);
            _sessionMock
                .Verify(x => x.Set("Username", It.IsAny<byte[]>()), Times.Never());
        }

        [Test]
        public async Task TestUpdateUserWrongPassword()
        {
            var model = new UpdateModel(OldPassword: "testtest", Username: "test");
            _messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.Unauthorized
                        }))
                .Verifiable();
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object){BaseAddress = new Uri(_baseAddress)});

            ProblemModel<UserModel> userUpdate = await userApi.UpdateUserAsync(_sessionMock.Object, model);

            Assert.True(userUpdate.HasErrors);
            Assert.AreEqual(1, userUpdate.Errors.Count());
            Assert.AreEqual(userUpdate.Errors["Detail"], new[] {UserApiService.WRONG_PASSWORD});
        }
    }

    [TestFixture]
    public class DeleteUserTests : UserApiServiceTests
    {
        [Test]
        public async Task TestDeleteUser()
        {
            string jwt = "access";
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
            byte[] jwtBytes = Encoding.UTF8.GetBytes(jwt);
            _sessionMock
                .Setup(x => x.TryGetValue("AccessToken", out jwtBytes!));
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object){BaseAddress = new Uri(_baseAddress)});

            ProblemModel deleteUser = await userApi.DeleteUserAsync(_sessionMock.Object, deleteModel);

            Assert.False(deleteUser.HasErrors);
            Assert.NotNull(sent);
            Assert.AreEqual(HttpMethod.Delete, sent.Method);
            Assert.AreEqual(new Uri(_baseAddress), sent.RequestUri);
            Assert.AreEqual(new AuthenticationHeaderValue("Bearer", jwt), sent.Headers.Authorization);
            JsonContent? json = sent.Content as JsonContent;
            Assert.NotNull(json);
            DeleteModel? sentModel = await json!.ReadFromJsonAsync<DeleteModel>();
            Assert.NotNull(sentModel);
            Assert.AreEqual(deleteModel, sentModel);
            _sessionMock
                .Verify(x => x.Clear(), Times.Once());
            _sessionMock
                .Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task TestDeleteUserWrongPassword()
        {
            var model = new DeleteModel("testtest");
            _messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.Unauthorized
                        }))
                .Verifiable();
            IUserApiService userApi = new UserApiService(
                    new HttpClient(_messageHandlerMock.Object){BaseAddress = new Uri(_baseAddress)});

            ProblemModel userDelete = await userApi.DeleteUserAsync(_sessionMock.Object, model);

            Assert.True(userDelete.HasErrors);
            Assert.AreEqual(1, userDelete.Errors.Count());
            Assert.AreEqual(userDelete.Errors["Detail"], new[] {UserApiService.WRONG_PASSWORD});
        }
    }

}
