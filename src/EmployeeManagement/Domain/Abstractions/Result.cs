﻿namespace EmployeeManagement.Domain.Abstractions;

public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("Success and error mismatch");
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("Error must be provided for failure");
        }

        IsSuccess = isSuccess;
        Errors = [error];
    }
    protected internal Result(bool isSuccess, List<Error> errors)
    {
        if (isSuccess && errors.Count > 0)
        {
            throw new InvalidOperationException("Success and error mismatch");
        }

        if (!isSuccess && errors.Count == 0)
        {
            throw new InvalidOperationException("Error must be provided for failure");
        }

        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public List<Error> Errors { get; }

    public static Result Success() => new Result(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result Failure(List<Error> errors) => new(false, errors);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
    public static Result<TValue> Failure<TValue>(List<Error> errors) => new(default, false, errors);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    protected internal Result(TValue? value, bool isSuccess, List<Error> errors)
        : base(isSuccess, errors)
    {
        _value = value;
    }

    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result cannot be accessed.");
}
