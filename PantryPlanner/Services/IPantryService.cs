using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.Services
{
    /// <summary>
    /// Interface that is used for performing CRUD operations on a <see cref="PantryPlannerContext"/> 
    /// in which the operations may contain custom business logic such as validation.
    /// </summary>
    interface IPantryService
    {
        PantryPlannerContext Context { get; set; }
    }
}
