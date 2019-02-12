using System;
using message_types;
using System.Threading.Tasks;
using System.Web.Mvc;
using MassTransit;

namespace autocomplete_example.Controllers
{
    public class SuggestController : Controller
    {
        // GET: Suggest
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Index(string prefix)
        {
            var address = new Uri("rabbitmq://localhost/suggestions");
            var requestTimeout = TimeSpan.FromSeconds(30);

            IRequestClient<IGetSuggestions, IReturnSuggestions> client = new MessageRequestClient<IGetSuggestions, IReturnSuggestions>(MvcApplication.BusControl, address, requestTimeout);
            IReturnSuggestions result = await client.Request(new { Prefix = prefix });

            return Json(result.Suggestions, JsonRequestBehavior.AllowGet);
        }
    }
}