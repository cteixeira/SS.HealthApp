using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.HealthApp.PCL.Exceptions
{
    public abstract class ServicesAuthenticationException : Exception
    {
        public ServicesAuthenticationException() { }

        public ServicesAuthenticationException(string Message) : base(Message) { }
    }

    public class AppAuthenticationException : ServicesAuthenticationException
    {
        public AppAuthenticationException() { }

        public AppAuthenticationException(string Message) : base(Message) { }
    }

    public class UserAuthenticationException : ServicesAuthenticationException
    {
        public UserAuthenticationException() { }

        public UserAuthenticationException(string Message) : base(Message) { }
    }

}
