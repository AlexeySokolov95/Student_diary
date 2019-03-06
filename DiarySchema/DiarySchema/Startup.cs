using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using DiarySchema.Controllers;

[assembly: OwinStartupAttribute(typeof(DiarySchema.Startup))]
namespace DiarySchema
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
