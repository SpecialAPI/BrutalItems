global using UnityEngine;
global using System.Collections;
global using System.Collections.Generic;
global using System.IO;
global using System.Reflection;
global using HarmonyLib;
global using Mono.Cecil.Cil;
global using MonoMod.Cil;

global using BrutalItems.Tools;
global using BrutalItems.Extension;
global using BrutalItems.Enums;

global using BrutalItems.Content.Items;
global using BrutalItems.Content.Components;
global using BrutalItems.Content.StatusEffects;

global using Object = UnityEngine.Object;
global using Random = UnityEngine.Random;

global using static BrutalItems.Plugin;
global using static BrutalItems.Tools.BundleLoader;
global using static BrutalItems.Tools.ItemBuilder;
global using static BrutalItems.Tools.SynegyBuilder;
global using static BrutalItems.Tools.ProjectileBuilder;
global using static BrutalItems.Tools.GlobalEnemyDatabase;
global using static BrutalItems.Tools.GlobalItemDatabase;