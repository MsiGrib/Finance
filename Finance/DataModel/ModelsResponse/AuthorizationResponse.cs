using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ModelsResponse
{
    public class AuthorizationResponse
    {
        public DateTime ExpirationTimeToken { get; set; }
        public string Token { get; set; }
    }
}
