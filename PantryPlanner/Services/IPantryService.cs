using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.Services
{
    interface IPantryService
    {
        PantryPlannerContext Context { get; set; }
    }
}
