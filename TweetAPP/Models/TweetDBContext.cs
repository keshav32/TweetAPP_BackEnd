namespace TweetAPP.Models
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// TweetDBContext.
    /// </summary>
    public class TweetDBContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TweetDBContext"/> class.
        /// </summary>
        public TweetDBContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetDBContext"/> class.
        /// </summary>
        /// <param name="options">options.</param>
        public TweetDBContext(DbContextOptions<TweetDBContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets user.
        /// </summary>
        public DbSet<User> User { get; set; }

        /// <summary>
        /// Gets or sets tweets.
        /// </summary>
        public DbSet<Tweet> Tweets { get; set; }

        /// <summary>
        /// Gets or sets TweetData.
        /// </summary>
        public DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// OnConfiguring.
        /// </summary>
        /// <param name="optionsBuilder">optionsBuilder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           optionsBuilder.UseSqlServer(@"Data Source=DELL-G15-BSK;Initial Catalog=TweetAppComp2;Persist Security Info=True;Integrated Security=SSPI");
        }
    }
}
