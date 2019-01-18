using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SessionManagerDemo.Models;
using SessionManagerDemo.Services;

namespace SessionManagerDemo.Controllers
{
    public class HomeController : Controller
    {
		private const string c_CONTRIVEDSESSIONKEY = "contrived";
		private const string c_NAMESESSIONKEY = "basicname";

		private readonly ISessionService _sessionService;

		public HomeController(ISessionService sessionService)
		{
			_sessionService = sessionService;
		}

		public IActionResult Index()
        {
			var name = _sessionService.Get<string>(c_NAMESESSIONKEY);
			var contrived = _sessionService.Get<ContrivedValues>("contrived") ?? new ContrivedValues { Name = "Guest" };

			var viewModel = new HomeViewModel
			{
				Name = name,
				Contrived = contrived
			};

			return View(viewModel);
		}

		[HttpPost]
		public IActionResult PostBasic(NameRequest request)
		{
			_sessionService.Set(c_NAMESESSIONKEY, request.Name);

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public IActionResult PostContrived(ContrivedValues request)
		{
			_sessionService.Set(c_CONTRIVEDSESSIONKEY, request);

			return RedirectToAction(nameof(Index));
		}

		public IActionResult DeleteContrived()
		{
			_sessionService.Remove(c_CONTRIVEDSESSIONKEY);

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
