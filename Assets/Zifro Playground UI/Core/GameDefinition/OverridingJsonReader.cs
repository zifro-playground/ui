﻿using System;
using Newtonsoft.Json;

namespace PM
{
	public class OverridingJsonReader<TWhatToReplace, TWhatToRead> : JsonConverter<TWhatToReplace>
		where TWhatToRead : TWhatToReplace
	{
		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, TWhatToReplace value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}

		public override TWhatToReplace ReadJson(JsonReader reader, Type objectType, TWhatToReplace existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return serializer.Deserialize<TWhatToRead>(reader);
		}
	}
}