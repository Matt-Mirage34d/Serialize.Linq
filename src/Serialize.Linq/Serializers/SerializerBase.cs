﻿using System;
using System.Collections.Generic;
using System.Linq;
#if WINDOWS_PHONE7
using Serialize.Linq.Internals;
#endif
using System.Reflection;

namespace Serialize.Linq.Serializers
{
    public abstract class SerializerBase
    {
        private static readonly Type[] _knownTypes =
        { 
            typeof(bool),
            typeof(decimal), typeof(double),
            typeof(float),
            typeof(int), typeof(uint),
            typeof(short), typeof(ushort),
            typeof(long), typeof(ulong),
            typeof(string),
            typeof(DateTime), typeof(TimeSpan), typeof(Guid), typeof(DayOfWeek)
        };

        private readonly HashSet<Type> _customKnownTypes;

        protected SerializerBase()
        {
            _customKnownTypes = new HashSet<Type>();
        }

        public void AddKnownType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            _customKnownTypes.Add(type);
        }

        public void AddKnownTypes(IEnumerable<Type> types)
        {
            if (types == null)
                throw new ArgumentNullException("types");

            foreach (var type in types)
                this.AddKnownType(type);
        }

        protected virtual IEnumerable<Type> GetKnownTypes()
        {
            return ExplodeKnownTypes(_knownTypes).Concat(ExplodeKnownTypes(_customKnownTypes));
        }

        private static IEnumerable<Type> ExplodeKnownTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                yield return type;
                yield return type.MakeArrayType();
                if (!type.GetTypeInfo().IsClass)
                    yield return typeof(Nullable<>).MakeGenericType(type);
            }
        }
    }
}
