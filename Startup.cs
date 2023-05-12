using Microsoft.Identity.Web;
using SiseveebRaptortid.Services;

namespace SiseveebRaptortid
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string[] scopes = new string[] { ApiService.PowerBiDefaultScope };
            services.AddMicrosoftIdentityWebAppAuthentication(Configuration) 
                .EnableTokenAcquisitionToCallDownstreamApi(scopes).AddInMemoryTokenCaches();
                services.AddScoped(typeof(ApiService));
        }
    }
}