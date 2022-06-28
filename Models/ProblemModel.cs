namespace MeerkatMvc.Models;

public class ProblemModel<T> where T : class
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

    private ProblemModel(IDictionary<string, string[]> errors)
    {
        Model = null;
        Errors = errors;
    }

    public static implicit operator ProblemModel<T>(T model) => new (model);

    public static implicit operator ProblemModel<T>(ProblemModel problem) => new(problem.Errors);

}

public class ProblemModel
{
    public IDictionary<string, string[]> Errors {get; init;}
    public bool HasErrors => Errors.Count() > 1;

    public ProblemModel()
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ProblemModel(ValidationProblemModel validationProblem)
    {
        Errors = validationProblem.Errors;
    }

    public ProblemModel(string detailMessage)
    {
        Errors = new Dictionary<string, string[]>();
        Errors.Add("Detail", new[] { detailMessage });
    }

}
