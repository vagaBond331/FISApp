using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FISApp.Startup))]
namespace FISApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
