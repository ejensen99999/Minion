using System;

namespace Minion.Core
{
    public class ThreadControl
    {
	    public static bool DoubleLock(object lockObj,
		    Func<bool> lockTest,
		    Action blockedAction)
	    {
		    var matched = true;

		    if (lockTest())
		    {
			    lock (lockObj)
			    {
				    if (lockTest())
				    {
					    blockedAction();
				    }
			    }
		    }
		    else
		    {
			    matched = false;
		    }

		    return matched;
	    }
    }
}
