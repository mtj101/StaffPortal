using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StaffPortal.Startup))]
namespace StaffPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
