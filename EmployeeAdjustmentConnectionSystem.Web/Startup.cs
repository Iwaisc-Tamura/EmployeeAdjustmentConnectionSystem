using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmployeeAdjustmentConnectionSystem.Web.Startup))]
namespace EmployeeAdjustmentConnectionSystem.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
