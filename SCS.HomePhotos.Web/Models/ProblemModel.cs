using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace SCS.HomePhotos.Web.Models
{
    public class ProblemModel
    {
        public ProblemModel()
        {

        }
        public ProblemModel(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(o => o.Errors);

            if (errors.Count() > 0)
            {
                Id = "InvalidRequestPayload";
                Message = errors.Select(e => e.ErrorMessage).Aggregate((s1, s2) => s1 + " " + s2);
            }
        }

        public string Id { get; set; }
        public string Message { get; set; }
    }
}
