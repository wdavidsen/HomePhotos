using Dapper;
using System;

namespace SCS.HomePhotos.Model
{
    [Table("UserToken")]
    public class UserToken
    {
        [Key]
        public int TokenId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }

        public bool Refresh { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public DateTime Expiration { get; set; }

        public string AgentIdentifier { get; set; }
    }
}
