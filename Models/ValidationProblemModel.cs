namespace MeerkatMvc.Models;

public record ValidationProblemModel(
        string Type,
        string Title,
        int Status,
        string Detail,
        IDictionary<string, string[]> Errors,
        string? Instance = null
);
