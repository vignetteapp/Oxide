// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Oxide.Javascript.Interop
{
    internal class HostMethod
    {
        public readonly Type DeclaringType;
        public readonly MethodBase Method;
        private readonly Func<object, object[], object> func;

        public HostMethod(Type type, MethodBase method)
        {
            Method = method;
            DeclaringType = type;

            LambdaExpression lambdaExpression = null;

            var argsExpression = Expression.Parameter(typeof(object[]), "Params");
            var paramExpressions = method.GetParameters().Select((p, i) =>
            {
                var constExpression = Expression.Constant(i, typeof(int));
                var indexExpression = Expression.ArrayIndex(argsExpression, constExpression);
                return Expression.Convert(indexExpression, p.ParameterType);
            });

            if (Method is MethodInfo mi)
            {
                var targetExpression = Expression.Parameter(typeof(object), "Target");
                var invokeExpression = Expression.Call(Expression.Convert(targetExpression, type), mi, paramExpressions);

                if (mi.ReturnType != typeof(void))
                {
                    lambdaExpression = Expression.Lambda(Expression.Convert(invokeExpression, typeof(object)), targetExpression, argsExpression);
                }
                else
                {
                    var nullExpression = Expression.Constant(null, typeof(object));
                    var bodyExpression = Expression.Block(invokeExpression, nullExpression);
                    lambdaExpression = Expression.Lambda(bodyExpression, targetExpression, argsExpression);
                }
            }

            if (Method is ConstructorInfo ci)
            {
                lambdaExpression = Expression.Lambda(Expression.Convert(Expression.New(ci, paramExpressions), typeof(object)), Expression.Parameter(typeof(object)), argsExpression);
            }

            func = (Func<object, object[], object>)lambdaExpression?.Compile();
        }

        public object Invoke(object target, object[] args) => func(target, args);
    }
}
