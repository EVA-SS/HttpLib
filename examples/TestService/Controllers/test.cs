using Microsoft.AspNetCore.Mvc;

namespace TestService.Controllers
{
    [ApiController]
    [Route("/[action]")]
    public class test : ControllerBase
    {
        [HttpGet]
        public Model Get()
        {
            return new Model(this.Request);
        }

        [HttpPost]
        public Model Post()
        {
            return new Model(this.Request);
        }
    }
}