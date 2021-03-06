using System.Threading.Tasks;

namespace Antix.Services.Validation.Predicates
{
    public class StringNullOrWhiteSpacePredicate : ValidationPredicateBase<string>
    {
        public override async Task<bool> IsAsync(string model)
        {
            return string.IsNullOrWhiteSpace(model);
        }
    }
}