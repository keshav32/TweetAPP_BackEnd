namespace TweetAPP.Controller
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Confluent.Kafka;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using TweetAPP.Models;
    using TweetAPP.Service;

    /// <summary>
    /// TweetAppController.
    /// </summary>
    [Route("api/v1.0/tweets/")]
    [ApiController]
    public class TweetAppController : ControllerBase
    {
        private readonly ITweetAppService tweetAppService;
        private readonly IConfiguration configuration;
        private readonly ILogger<TweetAppController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetAppController"/> class.
        /// TweetAppController.
        /// </summary>
        /// <param name="tweetAppService">tweetAppService.</param>
        /// <param name="logger">logger.</param>
        /// <param name="configuration">configuration.</param>
        public TweetAppController(ITweetAppService tweetAppService, ILogger<TweetAppController> logger, IConfiguration configuration)
        {
            this.tweetAppService = tweetAppService;
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Register User.
        /// </summary>
        /// <param name="user">user.</param>
        /// <returns>response.</returns>
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                this.logger.LogInformation("Controller Register Method - Started Successfully");
                var result = await this.tweetAppService.Register(user);
                this.logger.LogInformation("Controller Register Method - Finished Successfully");

                using (var producer =
                 new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build())
                {
                    try
                    {
                        Console.WriteLine(producer.ProduceAsync("tweetapp_topic", new Message<Null, string> { Value = user.Username + " Registered Successfully!" })
                            .GetAwaiter()
                            .GetResult());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Oops, something went wrong: {e}");
                    }
                }

                return this.Ok(result);

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while registering user");
                throw;
            }
        }

        /// <summary>
        /// Login User.
        /// </summary>
        /// <param name="emailId">emailID.</param>
        /// <param name="password">password.</param>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("login/{username},{password}")]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                this.logger.LogInformation("Controller Login Method - Started Successfully");
                Token token = null;
                var result = await this.tweetAppService.UserLogin(username, password);
                if (result != null)
                {
                    this.logger.LogInformation("Controller Token Method- Token Generated - Started Successfully");
                    token = new Token() { UserId = result.UserId, Username = result.Username, Tokens = this.GenerateJwtToken(username), Message = "Success" };
                    this.logger.LogInformation("Controller Token Method- Token Generated - Finished Successfully");
                }
                else
                {
                    this.logger.LogInformation("Controller Token Method - Token Not Generated - Started Successfully");
                    token = new Token() { Tokens = null, Message = "UnSuccess" };
                    this.logger.LogInformation("Controller Token Method- Token Not Generated - Started Successfully");
                }
                this.logger.LogInformation("Controller Login Method - Finished Successfully");

                using (var producer =
                 new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build())
                {
                    try
                    {
                        Console.WriteLine(producer.ProduceAsync("tweetapp_topic", new Message<Null, string> { Value = username + " logged in!" })
                            .GetAwaiter()
                            .GetResult());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Oops, something went wrong: {e}");
                    }
                }

                return this.Ok(token);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while login user");
                throw;
            }
        }

        /// <summary>
        /// Post Tweet.
        /// </summary>
        /// <param name="tweet">tweet.</param>
        /// <returns>response.</returns>
        [HttpPost]
        [Route("tweet")]
        public async Task<IActionResult> Tweet(Tweet tweet)
        {
            try
            {
                this.logger.LogInformation("Controller Post Tweet Method - Started Succesfully");
                var result = await this.tweetAppService.PostTweet(tweet);
                this.logger.LogInformation("Controller Post Tweet Method - Finished Succesfully");

                using (var producer =
                 new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build())
                {
                    try
                    {
                        Console.WriteLine(producer.ProduceAsync("tweetapp_topic", new Message<Null, string> { Value = tweet.Tweets + " posted successfully!" })
                            .GetAwaiter()
                            .GetResult());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Oops, something went wrong: {e}");
                    }
                }

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while posting user tweet");
                throw;
            }
        }

        /// <summary>
        /// Delete Tweet.
        /// </summary>
        /// <param name="username">username.</param>
        /// <param name="tweet">tweet.</param>
        /// <returns>response.</returns>
        [HttpDelete]
        [Route("tweetdelete/{username},{tweet}")]
        public async Task<IActionResult> DeleteTweet(string username, string tweet)
        {
            try
            {
                this.logger.LogInformation("Controller Delete  Tweet Method - Started Succesfully");
                var result = await this.tweetAppService.DeleteTweet(username, tweet);
                this.logger.LogInformation("Controller Delete  Tweet Method - Finished Succesfully");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while Deleteing user tweet");
                throw;
            }
        }

        /// <summary>
        /// Get All Users.
        /// </summary>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("users/all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                this.logger.LogInformation("Controller Get All Users Method - Started Succesfully");
                var result = await this.tweetAppService.GetAllUsers();
                this.logger.LogInformation("Controller Get All Users Method - Finished Succesfully");

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while retrieving users");
                throw;
            }
        }

        /// <summary>
        /// Get Tweets By Users.
        /// </summary>
        /// <param name="username">username.</param>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("user/search/{username}")]
        public async Task<IActionResult> GetTweetsByUser(string username)
        {
            try
            {
                this.logger.LogInformation("Controller Get Tweets By User Method - Started Succesfully");
                var result = await this.tweetAppService.GetTweetsByUser(username);
                this.logger.LogInformation("Controller Get Tweets By User Method - Started Succesfully");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while fetching tweets by user");
                throw;
            }
        }

        /// <summary>
        /// Get All Tweets.
        /// </summary>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllTweets()
        {
            try
            {
                this.logger.LogInformation("Controller Get All Tweets Method - Started Succesfully");
                var result = await this.tweetAppService.GetAllTweets();
                this.logger.LogInformation("Controller Get All Tweets Method - Finished Succesfully");

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while fetching user tweets");
                throw;
            }
        }

        /// <summary>
        /// Get All Comments.
        /// </summary>
        /// <param name="username">username.</param>
        /// <param name="tweet">tweet.</param>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("allcomments/{username},{tweet}")]
        public async Task<IActionResult> GetAllComments(string username, string tweet)
        {
            try
            {
                this.logger.LogInformation("Controller Get All Comments Method - Started Succesfully");
                var result = await this.tweetAppService.GetComments(username, tweet);
                this.logger.LogInformation("Controller Get All Comments Method - Finished Succesfully");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while fetching user comments");
                throw;
            }
        }

        /// <summary>
        /// GetUserProfile.
        /// </summary>
        /// <param name="username">username.</param>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("user/{username}")]
        public async Task<IActionResult> GetUserProfile(string username)
        
       {
            try
            {
                this.logger.LogInformation("Controller Get User Profile Method - Started Succesfully");
                var result = await this.tweetAppService.GetUserProfile(username);
                this.logger.LogInformation("Controller Get User Profile Method - Finished Succesfully");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while fetching user");
                throw;
            }
        }

        /// <summary>
        /// UpdatePassword.
        /// </summary>
        /// <param name="emailId">emailId.</param>
        /// <param name="oldpassword">oldpassword.</param>
        /// <param name="newPassword">newPassword.</param>
        /// <returns>response.</returns>
        [HttpPut]
        [Route("update/{emailId},{oldpassword},{newpassword}")]
        public async Task<IActionResult> UpdatePassword(string emailId, string oldpassword, string newPassword)
        {
            try
            {
                this.logger.LogInformation("Controller Update Password Method - Started Succesfully");
                var result = await this.tweetAppService.UpdatePassword(emailId, oldpassword, newPassword);
                this.logger.LogInformation("Controller Update Password Method - Started Succesfully");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while updating user password");
                throw;
            }
        }

        /// <summary>
        /// ForgotPassword.
        /// </summary>
        /// <param name="emailId">emailId.</param>
        /// <param name="password">password.</param>
        /// <returns>response.</returns>
        [HttpPut]
        [Route("forgot/{emailId},{password}")]
        public async Task<IActionResult> ForgotPassword(string emailId, string password)
        {
            try
            {
                this.logger.LogInformation("Controller Forgot Password Method - Started Succesfully");
                var result = await this.tweetAppService.ForgotPassword(emailId, password);
                this.logger.LogInformation("Controller Forgot Password Method - Finished Succesfully");

                using (var producer =
                 new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build())
                {
                    try
                    {
                        Console.WriteLine(producer.ProduceAsync("tweetapp_topic", new Message<Null, string> { Value = " Password Updated Successfully" })
                            .GetAwaiter()
                            .GetResult());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Oops, something went wrong: {e}");
                    }
                }
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while reseting user password");
                throw;
            }
        }

        /// <summary>
        /// Comments.
        /// </summary>
        /// <param name="comment">comment.</param>
        /// <param name="username">username.</param>
        /// <param name="tweet">tweet.</param>
        /// <param name="date">date.</param>
        /// <returns>response.</returns>
        [HttpPost]
        [Route("reply/{comment},{username},{Name},{tweet}")]
        public async Task<IActionResult> PostComment(string comment, string username, string Name, string tweet)
        {
            try
            {
                this.logger.LogInformation("Controller Post Comment Method - Started Succesfully");
                var result = await this.tweetAppService.Comments(comment, username,Name, tweet);
                this.logger.LogInformation("Controller Post Comment Method - Finished Succesfully");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $" Error occured while posting user comment");
                throw;
            }
        }

        /// <summary>
        /// Likes.
        /// </summary>
        /// <param name="username">username.</param>
        /// <param name="tweet">tweet.</param>
        /// <returns>response.</returns>
        [HttpGet]
        [Route("likes/{username},{tweet}")]
        public async Task<IActionResult> GetLikes(string username, string tweet)
        {
            try
            {
                this.logger.LogInformation("Controller Get Likes Method - Started Succesfully");
                var result = await this.tweetAppService.Likes(username, tweet);
                this.logger.LogInformation("Controller Get Likes Method - Finished Succesfully");
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occured while getting user like");
                throw;
            }
        }

        private string GenerateJwtToken(string emailId)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, emailId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, emailId),
                new Claim(ClaimTypes.Role, emailId),
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["JwtKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //recommended is 5 min
            var expires = DateTime.Now.AddDays(Convert.ToDouble(this.configuration["JwtExpireDays"]));
            // the issuer will be changed when the app is deployed in cloud as of now the issuer is local host becasue angular app is loaded in local host
            var token = new JwtSecurityToken(
                this.configuration["JwtIssuer"],
                this.configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
