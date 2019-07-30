using PantryPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    public class RecipeStepDto
    {
        public int RecipeStepId { get; set; }
        public long RecipeId { get; set; }
        public string Text { get; set; }
        public int SortOrder { get; set; }

        #region Additional Properties Not In Model


        #endregion

        public RecipeStepDto(RecipeStep step)
        {
            if (step == null)
            {
                return;
            }

            RecipeStepId = step.RecipeStepId;
            RecipeId = step.RecipeId;
            Text = step.Text;
            SortOrder = step.SortOrder;
        }

        public static List<RecipeStepDto> ToList(IEnumerable<RecipeStep> list)
        {
            return list?.Select(k => new RecipeStepDto(k))?.ToList();
        }

        public override string ToString()
        {
            return $"rs: {Text}|{SortOrder}";
        }
    }
}
