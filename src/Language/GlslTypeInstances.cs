using System;
using System.Collections;
using System.Collections.Generic;

namespace DMS.GLSL
{
	public class GlslTypeInstances : IEnumerable<string>
	{
		public GlslTypeInstances(string instances)
		{
			//read keywords form text file resource
			var words = instances.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			//build hash-set of keywords
			foreach (var word in words)
			{
				this.instances.Add(word);
			}
		}

		public bool IsInstance(string word)
		{
			return instances.Contains(word);
		}

		public IEnumerator<string> GetEnumerator()
		{
			return instances.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return instances.GetEnumerator();
		}

		private readonly HashSet<string> instances = new HashSet<string>();
	}
}
