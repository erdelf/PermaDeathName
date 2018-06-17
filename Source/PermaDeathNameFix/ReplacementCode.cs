using RimWorld;
using System;
using System.IO;
using Verse;

namespace PermaDeathNameFix
{
    internal static class ReplacementCode
    {
        public static bool _IsValidName(string s)
        {
            bool valid = s.Length != 0 && GenText.IsValidFilename(str: s);
            if (!Current.Game.Info.permadeathMode) return valid;
            if (!valid) return false;
            foreach (FileInfo current in GenFilePaths.AllSavedGameFiles)
                if (current.Name.Substring(startIndex: 0, length: current.Name.LastIndexOf(value: '.')).Equals(value: Current.Game.Info.permadeathModeUniqueName))
                    current.MoveTo(destFileName: current.Directory?.FullName + "\\" + s + " (Permadeath)" + current.Extension);

            Current.Game.Info.permadeathModeUniqueName = s + " (Permadeath)";
            return true;
        }

        public static void _CheckUpdatePermadeathModeUniqueNameOnGameLoad(string filename)
        {
            try
            {
                if (!Current.Game.Info.permadeathMode) return;
                GameInfo info = Current.Game.Info;
                if (!Faction.OfPlayer.HasName) return;
                if (info.permadeathModeUniqueName == (Faction.OfPlayer.Name) + " (Permadeath)") return;
                Current.Game.Info.permadeathModeUniqueName = Faction.OfPlayer.Name + " (Permadeath)";
                //Log.Warning("Faction's name has changed and doesn't match save name. Fixing...");
                foreach (FileInfo current in GenFilePaths.AllSavedGameFiles)
                    if (current.Name.Substring(startIndex: 0, length: current.Name.LastIndexOf(value: '.')).Equals(value: filename))
                        current.MoveTo(destFileName: current.Directory?.FullName + "\\" + info.permadeathModeUniqueName + current.Extension);
            } catch(Exception ex)
            {
                Log.Message(text: ex.Message);
            }
        }
    }
}
