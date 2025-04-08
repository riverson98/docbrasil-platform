using System.Linq.Expressions;

namespace DocAssociados.Application.Extensions;

public static class ExpressionConverter
{
    public static Expression<Func<TDestination, bool>> ConvertExpression<TSource, TDestination>(
        this Expression<Func<TSource, bool>> sourceExpression)
    {
        var parameter = Expression.Parameter(typeof(TDestination), sourceExpression.Parameters[0].Name);
        var body = new ExpressionReplacer(sourceExpression.Parameters[0], parameter).Visit(sourceExpression.Body);

        return Expression.Lambda<Func<TDestination, bool>>(body!, parameter);
    }

    private class ExpressionReplacer : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ExpressionReplacer(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == _oldValue)
            {
                var newMember = _newValue.Type.GetMember(node.Member.Name).FirstOrDefault();
                if (newMember == null)
                {
                    throw new ArgumentException($"Property '{node.Member.Name}' is not defined for type '{_newValue.Type}'");
                }
                return Expression.MakeMemberAccess(_newValue, newMember);
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldValue ? _newValue : base.VisitParameter(node);
        }
    }
}
