using Microsoft.AspNetCore.Mvc;

namespace CacheCow.Samples.CarAPI.Controllers
{
    [Route("api/cars")]
    public class CarsController : Controller
    {
        [HttpGet(Name = "GetCars")]
        public IActionResult GetCars()
        {
            return Ok();
        }
    }
}
