﻿using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using ModelReplacement;
using BepInEx.Configuration;
using CackleCrew.ThisIsMagical;
using BepInEx.Logging;

//using System.Numerics;

namespace CreatureModelReplacement
{


    [BepInPlugin("CreatureReplacement", "Cackle Crew", "3.0.0")] //Name of Config / Name of Mod / Version number
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]


    public class Plugin : BaseUnityPlugin
    {
        internal new static ManualLogSource logger;
        public static ConfigFile config;

        // Universal config options  
        public static ConfigEntry<bool> enableModelForAllSuits { get; private set; }
        public static ConfigEntry<bool> enableModelAsDefault { get; private set; }
        public static ConfigEntry<string> suitNamesToEnableModel { get; private set; }

        private void Awake()
        {
            logger = base.Logger; 
            config = base.Config;
            enableModelForAllSuits = config.Bind<bool>("Suits to Replace Settings", "Enable Model for all Suits", false, "Enable to replace every suit with Model. Set to false to specify suits");
            enableModelAsDefault = config.Bind<bool>("Suits to Replace Settings", "Enable Model as default", false, "Enable to replace every suit that hasn't been otherwise registered with Model.");
            suitNamesToEnableModel = config.Bind<string>("Suits to Replace Settings", "Suits to enable Model for", "Default,Orange suit,Green suit,Pajama suit,Hazard suit,Purple Suit", "For use with Moresuits, replace list with: CARed,CAGreen,CAHaz,CAPajam,CAPurp");
            Assets.PopulateAssets();

            // Plugin startup logic

            if (enableModelForAllSuits.Value)
            {
                ModelReplacementAPI.RegisterModelReplacementOverride(typeof(BodyReplacement));
            }

            if (enableModelAsDefault.Value)
            {
                ModelReplacementAPI.RegisterModelReplacementDefault(typeof(BodyReplacement));
            }

            var commaSepList = suitNamesToEnableModel.Value.Split(',');
            foreach (var item in commaSepList)
            {
                ModelReplacementAPI.RegisterSuitModelReplacement(item, typeof(BodyReplacement));
            }


            Harmony harmony = new Harmony("LeCreature");
            harmony.PatchAll();
            //Setup Customization...!
            Options.Init();
            Logger.LogInfo($"Plugin {"CreatureReplacement"} is loaded!");
        }
    }
    public static class Assets
    {
        // Replace lecreature with the Asset Bundle Name from your unity project 
        public static string mainAssetBundleName = "lecreature";
        public static string customizationAssetBundleName = "lecustomization";
        public static AssetBundle MainAssetBundle = null;
        public static AssetBundle CustomizationAssetBundle = null;

        private static string GetAssemblyName() => Assembly.GetExecutingAssembly().FullName.Split(',')[0];
        public static void PopulateAssets()
        {
            if (MainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + "." + mainAssetBundleName))
                {
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }

            }
            if (CustomizationAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + "." + customizationAssetBundleName))
                {
                    CustomizationAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }

            }
        }
    }

}