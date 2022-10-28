using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace SCS.HomePhotos.Web.Models
{
    /// <summary>Problem model.</summary>
    public class ProblemModel
    {
        /// <summary>Initializes a new instance of the <see cref="ProblemModel" /> class.</summary>
        public ProblemModel()
        {

        }

        /// <summary>Initializes a new instance of the <see cref="ProblemModel" /> class.</summary>
        /// <param name="modelState">State of the model.</param>
        public ProblemModel(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(o => o.Errors);

            if (errors.Any())
            {
                Id = "InvalidRequestPayload";
                Message = errors.Select(e => e.ErrorMessage).Aggregate((s1, s2) => s1 + " " + s2);
            }
        }

        /// <summary>Gets or sets the problem id.</summary>
        /// <value>The problem id.</value>
        public string Id { get; set; }

        /// <summary>Gets or sets the problem message.</summary>
        /// <value>The problem message.</value>
        public string Message { get; set; }
    }
}
