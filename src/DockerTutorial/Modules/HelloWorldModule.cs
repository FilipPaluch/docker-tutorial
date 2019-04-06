using System;
using System.Collections.Generic;
using System.Text;
using Carter;
using Microsoft.AspNetCore.Http;

namespace DockerTutorial.Modules
{
    public class HelloWorldModule : CarterModule
    {
        public HelloWorldModule()
        {
            Get("/", async (req, res, routeData) => await res.WriteAsync("Hello-World"));
        }
    }
}
