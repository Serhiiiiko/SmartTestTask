﻿namespace SmartTestTask.Application.DTOs.Responce;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; }
    
    public static ApiResponse<T> SuccessResponse(T data, string message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Errors = new List<string>()
        };
    }
    
    public static ApiResponse<T> FailureResponse(string message, List<string> errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}