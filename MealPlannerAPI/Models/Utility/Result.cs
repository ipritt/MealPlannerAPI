namespace MealPlannerAPI.Models.Utility
{
    public class Result<T>
    {
        public T? Value { get; }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IReadOnlyList<Error> Errors { get; }

        private Result(T? value)
        {
            IsSuccess = true;
            Value = value;
            Errors = [];
        }

        private Result(IEnumerable<Error> errors)
        {
            IsSuccess = false;
            Value = default;
            Errors = errors.ToList().AsReadOnly();
        }

        public T TValue => IsSuccess
        ? Value!
        : throw new InvalidOperationException("Cannot access Value of a failed result.");

        // Factory Methods
        public static Result<T> Success(T? value) => new(value);
        public static Result<T> Failure(Error error) => new([error]);
        public static Result<T> Failure(IEnumerable<Error> errors) => new(errors);

        // Implicit Conversions for cleaner Service syntax
        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(Error error) => Failure(error);
        public static implicit operator Result<T>(List<Error> errors) => Failure(errors);
    }
}
