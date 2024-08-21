using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Owin;
using System;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

[assembly: OwinStartupAttribute(typeof(ASS2_20240802.Startup))]
namespace ASS2_20240802
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

        }  
    }


}

