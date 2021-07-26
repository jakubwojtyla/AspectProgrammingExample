using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOP
{
    public class RepositoryFactory
    {
        public static IRepository<T> Create<T>()
        {
            var repository = new Repository<T>();

            var logRepository = new LogProxy<IRepository<T>>(repository as IRepository<T>);

            var authenticationRepository = new AuthenticationProxy<IRepository<T>>(logRepository.GetTransparentProxy() as IRepository<T>);

            var flexibleRepository =
                new FlexibleProxy<IRepository<T>>(authenticationRepository.GetTransparentProxy() as IRepository<T>);


            flexibleRepository.BeforeExecute += (s, e) => flexibleRepository.Log("Flex Before executing '{0}'", e.MethodName);
            flexibleRepository.AfterExecute += (s, e) => flexibleRepository.Log("Flex After executing '{0}'", e.MethodName);
            flexibleRepository.ErrorExecuting += (s, e) => flexibleRepository.Log("Flex Error executing '{0}'", e.MethodName);

            flexibleRepository.Filter = (m) => m.Name.Contains("Get");

            return flexibleRepository.GetTransparentProxy() as IRepository<T>;
        }


    }
}
