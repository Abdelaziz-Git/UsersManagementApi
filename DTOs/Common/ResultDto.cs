namespace TailorSoftAPI.DTOs.Common
{
    public class ResultDto<T>
    {
        public T? Value { get; init; }
        public string? Error { get; init; }
        public bool IsSuccess => Error is null;

        public static ResultDto<T> Success(T value) => new() { Value = value };
        public static ResultDto<T> Failure(string error) => new() { Error = error };
    }
}
