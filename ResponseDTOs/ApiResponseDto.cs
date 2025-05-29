using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // For AllowNull

namespace KTUN_Final_Year_Project.ResponseDTOs
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; } // T can be nullable itself, e.g. ApiResponseDto<string?>
        public string? Message { get; set; }
        public List<string> Errors { get; set; }

        public ApiResponseDto(bool success, T data, string? message = null, List<string>? errors = null)
        {
            Success = success;
            Data = data;
            Message = message;
            Errors = errors ?? new List<string>();
        }

        // Parameterless constructor for deserialization if needed
        public ApiResponseDto() 
        {
            Errors = new List<string>();
            Data = default!; // Initialize Data to its default value (null for reference types)
            // For non-nullable reference types, you might need to initialize Success and Data if T is not nullable.
            // However, for a generic DTO, it's common to leave them to their default if not set by a specific constructor.
            // If T is a reference type, Data will be null by default.
            // If T is a value type, Data will be its default (e.g., 0 for int).
        }
    }
} 