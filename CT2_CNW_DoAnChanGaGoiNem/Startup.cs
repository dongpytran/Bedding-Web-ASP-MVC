using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CT2_CNW_DoAnChanGaGoiNem.Startup))]
namespace CT2_CNW_DoAnChanGaGoiNem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
