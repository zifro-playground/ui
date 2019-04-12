using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Newtonsoft.Json;

public class JsonNetTests
{
	private struct Example
	{
		public string field1;
		public int field2;
		public bool field3;
	}

	[TestCase(1, "1")]
	[TestCase("foo", "\"foo\"")]
	[TestCase(1d, "1.0")]
	[TestCase(true, "true")]
	public void TestSerializePrimitives(object input, string expected)
	{
		string result = JsonConvert.SerializeObject(input);

		Assert.AreEqual(expected, result);
		Assert.Pass($"Object input: ({input.GetType().Name}) {input}\nSerialized output: {result}");
	}

	[Test]
	public void TestSerializeStruct()
	{
		var input = new Example {
			field1 = "hello world",
			field2 = -256,
			field3 = false
		};
		const string expected = "{" +
		                        "\"field1\":\"hello world\"," +
		                        "\"field2\":-256," +
		                        "\"field3\":false" +
		                        "}";

		string result = JsonConvert.SerializeObject(input, Formatting.None);

		Assert.AreEqual(expected, result);
		Assert.Pass($"Object input: ({input.GetType().Name}) {input}\nSerialized output: {result}");
	}

	[TestCase("250", 250)]
	[TestCase("\"moo\"", "moo")]
	[TestCase("1.0", 1d)]
	[TestCase("false", false)]
	public void TestDeserializePrimitives(string input, object expected)
	{
		object result = JsonConvert.DeserializeObject(input);

		Assert.AreEqual(expected, result);
		Assert.Pass($"Serialized input: {input}\nObject output: ({result.GetType().Name}) {result}");
	}

	[Test]
	public void TestDeserializeStruct()
	{
		const string field1 = "foo bar";
		const int field2 = -129;
		const bool field3 = true;
		const string input = "{" +
		                        "\"field1\":\"foo bar\"," +
		                        "\"field2\":-129," +
		                        "\"field3\":true" +
		                        "}";

		Example result = JsonConvert.DeserializeObject<Example>(input);

		Assert.AreEqual(field1, result.field1);
		Assert.AreEqual(field2, result.field2);
		Assert.AreEqual(field3, result.field3);
		Assert.Pass($"Serialized input: {input}\nObject output: ({result.GetType().Name}) {result}");
	}
}