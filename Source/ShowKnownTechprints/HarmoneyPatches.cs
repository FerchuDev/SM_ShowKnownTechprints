using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse.Sound;
using LudeonTK;

namespace ShowKnownTechprints
{
	[StaticConstructorOnStartup]
    static public class HarmonyPatches
    {
        public static Harmony harmonyInstance;


        static HarmonyPatches()
        {
            harmonyInstance = new Harmony("rimworld.rwmods.ShowKnownTechprints");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

    }


    [HarmonyPatch(typeof(TransferableUIUtility))]
    [HarmonyPatch("DoExtraIcons")]
    internal static class DoExtraIconsTweak
	{
		private static readonly Texture2D TechprintIcon_Missing = ContentFinder<Texture2D>.Get("UI/Icons/TechprintIcon_Missing");
		private static readonly Texture2D TechprintIcon_Part = ContentFinder<Texture2D>.Get("UI/Icons/TechprintIcon_Part");
		private static readonly Texture2D TechprintIcon_Complete = ContentFinder<Texture2D>.Get("UI/Icons/TechprintIcon_Complete");
		[TweakValue("Interface", 0.0f, 50f)]
		private static float TechprintIconWidth = 24f;

		public static void Postfix(Transferable trad, Rect rect, ref float curX)
		{
			if (trad.AnyThing.TryGetComp<CompTechprint>() != null)
			{
				DoExtraIconsTweak.DrawTechprintIcon(trad.AnyThing, new Rect(curX - DoExtraIconsTweak.TechprintIconWidth, (float)(((double)rect.height - (double)DoExtraIconsTweak.TechprintIconWidth) / 2.0), DoExtraIconsTweak.TechprintIconWidth, DoExtraIconsTweak.TechprintIconWidth));
				curX -= DoExtraIconsTweak.TechprintIconWidth;
			}
        }


		private static void DrawTechprintIcon(Thing TechprintToDisplay, Rect rect)
		{
			CompTechprint TechThing = TechprintToDisplay.TryGetComp<CompTechprint>();

			int TechprintReq = TechThing.Props.project.TechprintCount;
			int TechprintApp = TechThing.Props.project.TechprintsApplied;

			bool TechprintNotNeeded = false;

			if (TechprintApp >= TechprintReq)
				TechprintNotNeeded = true;

			if (TechprintNotNeeded == true)
				GUI.DrawTexture(rect, (Texture)DoExtraIconsTweak.TechprintIcon_Complete);
			else if(TechprintApp > 0)
				GUI.DrawTexture(rect, (Texture)DoExtraIconsTweak.TechprintIcon_Part);
			else
				GUI.DrawTexture(rect, (Texture)DoExtraIconsTweak.TechprintIcon_Missing);

			if (!Mouse.IsOver(rect))
				return;


			Widgets.DrawHighlight(rect);
			//			string TooltipText = TechprintApp.ToString() + " Techprints of " + TechprintReq.ToString() + " applied for project " + TechThing.Props.project.UnlockedDefs.Select<Def, string>((Func<Def, string>)(x => x.label)).ToCommaList().CapitalizeFirst();
			string TooltipText = "ShowKnownTechprints.Tooltip".Translate(TechprintApp.ToString(), TechprintReq.ToString(), TechThing.Props.project.UnlockedDefs.Select<Def, string>((Func<Def, string>)(x => x.label)).ToCommaList().CapitalizeFirst());
			TooltipHandler.TipRegion(rect, (TipSignal)TooltipText);

		}
	}

}
