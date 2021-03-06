using System.Linq;
using Antix.Services.Validation.Predicates;

namespace Antix.Services.Validation.Rules
{
    public class ValidationRulePredicated<TModel> :
        ValidationRule<TModel>, 
        IValidationRulePredicated<TModel>
    {
        readonly ValidationRulePredicateGroupsValidator<TModel> _predicateGroups;

        public ValidationRulePredicated(
            IValidationRuleBuilder<TModel> builder,
            ValidationRulePredicateGroupsValidator<TModel> predicateGroups) :
                base(builder)
        {
            _predicateGroups = predicateGroups;
        }

        public IValidationRulePredicated<TModel> Or(
            IValidationPredicate<TModel> predicate,
            params IValidationPredicate<TModel>[] predicates)
        {
            _predicateGroups
                .Add(predicate.And(predicates).ToArray());

            return this;
        }
    }
}