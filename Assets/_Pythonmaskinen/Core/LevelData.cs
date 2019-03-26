namespace PM
{
	public class LevelData
	{
		public string id;
		public bool hasShownDescription;
		public bool isStarted;
		public bool isCompleted;
		public string mainCode;
		public int codeLineCount;
		public int secondsSpent;

		public LevelData(string id)
		{
			this.id = id;
		}

		public LevelData(LevelProgress levelProgress)
		{
			id = levelProgress.levelId;
			hasShownDescription = true;
			isStarted = true;
			isCompleted = levelProgress.isCompleted;
			mainCode = levelProgress.mainCode;
			codeLineCount = levelProgress.codeLineCount;
		}
	}
}