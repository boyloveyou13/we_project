using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(WE_Project.Views.Startup1))]

namespace WE_Project.Views
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
