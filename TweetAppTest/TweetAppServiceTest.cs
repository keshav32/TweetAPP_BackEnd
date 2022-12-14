using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TweetAPP.Models;
using TweetAPP.Repositories;
using TweetAPP.Service;
using Xunit;

namespace TweetAppTest
{
    /// <summary>
    /// TweetAppServiceTest.
    /// Testing all the methods of TweetService by Mocking the repository.
    /// </summary>
    public class TweetAppServiceTest
    {
        private readonly Mock<ILogger<TweetAppService>> _mockLogger;
        private readonly Mock<ITweetRepository> _mockTweetRepo;
        private readonly TweetAppService _tweetAppService;


        /// <summary>
        /// Initializes a new instance of the <see cref="TweetAppServiceTest"/> class.
        /// TweetAppServiceTest.
        /// </summary>
        public TweetAppServiceTest()
        {
            _mockLogger = new Mock<ILogger<TweetAppService>>();
            _mockTweetRepo = new Mock<ITweetRepository>();
            _tweetAppService = new TweetAppService(_mockTweetRepo.Object, _mockLogger.Object);
        }

        /// <summary>
        /// Mock Testing for ForgotPassword Method of TweetService
        /// </summary>
        [Fact]
        public async Task ForgotPassword_Test()
        {
            string emailId = "bijin@gmail.com";
            string password = "Bijin@123";
            _mockTweetRepo.Setup(x => x.ForgotPassword(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await _tweetAppService.ForgotPassword(emailId, password);

            Assert.NotNull(result);
            Assert.Equal("\"Changed Password\"", result);
            _mockTweetRepo.Verify(x => x.ForgotPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Mock Testing for PostTweet Method of TweetService
        /// </summary>
        [Fact]
        public async Task PostTweet_Test()
        {
            Tweet tweet = new Tweet
            {
                Id = 1,
                UserId = 3,
                Username = "akanksha",
                FirstName = "Akanksha",
                LastName = "Sahu",
                Tweets = "Heloo World",
                TweetDate = System.DateTime.Now
            };
            _mockTweetRepo.Setup(x => x.PostTweet(It.IsAny<Tweet>())).ReturnsAsync(2);
            var result = await _tweetAppService.PostTweet(tweet);
            Assert.Equal("\"Posted\"", result);
            _mockTweetRepo.Verify(x => x.PostTweet(It.IsAny<Tweet>()), Times.Once);
        }

        /// <summary>
        /// Mock Testing for GetAllTweets Method of TweetService
        /// </summary>
        [Fact]
        public async Task GetAllTweets_Test()
        {
            UserTweets tweet = new UserTweets
            {
                UserName = "Bijin@gmail.com",
                Tweets = "Hello World",
                FirstName = "Bijin",
                LastName = "Kurien",
                Likes = 3,
                TweetDate = System.DateTime.Now,
                Imagename = "Image.jpg"
            };

            List<UserTweets> tweets = new List<UserTweets>();
            tweets.Add(tweet);
            _mockTweetRepo.Setup(x => x.GetAllTweets()).ReturnsAsync(tweets);
            var result = await _tweetAppService.GetAllTweets();
            Assert.NotNull(result);
            Assert.Equal(tweets[0].UserName, result[0].UserName);
            Assert.Equal(tweets[0].Tweets, result[0].Tweets);
            Assert.Equal(tweets[0].FirstName, result[0].FirstName);
            Assert.Equal(tweets[0].LastName, result[0].LastName);
            Assert.Equal(tweets[0].Likes, result[0].Likes);
            Assert.Equal(tweets[0].TweetDate, result[0].TweetDate);
            Assert.Equal(tweets[0].Imagename, result[0].Imagename);
            _mockTweetRepo.Verify(X => X.GetAllTweets(), Times.Once);
        }

        /// <summary>
        /// Mock Testing for Comments Method of TweetService
        /// </summary>
        [Fact]
        public async Task Comments_Test()
        {
            string username = "akanksha";
            string userName = "bijin";
            string comment = "Hello World";
            string tweet = "Hello Akanksha";
            DateTime time = DateTime.Now;
            _mockTweetRepo.Setup(x => x.Comments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(2);
            var result = await _tweetAppService.Comments(comment, username, userName, tweet);
            Assert.Equal(2, result);
            _mockTweetRepo.Verify(x => x.Comments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once());
        }

        /// <summary>
        /// Mock Testing for GetAllUsers Method of TweetService
        /// </summary>
        [Fact]
        public async Task GetAllUsers_Test()
        {
            RegisteredUser user = new RegisteredUser
            {
                Username = "Bijin@gmail.com",
                FirstName = "Bijin",
                LastName = "Kurien",
                ImageName = "image.jpg"
            };
            List<RegisteredUser> Users = new List<RegisteredUser>();
            Users.Add(user);

            _mockTweetRepo.Setup(x => x.GetAllUsers()).ReturnsAsync(Users);

            var result = await _tweetAppService.GetAllUsers();

            Assert.NotNull(result);
            Assert.Equal(Users[0].Username, result[0].Username);
            Assert.Equal(Users[0].FirstName, result[0].FirstName);
            Assert.Equal(Users[0].LastName, result[0].LastName);
            Assert.Equal(Users[0].ImageName, result[0].ImageName);
            _mockTweetRepo.Verify(X => X.GetAllUsers(), Times.Once);
        }

        /// <summary>
        /// Mock Testing for GetTweetsByUser Method of TweetService
        /// </summary>
        [Theory]
        [InlineData("akanksha")]
        [InlineData("bijin")]
        [InlineData("Navya")]
        public async Task GetTweetsByUser_Test(string username_param)
        {
            UserTweets tweet = new UserTweets
            {
                UserName = "bijin@gmail.com",
                Tweets = "Hello World",
                FirstName = "Bijin",
                LastName = "Kurien",
                Likes = 3,
                TweetDate = System.DateTime.Now,
                Imagename = "Image.jpeg"
            };
            List<UserTweets> userTweets = new List<UserTweets>();
            userTweets.Add(tweet);

            string username = username_param;

            _mockTweetRepo.Setup(x => x.GetTweetsByUser(It.IsAny<string>())).ReturnsAsync(userTweets);

            var result = await _tweetAppService.GetTweetsByUser(username);

            Assert.NotNull(result);
            Assert.Equal(userTweets[0].UserName, result[0].UserName);
            Assert.Equal(userTweets[0].Tweets, result[0].Tweets);
            Assert.Equal(userTweets[0].FirstName, result[0].FirstName);
            Assert.Equal(userTweets[0].LastName, result[0].LastName);
            Assert.Equal(userTweets[0].Likes, result[0].Likes);
            Assert.Equal(userTweets[0].TweetDate, result[0].TweetDate);
            Assert.Equal(userTweets[0].Imagename, result[0].Imagename);
            _mockTweetRepo.Verify(x => x.GetTweetsByUser(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Mock Testing for GetUserProfile Method of TweetService
        /// </summary>
        [Theory]
        [InlineData("akanksha")]
        [InlineData("bijin")]
        [InlineData("Navya")]
        public async Task GetUserProfile_Test(string username_param)
        {
            User user = new User
            {
                UserId = 1,
                FirstName = "Bijin",
                LastName = "Kurien",
                Username = username_param,
                EmailId = "bijin@gmail.com",
                ContactNumber = "8871147488",
                Password = "Bijin@123",
                Tweet = new List<Tweet>(),
                ImageName = "image.jpeg"
            };

            string username = username_param;

            _mockTweetRepo.Setup(x => x.GetUserProfile(It.IsAny<string>())).ReturnsAsync(user);

            var result = await _tweetAppService.GetUserProfile(username);

            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Username, result.Username);
            _mockTweetRepo.Verify(x => x.GetUserProfile(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Mock Testing for Likes Method of TweetService
        /// </summary>
        [Theory]
        [InlineData("akanksha", "Hello World")]
        [InlineData("bijin", "Hello World")]
        public async Task Likes_Test(string username_param, string tweet_param)
        {
            string username = username_param;
            string tweet = tweet_param;
            _mockTweetRepo.Setup(x => x.Likes(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(2);

            var result = await _tweetAppService.Likes(username, tweet);

            Assert.Equal(2, result);
            _mockTweetRepo.Verify(x => x.Likes(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Mock Testing for UpdatePassword Method of TweetService
        /// </summary>
        [Fact]
        public async Task UpdatePassword_Test()
        {
            string emailId = "bijin@gmail.com";
            string oldPassword = "Bijin@1234";
            string newPassword = "Bijin@123";
            _mockTweetRepo.Setup(x => x.UpdatePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            var result = await _tweetAppService.UpdatePassword(emailId, oldPassword, newPassword);

            Assert.NotNull(result);
            Assert.Equal("\"Updated Successfully\"", result);
            _mockTweetRepo.Verify(x => x.UpdatePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        /// <summary>
        /// Mock Testing for Register Method of TweetService
        /// </summary>
        [Fact]
        public async Task Register_Test()
        {
            User user = new User
            {
                UserId = 3,
                Username = "bijin",
                FirstName = "Bijin",
                LastName = " Kurien",
                Tweet = new List<Tweet>(),
                Password = "Bijin@gmail.com",
                ContactNumber = "8871147488"
            };
            _mockTweetRepo.Setup(x => x.Register(It.IsAny<User>())).ReturnsAsync(2);
            var result = await _tweetAppService.Register(user);
            Assert.NotNull(result);
            Assert.Equal("Successfully registerd", result);
            _mockTweetRepo.Verify(x => x.Register(It.IsAny<User>()), Times.Once);
        }

        /// <summary>
        /// Mock Testing for UserLogin Method of TweetService
        /// </summary>
        [Fact]
        public async Task UserLogin_Test()
        {
            string username = "Asmita@gmail.com";
            string password = "Asmita@123";
            User user = new User
            {
                UserId = 1,
                FirstName = "Bijin",
                LastName = "Kurien",
                Username = "Bijin@gmail.com",
                EmailId = "bijin@gmail.com",
                ContactNumber = "8871147488",
                Password = "Bijin@123",
                Tweet = new List<Tweet>(),
                ImageName = "image.jpeg"
            };
            _mockTweetRepo.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(user);

            var result = await _tweetAppService.UserLogin(username, password);

            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Username, result.Username);
            _mockTweetRepo.Verify(x => x.Login(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Mock Testing for DeleteTweet Method of TweetService
        /// </summary>
        [Fact]
        public async Task DeleteTweet_Test()
        {
            string userName = "bijin";
            string tweet = "Hello Wolrd";
            _mockTweetRepo.Setup(x => x.DeleteTweet(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(2);
            var result = await _tweetAppService.DeleteTweet(userName, tweet);
            Assert.NotNull(result);
            Assert.Equal("\"Deleted\"", result);
            _mockTweetRepo.Verify(x => x.DeleteTweet(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
