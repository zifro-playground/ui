using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PM.Manus {
	
	public static class Loader {

		public const string resourceName = "levelmanus_{0}";
		public static readonly string[] linebreaks = new string[] { "\n\r", "\r\n", "\n", "\r" };
		public static ReadOnlyCollection<Manus> allManuses { get; private set; }

		private static Step.IStep GetAppropietStep(string name) {
			switch (name.ToLower()) {
				case "set-speed":
				case "speed":
					return new Step.SetSpeed();

				case "bubble-settings":
				case "settings":
				case "bubblesettings":
				case "popup-settings":
				case "popupsettings":
					return new Step.SetPopupSettings();

				case "bubble":
				case "popup":
					return new Step.Popup();

				case "kör":
				case "steg":
				case "run":
				case "step":
					return new Step.WalkSteps();

				case "stopp":
				case "stop":
					return new Step.StopCode();

				case "code":
				case "maincode":
				case "main-code":
				case "set-code":
				case "set-main-code":
					return new Step.SetCode(Step.Code.MainCode);

				case "precode":
				case "pre-code":
				case "set-pre-code":
					return new Step.SetCode(Step.Code.PreCode);

				case "postcode":
				case "post-code":
				case "set-post-code":
					return new Step.SetCode(Step.Code.PostCode);

				case "clearcode":
				case "clear-code":
				case "clearallcode":
				case "clear-all-code":
					return new Step.ClearCode(Step.Code.All);

				case "clearmaincode":
				case "clear-main-code":
					return new Step.ClearCode(Step.Code.MainCode);

				case "clearprecode":
				case "clear-pre-code":
					return new Step.ClearCode(Step.Code.PreCode);

				case "clearpostcode":
				case "clear-post-code":
					return new Step.ClearCode(Step.Code.PostCode);

				case "wait":
				case "sleep":
				case "vänta":
					return new Step.WaitSeconds();

				case "finish":
				case "kör-klart":
					return new Step.WalkTilFinished();

				default:
					throw new ManusBuildingException("Invalid step type \"" + name + "\".");
			}
		}
		
		public static void BuildAll() {
			List<Manus> list = new List<Manus>();
			for (int i=0; i<PMWrapper.numOfLevels; i++) {
				list.Add(BuildFromPath(string.Format(resourceName, i)));
			}
			
			allManuses = new ReadOnlyCollection<Manus>(list);
		}

		public static Manus BuildFromPath(string path) {
			TextAsset asset = Resources.Load<TextAsset>(path);
			if (asset == null) return null;

			try {
				return BuildFromString("Assets/Resources/" + asset.name, asset.text);
			} catch (Exception err) {
				if (err is ManusBuildingException)
					Debug.LogException(err);
				return null;
			}
		}

		private static Manus BuildFromString(string filename, string manusString) {
			List<string> splitManus = new List<string>(manusString.Split(linebreaks, StringSplitOptions.None));
			Manus manus = new Manus();

			Step.IStep step = null;
			string contentHeader = null;
			string contentBody = null;
			string row;
			int rowNumber = 0;

			while ((row = DequeueRow(splitManus)) != null) {
				rowNumber++;
				row = row.Trim();

				if (step != null) {
					// Try continue content reading
					if (row.Length > 0 && row[0] == '>') {
						if (step is Step.IStepNoBody) throw new ManusBuildingException(step.GetType().Name + " doesn't support body parameters!", rowNumber, filename);

						if (contentBody == null) 
							contentBody = row.Substring(1);
						else 
							contentBody += "\n" + row.Substring(1);
					} else {
						// Reading complete
						if (contentBody == null && !(step is Step.IStepNoBody)) throw new ManusBuildingException(step.GetType().Name + " requires a body parameter!", rowNumber, filename);

						try {
							step.ParseContent(manus, contentHeader, contentBody);
						} catch (ManusBuildingException inner) {
							throw new ManusBuildingException(rowNumber, filename, inner);
						}
						manus.allSteps.Add(step);
						step = null;
					}
				}

				// Comments
				if (row.StartsWith("//") || row.StartsWith("#")) continue;

				// Empty rows
				if (row.Length == 0) continue;

				if (step == null) {
					if (row[0] == '>') throw new ManusBuildingException("Unexpected body parameter.", rowNumber, filename);

					// Listen for step start
					Match match = Regex.Match(row, @"^([_\-\wåäöÅÄÖ.,]+):(?:\s*(.+)\s*)?$");

					if (!match.Success) {
						match = Regex.Match(row, @"^([_\-\wåäöÅÄÖ.,]+)\s*$");
						if (!match.Success)
							throw new ManusBuildingException("Expected step, got \"" + row + "\"", rowNumber, filename);
					}

					string g1 = match.Groups[1].Value;
					string g2 = (match.Groups.Count > 2 && match.Groups[2].Success) ? match.Groups[2].Value : null;

					try {
						step = GetAppropietStep(g1);
					} catch (ManusBuildingException inner) {
						throw new ManusBuildingException(rowNumber, filename, inner);
					}

					if (g2 != null && step is Step.IStepNoHeader) throw new ManusBuildingException(step.GetType().Name + " doesn't support header parameters!", rowNumber, filename);
					if (g2 == null && !(step is Step.IStepNoHeader)) throw new ManusBuildingException(step.GetType().Name + " requires a header parameter!", rowNumber, filename);
					if (match.Groups.Count != 3 && !(step is Step.IStepNoParameters)) throw new ManusBuildingException(step.GetType().Name + " requires a body parameter!", rowNumber, filename);

					contentHeader = g2;
					contentBody = null;

					if (step is Step.IStepNoBody) {
						// Reading complete
						try {
							step.ParseContent(manus, contentHeader, contentBody);
						} catch (ManusBuildingException inner) {
							throw new ManusBuildingException(rowNumber, filename, inner);
						}
						manus.allSteps.Add(step);
						step = null;
					}
				}
			}

			if (step != null) {
				// Reading complete
				if (contentBody == null && !(step is Step.IStepNoBody)) throw new ManusBuildingException(step.GetType().Name + " requires a body parameter!", rowNumber, filename);

				try {
					step.ParseContent(manus, contentHeader, contentBody);
				} catch (ManusBuildingException inner) {
					throw new ManusBuildingException(rowNumber, filename, inner);
				}
				manus.allSteps.Add(step);
				step = null;
			}

			return manus;
		}

		private static string DequeueRow(List<string> manusList) {
			if (manusList.Count == 0) return null;
			string row = manusList[0];
			manusList.RemoveAt(0);
			return row;
		}


	}

}