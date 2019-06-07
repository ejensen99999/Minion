using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Minion.CP.Abstraction.Extensions
{
    public class CodeExtensionsTests
    {
		[Fact]
		public void ParseCode_ParsesGCode_IntoCodeBlocks()
		{
			CodeExtensions.ParseCode("g 1 X23 y 506 Z03457 F 50000");

		}


    }
}
