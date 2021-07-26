using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace AOP
{
    class FlexibleProxy<T> : RealProxy
    {
        private readonly T _decorated;
        public event EventHandler<IMethodCallMessage> BeforeExecute;
        public event EventHandler<IMethodCallMessage> AfterExecute;
        public event EventHandler<IMethodCallMessage> ErrorExecuting;

        public FlexibleProxy(T decorated)
            : base(typeof(T))
        {
            _decorated = decorated;
        }

        public void Log(string msg, object arg = null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg, arg);
            Console.ResetColor();
        }

        private void OnBeforeExecute(IMethodCallMessage methodCall)
        {
            var beforeCopy = BeforeExecute;
            if (beforeCopy != null)
            {
                beforeCopy(this, methodCall);
            }
        }

        private void OnAfterExecute(IMethodCallMessage methodCall)
        {
            var afterExecure = AfterExecute;
            if (afterExecure != null)
            {
                afterExecure(this, methodCall);
            }
        }

        private void OnErrorExecuting(IMethodCallMessage methodCall)
        {

            var errorExecuting = ErrorExecuting;
            if (errorExecuting != null)
            {
                errorExecuting(this, methodCall);
            }
        }


        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall.MethodBase as MethodInfo;

            if (_filter(methodInfo) || methodInfo.CustomAttributes.Any(a => a.AttributeType == typeof(LogAttribute)))
                OnBeforeExecute(methodCall);
            try
            {
                var result = methodInfo.Invoke(_decorated, methodCall.InArgs);

                if (_filter(methodInfo) || methodInfo.CustomAttributes.Any(a => a.AttributeType == typeof(LogAttribute)))
                    OnAfterExecute(methodCall);

                return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception e)
            {
                if (_filter(methodInfo) || methodInfo.CustomAttributes.Any(a => a.AttributeType == typeof(LogAttribute)))
                    OnErrorExecuting(methodCall);

                return new ReturnMessage(null, methodCall);
            }

        }

        private Predicate<MethodInfo> _filter = m => true;

        public Predicate<MethodInfo> Filter
        {
            get { return _filter; }
            set
            {
                if (value == null)
                    _filter = m => true;
                else
                    _filter = value;
            }
        }
    }
}
