using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AOP
{
    class Program
    {
        static void Main(string[] args)
        {
            IRepository<User> useRepository = RepositoryFactory.Create<User>();

            Thread.CurrentPrincipal =
                new GenericPrincipal(new GenericIdentity("Administrator"), new[] { "ADMIN" });

            User user = new User() { Name = "Kubek" };
            useRepository.Add(user);
            useRepository.Update(user);
            useRepository.Delete(user);
            useRepository.GetAll();
            useRepository.GetById(0);
            Console.WriteLine("\r\nEnd program");
            Console.ReadLine();
        }
    }
}
