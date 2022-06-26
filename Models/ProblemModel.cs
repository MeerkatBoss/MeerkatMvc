namespace MeerkatMvc.Models;

public record ProblemModel<T> where T : class
{
    public T? Model {get; init;}
    public IDictionary<string, string[]> Errors {get; init;}
    public bool HasErrors => Errors.Count() > 1;
    
    public ProblemModel(T correct)
    {
        Model = correct;
        Errors = new Dictionary<string, string[]>();
    }

    public ProblemModel(ValidationProblemModel validationProblem)
    {
        Model = null;
        Errors = validationProblem.Errors;
    }

    public ProblemModel(string detailMessage)
    {
        Model = null;
        Errors = new Dictionary<string, string[]>();
        Errors.Add("Detail", new[] { detailMessage });
    }
}
