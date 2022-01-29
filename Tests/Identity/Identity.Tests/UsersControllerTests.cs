using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using RestEase;
using Shared.Features.Users;
using Shared.Persistence.Identity.Features.Users;
using Shared.Persistence.Mongo.Features.Database.Repository;
using Tests.Shared;

namespace Identity.Tests
{
    [TestFixture(Category = nameof(Identity))]
    [SuppressMessage("Style", "CC0061", MessageId = "Asynchronous method can be terminated with the \'Async\' keyword.")]
    public class UsersControllerTests : AuthenticatedTestsBase
    {

        [SetUp]
        public async Task SetUp()
        {
            _client = await GetIdentityRestClient();
        }

        [TearDown]
        public override async Task TearDown()
        {
            await ClearUsersButTestUser(GetIdentityHostService<IMongoClient>());
            await ClearCollection<UserDocument>(GetIdentityHostService<IMongoClient>());

            await base.TearDown();
        }

        public override string Password     => "P@a$$w0rd!";
        public override string ClientId     => "default";
        public override string ClientSecret => "secret";
        public override string Scope        => IdentityServerConstants.LocalApi.ScopeName;

        public override UserDocument TestUser { get; }
        private         IUsersApi    _client = null!;
        public UsersControllerTests()
        {
            TestUser = new()
            {
                Id       = ObjectId.GenerateNewId().ToString(),
                UserName = $"test_user_{TestUserSuffix:N}",
                Email    = "testuser@abhgsjkd.ocm",
                Roles =
                {
                    "user",
                    "admin",
                },
            };
        }

        [Test]
        public async Task Profile_Success()
        {
            // Arrange
            // Act
            var response = await _client.GetCurrentUserProfileAsync();
        
            // Assert
            response.Id.Should().Be(TestUser.Id);
            response.UserName.Should().Be(TestUser.UserName);
            response.Email.Should().Be(TestUser.Email);
            response.Roles.Should().BeEquivalentTo(TestUser.Roles);
        }

        [Test]
        public async Task CreateUser_Success()
        {
            // Arrange
            var request = new CreateUserDto
            {
                UserName = "john_doe123",
                Password = "Test1234!",
                Roles =
                {
                    "user",
                    "player",
                },
                Email = "test@example.com",
            };
            var mongoRepository = GetIdentityHostService<IMongoRepository<UserDocument>>();

            // Act
            var result = await _client.CreateUserAsync(request);

            // Assert
            result.Id.Should().NotBeEmpty();
            result.UserName.Should().Be(request.UserName);
            result.Email.Should().Be(request.Email);
            result.Roles.Should().BeEquivalentTo(request.Roles);
        }

        [Test]
        public async Task CreateUser_PasswordTooWeak()
        {
            // Arrange
            var request = new CreateUserDto
            {
                UserName = "john_doe123",
                Password = "123456",
                Roles =
                {
                    "user",
                    "player",
                },
                Email = "test@example.com",
            };

            // Act
            var act = async () => await _client.CreateUserAsync(request);

            // Assert
            await act.Should()
                     .ThrowAsync<ApiException>()
                     .Where(exception =>
                                exception.StatusCode == HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetUserById_Success()
        {
            // Arrange
            var password = "Test123!";
            var id       = ObjectId.GenerateNewId().ToString();
            var user = new UserDocument
            {
                Id       = id,
                UserName = "john_doe123",
                Roles =
                {
                    "user",
                    "player",
                },
                Email = "test@example.com",
            };

            await CreateUser(user, password);

            // Act
            var result = await _client.GetUserByIdAsync(id);

            // Assert
            result.Id.Should().NotBeEmpty();
            result.UserName.Should().Be(user.UserName);
            result.Email.Should().Be(user.Email);
            result.Roles.Should().BeEquivalentTo(user.Roles);
        }

        [Test]
        public async Task GetUserByUserName_Success()
        {
            // Arrange
            var password = "Test123!";
            var id       = ObjectId.GenerateNewId().ToString();
            var user = new UserDocument
            {
                Id       = id,
                UserName = "john_doe123",
                Roles =
                {
                    "user",
                    "player",
                },
                Email = "test@example.com",
            };

            await CreateUser(user, password);

            // Act
            var result = await _client.GetUserByUsernameAsync(user.UserName);

            // Assert
            result.Id.Should().NotBeEmpty();
            result.UserName.Should().Be(user.UserName);
            result.Email.Should().Be(user.Email);
            result.Roles.Should().BeEquivalentTo(user.Roles);
        }

        [Test]
        public async Task GetUserById_NotFound()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();

            // Act
            var act = async () => await _client.GetUserByIdAsync(id);

            // Assert
            await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
        }
        [Test]
        public async Task GetUserByUserName_NotFound()
        {
            // Arrange
            var userName = "notauser";

            // Act
            var act = async () => await _client.GetUserByUsernameAsync(userName);

            // Assert
            await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
        }

        [Test]
        public async Task UpdateUser_Success()
        {
            // Arrange
            var password = "Test123!";
            var id       = ObjectId.GenerateNewId().ToString();
            var user = new UserDocument
            {
                Id       = id,
                UserName = "john_doe123",
                Roles =
                {
                    "user",
                    "player",
                },
                Email = "john@doe.com",
            };

            var userManager = await CreateUser(user, password);

            var request = new UpdateUserDto
            {
                Email = "testuser@cms.cm",
            };

            // Act
            await _client.UpdateUserAsync(id, request);
            var result = await userManager.FindByIdAsync(id);

            result.Should().NotBeNull();
            result.UserName.Should().Be(user.UserName);
            result.Email.Should().Be(request.Email);
            result.Roles.Should().BeEquivalentTo(user.Roles);
        }

        [Test]
        public async Task DeleteUser_NotFound()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();

            // Act
            var act = async () => await _client.DeleteUserAsync(id);

            // Assert
            await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
        }

        [Test]
        public async Task UpdateUser_NotFound()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();

            // Act
            var act = async () =>
            {
                try
                {
                    await _client.UpdateUserAsync(id, new UpdateUserDto());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            };

            // Assert
            await act.Should().ThrowAsync<ApiException>().Where(x => x.StatusCode == HttpStatusCode.NotFound);
        }

        [Test]
        public async Task DeleteUser_Success()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var user = new UserDocument
            {
                Id       = id,
                UserName = "userName",
                Roles =
                {
                    "user",
                    "player",
                },
                Email = "email",
            };

            await CreateUser(user, "Test123!");

            // Act
            var act = async () =>
            {
                await _client.DeleteUserAsync(id);
            };

            // Assert
            await act.Should().NotThrowAsync<ApiException>();
        }

        public async Task<UserManager<UserDocument>> CreateUser(UserDocument user, string password)
        {
            var userManager    = GetIdentityHostService<UserManager<UserDocument>>();
            var identityResult = await userManager.CreateAsync(user, password);
            if (!identityResult.Succeeded)
                throw new InvalidOperationException(string.Join(", ", identityResult.Errors.Select(x => x.Description)));
            return userManager;
        }
    }
}