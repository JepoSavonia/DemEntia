using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemEntiaHRApplication.Models;

namespace DemEntiaHRApplication.Data
{
    public static class DbInit
    {
        public static void Init(DementiaContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
