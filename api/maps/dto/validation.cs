using Microsoft.IdentityModel.Tokens;

public class ValidationMapsDto
{
    public List<object> ValidateCreateInput(CreateMapsDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.Name))
        {
            errors.Add(new { Name = "Name is a required field." });
        }
        return errors;
    }
}