namespace ZifroPlaygroundTests
{
	public class LevelTestData
	{
		public int levelIndex;
		public Level levelData;
		public Scene scene;

		public override string ToString()
		{
			return $"level '{levelData?.id}' (scene {scene?.name}, level index {levelIndex})";
		}
	}
}
