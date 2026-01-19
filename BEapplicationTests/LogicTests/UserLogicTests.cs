using BEapplication.DBContexts;
using BEapplication.Models;
using BEapplication.RequestHandlers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BachelorThesisBETests.RequestHandlers
{
    [TestClass]
    public class UserLogicTests
    {
        private ApplicationContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationContext(options);
        }

        [TestMethod]
        public async Task Register_ShouldCreateUser_WhenEmailIsNew()
        {
            // Arrange
            var context = CreateDbContext();
            var logic = new UserLogic(context);

            var newUser = new RequestNewUser
            {
                Name = "Ana",
                Email = "ana@test.com",
                Password = "Password123!"
            };

            // Act
            await logic.Register(newUser);

            // Assert
            var user = await context.Users.FirstOrDefaultAsync();
            user.Should().NotBeNull();
            user!.Email.Should().Be("ana@test.com");
            BCrypt.Net.BCrypt.Verify("Password123!", user.PasswordHash).Should().BeTrue();
        }

        [TestMethod]
        public async Task Register_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var context = CreateDbContext();
            context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Name = "Ana",
                Email = "ana@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass")
            });
            await context.SaveChangesAsync();

            var logic = new UserLogic(context);

            var newUser = new RequestNewUser
            {
                Name = "Ana2",
                Email = "ana@test.com",
                Password = "AnotherPass"
            };

            // Act
            Func<Task> act = async () => await logic.Register(newUser);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Email deja folosit.");
        }

        [TestMethod]
        public async Task Login_ShouldReturnJwtToken_WhenCredentialsAreCorrect()
        {
            // Arrange
            var context = CreateDbContext();
            var password = "Secret123";

            context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Name = "Ana",
                Email = "login@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            });
            await context.SaveChangesAsync();

            var logic = new UserLogic(context);

            var model = new UserLoginModel
            {
                Email = "login@test.com",
                Password = password
            };

            // Act
            var token = await logic.Login(model);

            // Assert
            token.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task Login_ShouldReturnNull_WhenEmailDoesNotExist()
        {
            // Arrange
            var context = CreateDbContext();
            var logic = new UserLogic(context);

            var model = new UserLoginModel
            {
                Email = "missing@test.com",
                Password = "pass"
            };

            // Act
            var token = await logic.Login(model);

            // Assert
            token.Should().BeNull();
        }

        [TestMethod]
        public async Task Login_ShouldReturnNull_WhenPasswordIsWrong()
        {
            // Arrange
            var context = CreateDbContext();
            context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Name = "Ana",
                Email = "wrongpass@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct")
            });
            await context.SaveChangesAsync();

            var logic = new UserLogic(context);

            var model = new UserLoginModel
            {
                Email = "wrongpass@test.com",
                Password = "incorrect"
            };

            // Act
            var token = await logic.Login(model);

            // Assert
            token.Should().BeNull();
        }
    }
}
