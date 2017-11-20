using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemEntiaHRApplication.ViewComponents
{
    public class NewUserViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke()
        {

            return View();
        }
    }
}
