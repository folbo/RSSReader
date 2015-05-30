using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;


namespace ATOMUltimate
{
    public static class IoC
    {
        private static IContainer _container;

        public static void Initialize(IContainer kernel)
        {
            _container = kernel;
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static T Resolve<T>(IEnumerable<Parameter> parameters)
        {
            return _container.Resolve<T>(parameters);
        }


    }
}
