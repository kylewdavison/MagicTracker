using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MagicTracker.Startup))]
namespace MagicTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
