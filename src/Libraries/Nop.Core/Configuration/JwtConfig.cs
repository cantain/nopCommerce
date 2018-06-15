using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Configuration
{
    public partial class JwtConfig
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string JwtSecret { get; set; }
    }
}
