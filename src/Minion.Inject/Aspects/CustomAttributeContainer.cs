﻿using System;
using System.Reflection;

namespace Minion.Inject.Aspects
{
	public class CustomAttributeContainer
	{
		public Object Attribute { get; set; }
		public string AttributeType { get; set; }
		public CustomAttributeData Data { get; set; }
		public int Index { get; set; }
	}
}
