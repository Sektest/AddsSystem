using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AddsSystem.Startup))]
namespace AddsSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
