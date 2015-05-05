using Microsoft.Owin;
using Owin;
using TestsProject;

[assembly: OwinStartup(typeof(Startup))]
namespace TestsProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
