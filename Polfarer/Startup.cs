using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Polfarer.Startup))]
namespace Polfarer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
