using System;
using System.IO;
using UnityEngine;

namespace PM
{
	public class GameToMigration : MonoBehaviour
	{
		public static Version Version;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.A))
				CreateMigration();
		}

		public void CreateMigration()
		{
			var game = Main.Instance.GameDefinition;

			Version = new Version(1, 0, 1);
			var path = "C:/Users/Tifos/Documents/GitHub/Zifro/App_Code/Persistance/Migrations/GameUpgrades/TargetVersion_" + Version.PrintWithUnderscore() + ".cs";

			using (var tw = new StreamWriter(path))
			{
				tw.WriteLine("using System.Collections.Generic;");
				tw.WriteLine("using System.Linq;");
				tw.WriteLine("using Zifro.Models.Playground.Database;");
				tw.WriteLine("using Umbraco.Core.Logging;");
				tw.WriteLine("using Umbraco.Core.Persistence.Migrations;");
				tw.WriteLine("using Umbraco.Core.Persistence.SqlSyntax;");
				tw.WriteLine("using Zifro.Code;\n");

				tw.WriteLine("namespace Zifro.Persistance.Migrations.GameUpgrades.TargetVersion_" + Version.PrintWithUnderscore());
				tw.WriteLine("{");
				tw.WriteLine("	[Migration(\"" + Version.PrintWithDots() + "\", 1, Constants.Application.GameMigrationName)]");
				tw.WriteLine("	public class UpdatePlaygroundGameData : MigrationBase");
				tw.WriteLine("	{");
				tw.WriteLine("		public UpdatePlaygroundGameData(ISqlSyntaxProvider sqlSyntax, ILogger logger)");
				tw.WriteLine("			: base(sqlSyntax, logger)");
				tw.WriteLine("		{}\n");

				tw.WriteLine("		public override void Up()");
				tw.WriteLine("		{");
				tw.WriteLine("			using (var dbContext = new PlaygroundDbContext())");
				tw.WriteLine("			{");
				tw.WriteLine("				var game = dbContext.PlaygroundGame.Find(\"" + game.gameId + "\");\n");

				tw.WriteLine("				if (game == null)");
				tw.WriteLine("					game = dbContext.PlaygroundGame.Add(new PlaygroundGame() {GameId = \"" + game.gameId + "\"});\n");

				tw.WriteLine("				var levelsInDatabase = dbContext.PlaygroundLevel.Where(x => x.GameId == game.GameId);\n");

				tw.WriteLine("				var levelsToUpdate = new List<string>");
				tw.WriteLine("				{");
				foreach (var level in game.activeLevels)
				{
					tw.WriteLine("					\"" + level.levelId + "\",");
				}
				tw.WriteLine("				};\n");

				tw.WriteLine("				foreach (var levelToUpdate in levelsToUpdate)");
				tw.WriteLine("				{");
				tw.WriteLine("					if (!levelsInDatabase.Any(x => x.LevelId == levelToUpdate))");
				tw.WriteLine("						dbContext.PlaygroundLevel.Add(new PlaygroundLevel() {LevelId = levelToUpdate, GameId = game.GameId, PlaygroundGame = game});");
				tw.WriteLine("				}\n");

				tw.WriteLine("				dbContext.SaveChanges();");
				tw.WriteLine("			}");
				tw.WriteLine("		}\n");

				tw.WriteLine("		public override void Down()");
				tw.WriteLine("		{}");
				tw.WriteLine("	}");
				tw.WriteLine("}");
			}
		}
	}

	public struct Version
	{
		public int Major, Minor, Build;

		public Version(int major, int minor, int build)
		{
			Major = major;
			Minor = minor;
			Build = build;
		}

		public string PrintWithDots()
		{
			return String.Format("{0}.{1}.{2}", Major, Minor, Build);
		}

		public string PrintWithUnderscore()
		{
			return String.Format("{0}_{1}_{2}", Major, Minor, Build);
		}
	}
}
