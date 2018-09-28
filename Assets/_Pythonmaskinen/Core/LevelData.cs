namespace PM
{
	public class LevelData
	{
		public string Id;
		public bool HasShownDescription;
		public bool IsStarted;
		public bool IsCompleted;
		public string MainCode;
		public int CodeLineCount;
		public int SecondsSpent;

		public LevelData(string id)
		{
			Id = id;
		}
	}
}
