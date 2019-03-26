using UnityEditor;

namespace Editor
{
	public class WebGLEditorScript
	{
		[MenuItem("WebGL/Enable Embedded Resources", false, 10)]
		public static void EnableErrorMessageTesting()
		{
#pragma warning disable 618
			PlayerSettings.SetPropertyBool("useEmbeddedResources", true, BuildTargetGroup.WebGL);
#pragma warning restore 618
		}

		[MenuItem("WebGL/Enable Embedded Resources", isValidateFunction: true)]
		public static bool EnableErrorMessageTestingValidate()
		{
#pragma warning disable 618
			return !PlayerSettings.GetPropertyBool("useEmbeddedResources", BuildTargetGroup.WebGL);
#pragma warning restore 618
		}

		[MenuItem("WebGL/Disable Embedded Resources", false, 20)]
		public static void DisableErrorMessageTesting()
		{
#pragma warning disable 618
			PlayerSettings.SetPropertyBool("useEmbeddedResources", false, BuildTargetGroup.WebGL);
#pragma warning restore 618
		}

		[MenuItem("WebGL/Disable Embedded Resources", isValidateFunction: true)]
		public static bool DisableErrorMessageTestingValidate()
		{
#pragma warning disable 618
			return PlayerSettings.GetPropertyBool("useEmbeddedResources", BuildTargetGroup.WebGL);
#pragma warning restore 618
		}
	}
}