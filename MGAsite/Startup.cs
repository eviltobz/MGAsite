using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MGAsite.Startup))]
namespace MGAsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
