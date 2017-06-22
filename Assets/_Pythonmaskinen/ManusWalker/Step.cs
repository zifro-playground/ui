using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace PM.Manus.Step {

	public sealed class SetSpeed : IStepNoBody {
		float speed = 0.5f;

		public void StartStep() {
			PMWrapper.speedMultiplier = speed;
		}

		public bool IsStepDone() {
			return true;
		}

		public void ParseContent(Manus manus, string header, string __) {
			if (!float.TryParse(header, out speed))
				throw new ManusBuildingException(this.GetType().Name + " was unable to parse \"" + header + "\" as a number!");
		}
	}

	public sealed class StopCode : IStepNoParameters {
		public void StartStep() {}

		public bool IsStepDone() {
			if (PMWrapper.isCompilerRunning) PMWrapper.StopCompiler();
			return !PMWrapper.isCompilerRunning;
		}

		public void ParseContent(Manus manus, string _, string __) {}
	}

	public sealed class Popup : IStep {
		public static object target = null;
		public static SetPopupSettings.Action doneAction = SetPopupSettings.Action.Undefined;
		public static bool slowPrint = false;

		string header;
		string body;

		public void StartStep() {
			if (target == null)
				throw new ManusBuildingException("Target for popup bubble has not been set!");
			if (doneAction == SetPopupSettings.Action.Undefined)
				throw new ManusBuildingException("Action for popup bubble has not been set!");
			if (doneAction == SetPopupSettings.Action.ClickTarget && !(target is string))
				throw new ManusBuildingException("Action " + SetPopupSettings.Action.ClickTarget.ToString() + " is only available for selectable targets! (got " + (target==null?"null":target.GetType().Name) + ")");

			if (target is string) {

				int index = UISingleton.instance.manusSelectables.FindIndex(s => s.names.Contains(target as string));
				if (index == -1) throw new ManusBuildingException("No selectable found with name \"" + target + "\"!");
				target = UISingleton.instance.manusSelectables[index].selectable;

				Vector2 pivot = new Vector2(0.5f,0.5f);
				if ((target as Selectable).GetComponent<SmartButton>())
					pivot = new Vector2(1, 1);

				if (doneAction == SetPopupSettings.Action.ClickNext)
					UISingleton.instance.manusBubble.ShowMessage((target as Selectable).transform as RectTransform, pivot);
				else
					UISingleton.instance.manusBubble.ShowMessage(target as Selectable, pivot);

			} else if (target is Vector3) {
				UISingleton.instance.manusBubble.ShowMessage(worldPosition: (Vector3) target);
			} else if (target is Vector2) {
				UISingleton.instance.manusBubble.ShowMessage(canvasPosition: (Vector2) target);
			} else if (target is int) {
				UISingleton.instance.manusBubble.ShowMessage(codeRow: (int) target);
			}

			UISingleton.instance.manusBubble.SetMessageContent(header, body);
		}

		public bool IsStepDone() {
			return !UISingleton.instance.manusBubble.isShowing && !UISingleton.instance.manusBubble.isFading;
		}

		public void ParseContent(Manus manus, string header, string body) {
			// Reset so they dont take values from previous manus
			target = null;
			doneAction = SetPopupSettings.Action.Undefined;
			slowPrint = false;

			this.header = header;
			this.body = body;
			//// Trim all the lines
			//this.body = string.Join("\n", new List<string>(body.Split('\n')).ConvertAll(line=>line.Trim()).ToArray());
		}
	}

	public sealed class SetPopupSettings : IStepNoHeader {
		object target = null;
		Action? doneAction = null;
		bool? slowPrint = false;

		public void StartStep() {
			if (target != null)
				Popup.target = target;
			if (doneAction.HasValue)
				Popup.doneAction = doneAction.Value;
			if (slowPrint.HasValue)
				Popup.slowPrint = slowPrint.Value;
		}

		public bool IsStepDone() {
			return true;
		}

		public void ParseContent(Manus manus, string _, string body) {
			string[] allRows = body.Split(Loader.linebreaks, StringSplitOptions.None);

			for (int i = 0; i < allRows.Length; i++) {
				string row = allRows[i].Trim();
				if (row.Length == 0) continue;

				Match match = Regex.Match(row, @"^([_\-\wåäöÅÄÖ.,]+)\s*=\s*(.+)\s*$");

				if (!match.Success) throw new ManusBuildingException("Unable to parse bubble settings \"" + allRows[i] + "\"");

				string g1 = match.Groups[1].Value.TrimEnd();
				string g2 = match.Groups[2].Value.TrimEnd();

				switch (g1.ToLower()) {
					case "action":
						try {
							doneAction = (Action)Enum.Parse(typeof(Action), g2, true);
							if (doneAction == Action.Undefined) throw new Exception();
						} catch {
							throw new ManusBuildingException("Unable to parse action, no enum value matches \"" + g2 + "\"");
						}
						break;
						
					case "target":
						Match m = Regex.Match(g2, @"^\(([\s\d\.,\-;]+)\)$");

						if (m.Success) {
							// It's in paranteses. Convert all to numbers
							string content = m.Groups[1].Value;
							List<float> numbers;
							try {
								numbers = content.Split(';',',').Select(s => float.Parse(s.Replace(" ", "").Replace("\t", ""))).ToList();
							} catch {
								throw new ManusBuildingException("Unable to parse target position. Invalid number representations in \"" + g2 + "\"");
							}

							switch (numbers.Count) {
								case 1: target = Mathf.RoundToInt(numbers[0]); break;
								case 2: target = new Vector2(numbers[0], numbers[1]); break;
								case 3: target = new Vector3(numbers[0], numbers[1], numbers[2]); break;

								default:
									throw new ManusBuildingException("Invalid number of number values (" + numbers.Count + "). Only (row), (x,y), and (x,y,z) are valid.");
							}

						} else {
							// May be a selectable
							target = g2;
						}
						break;

					case "slowprint":
						slowPrint = MyParseBool(g2);
						if (!slowPrint.HasValue) throw new ManusBuildingException("Unable to parse slowprint, invalid boolean value \"" + g2 + "\"!");
						break;

					default:
						throw new ManusBuildingException("Unknown bubble setting \"" + g1 + "\"");
				}
			}
		}

		private static bool? MyParseBool(string v) {
			switch (v.ToLower().Trim()) {
				case "1":
				case "yes":
				case "on":
				case "true":
				case "på":
					return true;

				case "0":
				case "no":
				case "off":
				case "false":
				case "av":
					return false;

				default:
					return null;
			}
		}

		public enum Action {
			Undefined,
			ClickNext,
			ClickTarget,
		}

		public enum Target {
			Undefined,
			Selectable,
			ScreenPoint,
			WorldPoint,
			Row
		}
	}

	public sealed class SetCode : IStepNoHeader {
		string code;
		Code target;

		public SetCode(Code target) {
			this.target = target;
		}

		public void StartStep() {
			if (target == Code.MainCode)
				PMWrapper.mainCode = code + '\n';
			else if (target == Code.PreCode)
				PMWrapper.preCode = code;
			else if (target == Code.PostCode)
				PMWrapper.postCode = code;
			Canvas.ForceUpdateCanvases();
		}

		public bool IsStepDone() {
			return true;
		}

		public void ParseContent(Manus manus, string _, string body) {
			code = body ?? string.Empty;
		}
	}
	
	public sealed class ClearCode : IStepNoParameters {
		Code target;

		public ClearCode(Code target) {
			this.target = target;
		}

		public void StartStep() {
			if (target == Code.All)
				PMWrapper.preCode = 
				PMWrapper.postCode = 
				PMWrapper.mainCode = string.Empty;
			else if (target == Code.MainCode)
				PMWrapper.mainCode = string.Empty;
			else if (target == Code.PreCode)
				PMWrapper.preCode = string.Empty;
			else if (target == Code.PostCode)
				PMWrapper.postCode = string.Empty;
			Canvas.ForceUpdateCanvases();
		}

		public bool IsStepDone() {
			return true;
		}

		public void ParseContent(Manus manus, string _, string __) {}
	}

	public sealed class WalkSteps : IStepNoBody {
		int p_steps;
		int stepsLeft;

		public void StartStep() {
			stepsLeft = p_steps;
			if (!PMWrapper.isCompilerRunning)
				PMWrapper.StartCompiler();
		}

		public void ParseContent(Manus manus, string header, string _) {
			if (!int.TryParse(header.Trim(), out p_steps))
				throw new ManusBuildingException(this.GetType().Name + " was unable to parse \"" + header + "\" as an integer!");
		}

		public bool IsStepDone() {
			if (!PMWrapper.isCompilerRunning)
				PMWrapper.StartCompiler();

			if (CodeWalker.manusPlayerSaysICanContinue == false && stepsLeft > 0) {
				stepsLeft--;
				CodeWalker.manusPlayerSaysICanContinue = stepsLeft > 0;
			}
			
			// This step is ready when we no longer need the walker to continue
			return CodeWalker.manusPlayerSaysICanContinue == false && stepsLeft == 0;
		}
	}

	public sealed class WalkTilFinished : IStepNoParameters {
		public void StartStep() {
			if (!PMWrapper.isCompilerRunning)
				PMWrapper.StartCompiler();
		}

		public bool IsStepDone() {
			if (PMWrapper.isCompilerRunning)
				CodeWalker.manusPlayerSaysICanContinue = true;

			return !PMWrapper.isCompilerRunning;
		}

		public void ParseContent(Manus manus, string _, string __) {}
	}

	public sealed class WaitSeconds : IStepNoBody {
		float startstamp;
		float waittime;

		public void StartStep() {
			startstamp = Time.unscaledTime;
		}

		public bool IsStepDone() {
			return Time.unscaledTime - startstamp >= waittime;
		}

		public void ParseContent(Manus manus, string header, string _) {
			if (!float.TryParse(header.Trim(), out waittime))
				throw new ManusBuildingException(this.GetType().Name + " was unable to parse \"" + header + "\" as a float!");
		}

		public static WaitSeconds CheatyCreate(float delay) {
			var step = new WaitSeconds();
			step.waittime = delay;
			return step;
		}
	}

	#region Interface definitions
	public interface IStep {
		void ParseContent(Manus manus, string header, string body);
		void StartStep();
		bool IsStepDone();
	}

	public interface IStepNoBody : IStep { }
	public interface IStepNoHeader : IStep { }
	public interface IStepNoParameters : IStepNoHeader, IStepNoBody { }

	public enum Code {
		MainCode,
		PreCode,
		PostCode,
		All,
	}
	#endregion

}