using System;
using System.ComponentModel.DataAnnotations;

namespace TweetAPP.Models
{
    /// <summary>
    /// Like.
    /// </summary>
    public class UserComments
    {
        /// <summary>
        /// Gets or Sets Username.
        /// </summary>
        public string Username { get; set; }

        ///<summary>
        /// Gets or Sets Comment.
        /// </summary>
        public string Comments { get; set; }

        ///<summary>
        /// Gets or Sets Date.
        /// </summary>
        public DateTime Date { get; set; }

        ///<summary>
        /// Gets or Sets Imagename.
        /// </summary>
        public string Imagename { get; set; }
    }
}
