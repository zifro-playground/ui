namespace ZifroPlaygroundTests
{
	public class CaseTestData : LevelTestData
	{
		public int caseIndex;
		public Case caseData;

		public override string ToString()
		{
			return
				$"case {(caseData is null ? "<?>" : (caseIndex + 1).ToString())}, level '{levelData?.id}' (scene {scene?.name}, level index {levelIndex})";
		}
	}
}
