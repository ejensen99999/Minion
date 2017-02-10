using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minion
{
    public interface ILog
    {
        void Warn(string message);
        void Warn(string message, Exception ex);

        void Error(string message);
        void Error(string message, Exception ex);

        void Info(string message);
        void Info(string message, Exception ex);

        void Debug(string message);
        void Debug(string message, Exception ex);
    }
}
