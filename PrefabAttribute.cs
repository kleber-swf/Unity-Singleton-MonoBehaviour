using System;

namespace DoD.Lib.Core {
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class PrefabAttribute : Attribute {
		public readonly string Name;
		public PrefabAttribute(string name) { Name = name; }
		public PrefabAttribute() { Name = null; }
	}
}