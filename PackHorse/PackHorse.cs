using System.Reflection;
using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using SkillManager;

namespace PackHorse;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInIncompatibility("org.bepinex.plugins.valheim_plus")]
public class PackHorse : BaseUnityPlugin
{
	private const string ModName = "PackHorse";
	private const string ModVersion = "1.0.3";
	private const string ModGUID = "org.bepinex.plugins.packhorse";

	public void Awake()
	{
		Skill packhorse = new("PackHorse", "packhorse-icon.png");
		packhorse.Name.English("Pack horse");
		packhorse.Description.English("Increases the maximum carry weight.");
		packhorse.Name.German("Packesel");
		packhorse.Description.German("Erhöht das maximale Tragegewicht.");
  		packhorse.Name.Chinese("驮马");
		packhorse.Description.Chinese("增加最大负重");
		packhorse.Configurable = true;

		Assembly assembly = Assembly.GetExecutingAssembly();
		Harmony harmony = new(ModGUID);
		harmony.PatchAll(assembly);
	}

	[HarmonyPatch(typeof(Character), nameof(Character.UpdateWalking))]
	private class AddSkillGain
	{
		private static float skillGainCounter = 0;

		[UsedImplicitly]
		private static void Postfix(Character __instance, float dt)
		{
			if (__instance is Player player && player == Player.m_localPlayer)
			{
				if (player.m_currentVel.magnitude > 0.1 && player.GetMaxCarryWeight() * 0.9 <= player.m_inventory.m_totalWeight)
				{
					skillGainCounter += dt;
					if (skillGainCounter > 1)
					{
						skillGainCounter = 0;
						Player.m_localPlayer.RaiseSkill("PackHorse");
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(Player), nameof(Player.GetMaxCarryWeight))]
	private class IncreaseCarryWeight
	{
		[UsedImplicitly]
		private static void Postfix(Player __instance, ref float __result)
		{
			__result *= 1 + __instance.GetSkillFactor("PackHorse");
		}
	}
}
