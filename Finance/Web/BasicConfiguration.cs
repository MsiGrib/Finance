using DataModel;

namespace Web
{
    public class BasicConfiguration : BaseConfiguration
    {
        public string ApiUrl { get; private set; }
        public string ApiName { get; private set; }

        public BasicConfiguration(IConfiguration configuration) : base(configuration) { }
    }
}
