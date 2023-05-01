using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyAuthServer.Core.Configuration
{
    public class Client
    {
        // ClientId UserId gibi düşün
        public string Id { get; set; }

        // Client Secret password gibi düşün
        public string Secret { get; set; }

        // www.myapi1.com www.myapi2.com
        public List<String> Audiences { get; set; }
    }
}
