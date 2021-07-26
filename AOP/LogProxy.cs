using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace AOP
{
    internal class LogProxy<T> : RealProxy
    {
        private readonly T _decorated;

        public LogProxy(T decorated)
            : base(typeof(T))
        {
            _decorated = decorated;
        }

        private void Log(string msg, object arg = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg, arg);
            Console.ResetColor();
        }

        
        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall.MethodBase as MethodInfo;

            Log("In Dynamic Proxy - Before executing '{0}'", methodCall.MethodName);

            var result = methodInfo.Invoke(_decorated, methodCall.InArgs);
            
            Log("In Dynamic Proxy - After executing '{0}' ", methodCall.MethodName);

            return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);

        }
    }
}
