namespace ASC.Api.Core.Routing;
public class ConstraintRoute : Attribute
{
    private readonly string _constraint;

    public ConstraintRoute(string constraint)
    {
        _constraint = constraint;
    }

    public IRouteConstraint GetRouteConstraint()
    {
        switch (_constraint)
        {
            case "int":
                return new IntRouteConstraint();
        }

        return null;
    }
}
