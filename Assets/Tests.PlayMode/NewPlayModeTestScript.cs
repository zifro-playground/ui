using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
	public class NewPlayModeTestScript
	{
		// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
		// `yield return null;` to skip a frame.
		[UnityTest]
		public IEnumerator DestroyWithDelayPasses()
		{
			var obj = new GameObject("my object");
			Assert.IsTrue(obj);

			Object.Destroy(obj, 1);

			Assert.IsTrue(obj);

			yield return new WaitForSeconds(3);

			Assert.IsFalse(obj);
		}
	}
}
