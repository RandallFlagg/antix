using System;

namespace Antix.Services.Validation.Predicates
{
    public partial class StandardValidationPredicates :
        IStandardValidationPredicates
    {
        readonly ValidationPredicateCache<EqualPredicate, object> _equal
            = ValidationPredicateCache.Create(
                (object comparison) => new EqualPredicate(comparison));
        readonly ValidationPredicateCache<NotEqualPredicate, object> _notEqual
            = ValidationPredicateCache.Create(
                (object comparison) => new NotEqualPredicate(comparison));

        public IValidationPredicate<object> Equal(object value)
        {
            return _equal.GetOrCreate(value);
        }

        public IValidationPredicate<object> NotEqual(object value)
        {
            return _notEqual.GetOrCreate(value);
        }

        readonly IValidationPredicate<object> _null = new NullPredicate();
        readonly IValidationPredicate<object> _notNull = new NotNullPredicate();

        public IValidationPredicate<object> Null
        {
            get { return _null; }
        }

        public IValidationPredicate<object> NotNull
        {
            get { return _notNull; }
        }

        readonly IValidationPredicate<Guid> _guidEmpty = new GuidEmptyPredicate();
        readonly IValidationPredicate<Guid> _guidNotEmpty = new GuidNotEmptyPredicate();

        public IValidationPredicate<Guid> GuidEmpty
        {
            get { return _guidEmpty; }
        }

        public IValidationPredicate<Guid> GuidNotEmpty
        {
            get { return _guidNotEmpty; }
        }

        readonly ValidationPredicateCache<StringEqualPredicate, Tuple<string, StringComparison>> _stringEqual
            = ValidationPredicateCache.Create(
                (Tuple<string, StringComparison> t) => new StringEqualPredicate(t.Item1, t.Item2));
        readonly ValidationPredicateCache<StringNotEqualPredicate, Tuple<string, StringComparison>> _stringNotEqual
            = ValidationPredicateCache.Create(
                (Tuple<string, StringComparison> t) => new StringNotEqualPredicate(t.Item1, t.Item2));
        readonly IValidationPredicate<string> _empty = new StringEmptyPredicate();
        readonly IValidationPredicate<string> _nullOrEmpty = new StringNullOrEmptyPredicate();
        readonly IValidationPredicate<string> _nullOrWhiteSpace = new StringNullOrWhiteSpacePredicate();
        readonly IValidationPredicate<string> _notEmpty = new StringNotEmptyPredicate();
        readonly IValidationPredicate<string> _notNullOrEmpty = new StringNotNullOrEmptyPredicate();
        readonly IValidationPredicate<string> _notNullOrWhiteSpace = new StringNotNullOrWhiteSpacePredicate();
        readonly IValidationPredicate<string> _email = new EmailPredicate();

        public IValidationPredicate<string> Equal(string value, StringComparison comparison)
        {
            return _stringEqual.GetOrCreate(Tuple.Create(value, comparison));
        }

        public IValidationPredicate<string> NotEqual(string value, StringComparison comparison)
        {
            return _stringNotEqual.GetOrCreate(Tuple.Create(value, comparison));
        }
        
        public IValidationPredicate<string> Empty
        {
            get { return _empty; }
        }

        public IValidationPredicate<string> NullOrEmpty
        {
            get { return _nullOrEmpty; }
        }

        public IValidationPredicate<string> NullOrWhiteSpace
        {
            get { return _nullOrWhiteSpace; }
        }

        public IValidationPredicate<string> NotEmpty
        {
            get { return _notEmpty; }
        }

        public IValidationPredicate<string> NotNullOrEmpty
        {
            get { return _notNullOrEmpty; }
        }

        public IValidationPredicate<string> NotNullOrWhiteSpace
        {
            get { return _notNullOrWhiteSpace; }
        }

        readonly ValidationPredicateCache<StringLengthPredicate, Tuple<int, int>> _stringLength
            = ValidationPredicateCache.Create(
                (Tuple<int, int> k) => new StringLengthPredicate(k.Item1, k.Item2));

        readonly ValidationPredicateCache<StringMinLengthPredicate, Tuple<int>> _stringMinLength
            = ValidationPredicateCache.Create(
                (Tuple<int> k) => new StringMinLengthPredicate(k.Item1));

        readonly ValidationPredicateCache<StringMaxLengthPredicate, Tuple<int>> _stringMaxLength
            = ValidationPredicateCache.Create(
                (Tuple<int> k) => new StringMaxLengthPredicate(k.Item1));

        public IValidationPredicate<string> Length(int min, int max)
        {
            return _stringLength.GetOrCreate(Tuple.Create(min, max));
        }

        public IValidationPredicate<string> MinLength(int min)
        {
            return _stringMinLength.GetOrCreate(Tuple.Create(min));
        }

        public IValidationPredicate<string> MaxLength(int max)
        {
            return _stringMaxLength.GetOrCreate(Tuple.Create(max));
        }

        public IValidationPredicate<string> Email
        {
            get { return _email; }
        }
    }
}