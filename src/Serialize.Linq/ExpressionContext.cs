﻿#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Reflection;
#if !WINDOWS_PHONE
using System.Collections.Concurrent;
#else
using Serialize.Linq.Internals;
#endif
using System.Linq.Expressions;
using Serialize.Linq.Nodes;

namespace Serialize.Linq
{
    public class ExpressionContext
    {
        private readonly ConcurrentDictionary<string, ParameterExpression> _parameterExpressions;
        private readonly ConcurrentDictionary<string, Type> _typeCache;

        public ExpressionContext()
        {
            _parameterExpressions = new ConcurrentDictionary<string, ParameterExpression>();
            _typeCache = new ConcurrentDictionary<string, Type>();
        }

        public virtual ParameterExpression GetParameterExpression(ParameterExpressionNode node)
        {
            if(node == null)
                throw new ArgumentNullException("node");
            var key = node.Type.Name + Environment.NewLine + node.Name;
            return _parameterExpressions.GetOrAdd(key, k => Expression.Parameter(node.Type.ToType(this), node.Name));
        }

        public virtual Type ResolveType(TypeNode node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (string.IsNullOrWhiteSpace(node.Name))
                return null;

            return _typeCache.GetOrAdd(node.Name, n =>
            {
                var type = Type.GetType(n);
                if (type == null)
                {
                    // Matt Lynch removed this part, because AppDomain is not in Portable Class Libraries.

                    //foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    //{
                    //    type = assembly.GetType(n);
                    //    if (type != null)
                    //        break;
                    //}

                    throw new NotSupportedException(string.Format("Unable to find Type: {0}", node.Name));

                }
                return type;
            });
        }
    }
}
