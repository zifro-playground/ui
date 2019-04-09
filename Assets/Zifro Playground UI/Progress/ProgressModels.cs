using System.Collections.Generic;

namespace PM
{
	public class LevelProgress
	{
		public string levelId { get; set; }
		public bool isCompleted { get; set; }
		public string mainCode { get; set; }
		public int codeLineCount { get; set; }
		public int secondsSpent { get; set; }
	}

	public class GameProgress
	{
		public List<LevelProgress> levels { get; set; }
	}
}