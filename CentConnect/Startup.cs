using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CentConnect.Startup))]
namespace CentConnect
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
