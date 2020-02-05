using System;

namespace QCrawler.EventArgs
{
    public class OverExceptionArg
    {
        
        public Exception exception;

        public OverExceptionArg() { }
        public OverExceptionArg(Exception exception)
        {
            this.exception = exception;
        }
    }
}