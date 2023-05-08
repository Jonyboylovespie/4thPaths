using System.Net;
using System.Net.Http;
using MelonLoader;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Api.ModMenu;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Main;
using PathsPlusPlus;
using BTD_Mod_Helper.UI.BTD6;
using HarmonyLib;
using Il2CppAssets.Scripts.Unity;
using Il2CppFacepunch.Steamworks;
using Il2CppNinjaKiwi.Players.LiNKAccountControllers;
using Il2CppSteamNative;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using HTMLKeyModifiers = Il2CppFacepunch.Steamworks.HTMLKeyModifiers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MelonLoader.ICSharpCode.SharpZipLib.Core;
using MelonLoader.Utils;
using Environment = Il2CppSystem.Environment;
using Exception = System.Exception;
using Task = System.Threading.Tasks.Task;
using Uri = System.Uri;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using BTD_Mod_Helper.Api.Display;
using Il2Cpp;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Audio;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Mods;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Utils;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;
using IntPtr = System.IntPtr;


[assembly: MelonInfo(typeof(Main.Main), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace Main;

public class Main : BloonsTD6Mod
{
    public bool DependenciesRequired = false;

    public override void OnApplicationLateStart()
    {
        if (ModHelper.HasMod("PathsPlusPlus"))
        {

        }
        else
        {
            MelonLogger.Msg("Missing dependencies");
            DependenciesRequired = true;
        }
    }

    public void DownloadDependency()
    {
        string adress = "https://raw.githubusercontent.com/Jonyboylovespie/paths-plus-plus/main/PathsPlusPlus.dll";
        string filename = MelonEnvironment.ModsDirectory + "/PathsPlusPlus.dll";

        WebClient client = new WebClient();
        client.DownloadFile(adress, filename);

        PopupScreen.instance.ShowOkPopup("You must restart the game for changes to take place.");
    }

    public override void OnMainMenu()
    {
        if (DependenciesRequired)
        {
            PopupScreen.instance.ShowOkPopup("You must restart the game for changes to take place.");
        }
    }

    public override void OnTitleScreen()
    {
        if (DependenciesRequired)
        {
            PopupScreen.instance.ShowEventPopup(PopupScreen.Placement.menuCenter, "Missing Dependencies",
                "You are missing dependencies required for the mod 4thPaths. Click the download button to download the mod made by Doombubbles that is required for 4thPaths to function. After downloading, restart is required.", "Download",new System.Action(DownloadDependency), "Cancel",null,
                Popup.TransitionAnim.Slide, 38);
        }
    }

    public class SuperMonkeyFourthPath : PathPlusPlus
    {
        public override string Tower => TowerType.SuperMonkey;

        public override int UpgradeCount => 5;
    }
    
    public class SuperUpgrade1 : UpgradePlusPlus<SuperMonkeyFourthPath>
    {
        public override int Cost => 4000;
        public override int Tier => 1;
        public override string Icon => "SuperIconT1";
        public override string DisplayName => "Heated Darts";
        public override string Description => "Darts become heated making them have a bit higher damage and much higher pierce. Can also hit lead and frozen bloons.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var damageModel in towerModel.GetDescendants<DamageModel>().ToArray())
            {
                damageModel.immuneBloonProperties &= ~BloonProperties.Frozen;
                damageModel.immuneBloonProperties &= ~BloonProperties.Lead;
                damageModel.damage += 1;
            }
            foreach (var projectileModel in towerModel.GetDescendants<ProjectileModel>().ToArray())
            {
                projectileModel.pierce += 4;
            }
        }
    }
    public class SuperUpgrade2 : UpgradePlusPlus<SuperMonkeyFourthPath>
    {
        public override int Cost => 3000;
        public override int Tier => 2;
        public override string Icon => "SuperIconT2";
        public override string DisplayName => "Lava-Coated Darts";
        public override string Description => "Adds molten lava to the darts that add a DOT effect to the bloons.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            foreach (var projectileModel in towerModel.GetDescendants<ProjectileModel>().ToArray())
            {
                projectileModel.collisionPasses = new int[] { -1, 0 };
                var LavaBehavior = Game.instance.model.GetTowerFromId("Alchemist").GetDescendant<AddBehaviorToBloonModel>().Duplicate();
                LavaBehavior.GetBehavior<DamageOverTimeModel>().interval = 1/3f;
                LavaBehavior.lifespan = 20;
                LavaBehavior.lifespanFrames = 1200;
                LavaBehavior.overlayType = "Fire";
                //LavaBehavior.overlayType
                projectileModel.AddBehavior(LavaBehavior);
            }
        }
    }
    public class SuperUpgrade3 : UpgradePlusPlus<SuperMonkeyFourthPath>
    {
        public override int Cost => 20000;
        public override int Tier => 3;
        public override string Icon => "SuperIconT3";
        public override string Portrait => "SuperPortraitT3";
        public override string DisplayName => "Lava Bath";
        public override string Description => "The super monkey has taken a bath in a pool of lava, making his darts hotter and his attack speed faster.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.ApplyDisplay<SuperDisplayT3>();
            foreach (var weaponModel in towerModel.GetDescendants<WeaponModel>().ToArray())
            {
                weaponModel.rate /= 2;
            }
            foreach (var projectileModel in towerModel.GetDescendants<ProjectileModel>().ToArray())
            {
                projectileModel.GetBehavior<DamageModel>().damage += 1;
                projectileModel.GetDescendant<DamageOverTimeModel>().damage = 10f;
                projectileModel.pierce = 20f;
                if (projectileModel.name == "ProjectileModel_Projectile")
                {
                    projectileModel.display = new PrefabReference() { guidRef = "125ac5d653097974b9707f641258f71b" };
                }
            }
        }
    }

    public class SuperDisplayT3 : ModDisplay
    {
        public override string BaseDisplay => GetDisplay(TowerType.SuperMonkey);
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            SetMeshTexture(node, "SuperTextureT3");
            SetMeshOutlineColor(node, new UnityEngine.Color(255f/255, 0f/255, 0f/255));
        }
    }
    public class SuperUpgrade4 : UpgradePlusPlus<SuperMonkeyFourthPath>
    {
        public override int Cost => 75000;
        public override int Tier => 4;
        public override string Icon => "SuperIconT4";
        public override string Portrait => "SuperPortraitT4";
        public override string DisplayName => "The Volcanic Master";
        public override string Description => "Over a long time of studying with the great Monks of Mount Vesuvius, the super monkey has learned the secrets of fire, lava, and molten rock.";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.ApplyDisplay<SuperDisplayT4>();
            towerModel.displayScale = 1.25f;
            towerModel.range += 10;
            foreach (var weaponModel in towerModel.GetDescendants<WeaponModel>().ToArray())
            {
                weaponModel.emission = new ArcEmissionModel("multishot", 5, 0, 120, null, false);
            }
            foreach (var projectileModel in towerModel.GetDescendants<ProjectileModel>().ToArray())
            {
                projectileModel.GetDescendant<DamageOverTimeModel>().damage = 50f;
                projectileModel.pierce = 100f;
                if (projectileModel.name == "ProjectileModel_Projectile")
                {
                    projectileModel.display = new PrefabReference() { guidRef = GetDisplayGUID<SuperT4Proj>() };
                }
                projectileModel.AddBehavior(new TrackTargetWithinTimeModel("aimbot", 999999f, true, false, 144f, false, 99999999f, false, 3.47999978f, true));
            }
            foreach (var attackModel in towerModel.GetBehaviors<AttackModel>().ToArray())
            {
                attackModel.range += 10;
            }
        }
    }
    public class SuperDisplayT4 : ModDisplay
    {
        public override string BaseDisplay => GetDisplay(TowerType.NinjaMonkey,5,0,0);
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.genericRenderers)
            {
                renderer.material.mainTexture = GetTexture("SuperTextureT4");
            }
        }
    }
    public class SuperT4Proj : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;
        public override float Scale => 0.5f;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "SuperT4Proj");
        }
    }
    public class SuperUpgrade5 : UpgradePlusPlus<SuperMonkeyFourthPath>
    {
        public override int Cost => 1000000;
        public override int Tier => 5;
        public override string Icon => "SuperIconT5";
        public override string Portrait => "SuperPortraitT5";
        public override string DisplayName => "THE OVERLORD OF MOUNT VESUVIUS";
        public override string Description => "Kneel before him...";

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.range += 20;
            towerModel.ApplyDisplay<SuperDisplayT5>();
            towerModel.displayScale = 2;
            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("TackShooter-500").GetBehaviors<AttackModel>().Last().Duplicate());
            towerModel.GetBehaviors<AttackModel>().Last().weapons[0].emission = new ArcEmissionModel("FireBall", 10, 0, 360, null, false);
            towerModel.GetBehaviors<AttackModel>().Last().weapons[0].rate = 3;
            foreach (var weaponModel in towerModel.GetDescendants<WeaponModel>().ToArray())
            {
                weaponModel.emission = new ArcEmissionModel("multishot", 8, 0, 120, null, false);
            }
            foreach (var projectileModel in towerModel.GetDescendants<ProjectileModel>().ToArray())
            {
                projectileModel.GetBehavior<DamageModel>().damage *= 10;
                projectileModel.GetDescendant<DamageOverTimeModel>().damage = 1000f;
                projectileModel.pierce = 99999999999f;
            }
            foreach (var attackModel in towerModel.GetBehaviors<AttackModel>().ToArray())
            {
                attackModel.range += 20;
            }
        }
    }
    public class SuperDisplayT5 : ModDisplay
    {
        public override string BaseDisplay => "d84a1337513e01747ab7cb5152f466fa";
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.genericRenderers)
            {
                renderer.material.mainTexture = GetTexture("SuperTextureT5");
            }
        }
    }
}  