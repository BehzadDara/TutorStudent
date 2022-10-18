using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TutorStudent.Domain.DependencyInjectionAttribute
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class DependencyAttribute : Attribute
    {
        public ServiceLifetime DependencyType { get; set; }

        public Type ServiceType { get; set; }

        protected DependencyAttribute(ServiceLifetime dependencyType)
        {
            DependencyType = dependencyType;
        }

        public ServiceDescriptor BuildServiceDescriptor(TypeInfo type)
        {
            var serviceType = ServiceType ?? type.AsType();
            return new ServiceDescriptor(serviceType, type.AsType(), DependencyType);
        }
    }

    public class SingletonDependencyAttribute : DependencyAttribute
    {
        public SingletonDependencyAttribute()
            : base(ServiceLifetime.Singleton)
        { }
    }
    public class TransientDependencyAttribute : DependencyAttribute
    {
        public TransientDependencyAttribute()
            : base(ServiceLifetime.Transient)
        { }
    }
    public class ScopedDependencyAttribute : DependencyAttribute
    {
        public ScopedDependencyAttribute()
            : base(ServiceLifetime.Scoped)
        { }
    }
}
