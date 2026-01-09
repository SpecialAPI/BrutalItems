using BepInEx;
using BrutalItems.Content.Items;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BrutalItems
{
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string MOD_GUID = "spapi.etg.brutalitems";
        public const string MOD_NAME = "Brutal Items";
        public const string MOD_VERSION = "1.0.1";
        public const string MOD_PREFIX = "spapi";

        public static Harmony HarmonyInstance;

        public void Awake()
        {
            HarmonyInstance = new Harmony(MOD_GUID);
            HarmonyInstance.PatchAll();

            AutoEnumExtension.AutoExtendEnums();

            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager gm)
        {
            var stopwatch = Stopwatch.StartNew();

            LoadBundle("brutalitems");

            // Version 1.0.0
            DumDum.Init();
            Trepanation.Init();
            PrussianBlue.Init();
            DefectiveRounds.Init();
            PharmaceuticalRollercoaster.Init();
            DewormingPills.Init();
            BarelyUsedGauze.Init();
            ModernMedicine.Init();
            Surrender.Init();
            EsotericArtifact.Init();
            ChainOfCommand.Init();
            BoxOfMedals.Init();
            SoggyBandages.Init();
            CertificateOfExemption.Init();
            HeadOfScribe.Init();
            ShardOfNowak.Init();

            // Synergies
            Synergies.Init();

            stopwatch.Stop();
            ETGModConsole.Log($"Successfully initialized Brutal Items in {stopwatch.Elapsed.TotalSeconds} seconds.");
        }
    }
}
