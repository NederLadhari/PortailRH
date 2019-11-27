using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StagePFE.Startup))]
namespace StagePFE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
