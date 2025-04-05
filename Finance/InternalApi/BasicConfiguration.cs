using DataModel;

namespace InternalApi
{
    public class BasicConfiguration : BaseConfiguration
    {
        public string ConnectionString { get; private set; }
        public string SecretJWT { get; private set; }
        public string IssuerJWT { get; private set; }
        public string AudienceJWT { get; private set; }

        public BasicConfiguration(IConfiguration configuration) : base(configuration) { }
    }
}
