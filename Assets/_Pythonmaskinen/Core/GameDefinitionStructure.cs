using System.Collections.Generic;

public class ActiveLevel
{
	public string sceneName { get; set; }
	public string levelId { get; set; }
}

public class SceneSettings
{
	public float walkerStepTime { get; set; }
	public bool gameWindowUiLightTheme { get; set; }
	public List<string> availableFunctions { get; set; }
}

public class GuideBubble
{
	public string target { get; set; }
	public string text { get; set; }
}

public class TaskDescription{
    public string header { get; set; }
    public string body { get; set; }
}

public class LevelSettings
{
	public string precode { get; set; }
	public string startCode { get; set; }
	public int rowLimit { get; set; }
    public TaskDescription taskDescription { get; set; }
	public List<string> availableFunctions { get; set; }
}

/// <summary>
/// Can be used to specify custom level definitions in json. 
/// Should be implemented as partial class in file outside of _Pythonmaskinen.
/// </summary>
public partial class LevelDefinition
{
}

public class CaseSettings
{
	public string precode { get; set; }
	public float walkerStepTime { get; set; }
}

/// <summary>
/// Can be used to specify custom case definitions in json. 
/// Should be implemented as partial class in file outside of _Pythonmaskinen.
/// </summary>
public partial class CaseDefinition
{
}

public class Case
{
	public CaseSettings caseSettings { get; set; }
	public CaseDefinition caseDefinition { get; set; }
}

public class Level
{
	public string id { get; set; }
	public List<GuideBubble> guideBubbles { get; set; }
	public LevelSettings levelSettings { get; set; }
	public LevelDefinition levelDefinition { get; set; }
	public List<Case> cases { get; set; }
}

public class Scene
{
	public string name { get; set; }
	public SceneSettings sceneSettings { get; set; }
	public List<Level> levels { get; set; }
}

public class GameDefinition
{
	public List<ActiveLevel> activeLevels { get; set; }
	public List<Scene> scenes { get; set; }
}