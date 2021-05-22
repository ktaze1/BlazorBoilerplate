using System;
using System.Collections.Generic;
using System.Text;

namespace Boilerplate.Shared.Models
{
    public class LoginResultDto
    {
        public string AccessToken { get; set; }

        public string EncryptedAccessToken { get; set; }

        public int ExpireInSeconds { get; set; }

        public long UserId { get; set; }

        public string[] Roles { get; set; }
    }
}
