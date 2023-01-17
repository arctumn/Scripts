//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Legion/Revenant/CoreLR.cs
//cs_include Scripts/Legion/CoreLegion.cs
//cs_include Scripts/Legion/InfiniteLegionDarkCaster.cs
//cs_include Scripts/Story/Legion/SeraphicWar.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;
using System.Linq;

public class ArmyLR
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    public CoreFarms Farm = new();
    public CoreAdvanced Adv = new();
    private CoreArmyLite Army = new();
    public CoreLegion Legion = new CoreLegion();
    public CoreLR CoreLR = new CoreLR();
    public InfiniteLegionDC ILDC = new InfiniteLegionDC();
    public SeraphicWar_Story Seraph = new SeraphicWar_Story();
    private static CoreBots sCore = new();
    private static CoreArmyLite sArmy = new();
    public string OptionsStorage = "ArmyLR";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new List<IOption>()
    {
        new Option<int>("armysize","Players", "Input the minimum of players to wait for", 1),
        sArmy.player1,
        sArmy.player2,
        sArmy.player3,
        sArmy.player4,
        sArmy.player5,
        sArmy.player6,
        sArmy.packetDelay,
        CoreBots.Instance.SkipOptions
    };

    public string[] LRMaterials =
    {
        "Exalted Crown",
        "Revenant's Spellscroll",
        "Conquest Wreath",
        "Legion Revenant"
    };

    public string[] LF1 =
    {
        "Aeacus Empowered",
        "Tethered Soul",
        "Darkened Essence",
        "Dracolich Contract"
    };

    public string[] LF2 =
    {
        "Grim Cohort Conquered",
        "Ancient Cohort Conquered",
        "Pirate Cohort Conquered",
        "Battleon Cohort Conquered",
        "Mirror Cohort Conquered",
        "Darkblood Cohort Conquered",
        "Vampire Cohort Conquered",
        "Spirit Cohort Conquered",
        "Dragon Cohort Conquered",
        "Doomwood Cohort Conquered",
    };

    public string[] LF3 =
    {
        "Hooded Legion Cowl",
        "Legion Token",
        "Dage's Favor",
        "Emblem of Dage",
        "Diamond Token of Dage",
        "Dark Token"
    };

    public string[] legionMedals =
    {
        "Legion Round 1 Medal",
        "Legion Round 2 Medal",
        "Legion Round 3 Medal",
        "Legion Round 4 Medal"
    };

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Core.BankingBlackList.AddRange(LRMaterials.Concat(LF1).Concat(LF2).Concat(LF3).Concat(legionMedals));

        LR();

        Core.SetOptions(false);
    }

    public void LR()
    {
        Legion.JoinLegion();
        Legion.LegionRound4Medal();
        Seraph.SeraphicWar_Questline();
        /*
        ********************************************************************************
        ********************************PREFARM ZONE************************************
        ********************************************************************************
        */
        /*Step 1: Evil Rank 10*/
        ArmyEvilGoodRepMax();
        /*Step 2: Hooded Legion Cowl funds and some change for enhancement costs*/
        ArmyGoldFarm(5500000);
        /*Step 3: 3000 Dage Favor*/
        ArmyDageFavor();
        /*Step 4: 10 Emblem of Dage*/
        ArmyEmblemOfDage(10);
        /*Step 5: 300 Diamond Token of Dage*/
        ArmyDiamondTokenOfDage();
        /*Step 6: 600 Dark Token*/
        ArmyDarkTokenOfDage();
        /*
        ********************************************************************************
        **********************************FINISH****************************************
        ********************************************************************************
        */
        /*Step 7: LF1*/
        ArmyLF1();
        /*Step 9: LF2, thx tato :TatoGasm:*/
        ArmyFL2();
        /*Step 10: LF3 and Finish*/
        ArmyLF3();
        CoreLR.GetLR(true);
    }

    public void ArmyLF1(int quant = 20)
    {
        if (Core.CheckInventory("Revenant's Spellscroll", quant))
            return;

        bool hasDarkCaster = false;
        if (Core.CheckInventory(new[] { "Love Caster", "Legion Revenant" }, any: true))
            hasDarkCaster = true;
        else
        {
            List<InventoryItem> InventoryData = Bot.Inventory.Items;
            foreach (InventoryItem Item in InventoryData)
            {
                if (Item.Name.Contains("Dark Caster") && Item.Category == ItemCategory.Class)
                {
                    hasDarkCaster = true;
                    break;
                }
            }
            if (!hasDarkCaster)
            {
                List<InventoryItem> BankData = Bot.Bank.Items;
                foreach (InventoryItem Item in BankData)
                {
                    if (Item.Name.Contains("Dark Caster") && Item.Category == ItemCategory.Class)
                    {
                        hasDarkCaster = true;
                        Core.Unbank(Item.Name);
                        break;
                    }
                }
            }
        }
        if (!hasDarkCaster)
            ILDC.GetILDC(false);

        Core.AddDrop("Legion Token");
        Core.AddDrop(LRMaterials);
        Core.AddDrop(LF1);

        Core.FarmingLogger("Revenant's Spellscroll", quant);
        Bot.Quests.UpdateQuest(2060);
        Core.RegisterQuests(6897);
        int i = 1;
        while (!Bot.ShouldExit && !Core.CheckInventory("Revenant's Spellscroll", quant))
        {
            Adv.BestGear(GearBoost.Undead);
            /*Sells non-full stacks to keep in sync for each LF1 quest item*/
            if (!Core.CheckInventory("Aeacus Empowered", 50))
                Core.SellItem("Aeacus Empowered", 0, true);
            ArmyHunt("judgement", new[] { "Ultra Aeacus" }, "Aeacus Empowered", ClassType.Solo, false, 50);

            Adv.BestGear(GearBoost.dmgAll);
            if (!Core.CheckInventory("Tethered Soul", 300))
                Core.SellItem("Tethered Soul", 0, true);
            Core.KillMonster("revenant-999999", "r2", "Left", "*", "Tethered Soul", 300, false); //Temp fix for players > 3
            //ArmyHunt("revenant",  new[] {"Forgotten Soul"}, "Tethered Soul", false, 300);

            if (!Core.CheckInventory("Darkened Essence", 500))
                Core.SellItem("Darkened Essence", 0, true);
            ArmyHunt("shadowrealmpast", new[] { "Pure Shadowscythe, Shadow Guardian, Shadow Warrior" }, "Darkened Essence", ClassType.Farm, false, 500);

            Bot.Quests.UpdateQuest(2061);
            Adv.BestGear(GearBoost.Undead);
            if (!Core.CheckInventory("Dracolich Contract", 1000))
                Core.SellItem("Dracolich Contract", 0, true);
            ArmyHunt("necrodungeon", new[] { "5 Headed Dracolich" }, "Dracolich Contract", ClassType.Farm, false, 1000);

            Bot.Wait.ForPickup("Revenant's Spellscroll");
            Core.Logger($"Completed x{i++}");
        }
        Core.CancelRegisteredQuests();
    }

    public void ArmyFL2(int quant = 6)
    {
        if (Core.CheckInventory("Conquest Wreath", quant))
            return;

        Core.AddDrop(LF2);

        Core.RegisterQuests(6898);
        Adv.BestGear(GearBoost.Undead);
        Core.FarmingLogger("Conquest Wreath", quant);
        int i = 1;

        while (!Bot.ShouldExit && !Core.CheckInventory("Conquest Wreath", quant))
        {
            if (!Core.CheckInventory("Grim Cohort Conquered", 500))
                Core.SellItem("Grim Cohort Conquered", 0, true);
            ArmyHunt("doomvault", new[] { "Grim Soldier" }, "Grim Cohort Conquered", ClassType.Farm, false, 500);

            if (!Core.CheckInventory("Ancient Cohort Conquered", 500))
                Core.SellItem("Ancient Cohort Conquered", 0, true);
            ArmyHunt("mummies", new[] { "Mummy" }, "Ancient Cohort Conquered", ClassType.Farm, false, 500);

            if (!Core.CheckInventory("Pirate Cohort Conquered", 500))
                Core.SellItem("Pirate Cohort Conquered", 0, true);
            ArmyHunt("wrath", new[] { "Undead Pirate", "Mutineer", "Dark Fire", "Fishbones" }, "Pirate Cohort Conquered", ClassType.Farm, false, 500);

            if (!Core.CheckInventory("Battleon Cohort Conquered", 500))
                Core.SellItem("Battleon Cohort Conquered", 0, true);
            ArmyHunt("doomwar", new[] { "Zombie", "Zombie Knight" }, "Battleon Cohort Conquered", ClassType.Farm, false, 500);

            if (!Core.CheckInventory("Mirror Cohort Conquered", 500))
                Core.SellItem("Mirror Cohort Conquered", 0, true);
            ArmyHunt("overworld", new[] { "Undead Minion", "Undead Mage", "Undead Bruiser" }, "Mirror Cohort Conquered", ClassType.Farm, false, 500);

            if (!Core.CheckInventory("Darkblood Cohort Conquered", 500))
                Core.SellItem("Darkblood Cohort Conquered", 0, true);
            ArmyHunt("deathpits", new[] { "Ghastly Darkblood", "Rotting Darkblood", "Sundered Darkblood" }, "Darkblood Cohort Conquered", ClassType.Farm, false, 500);

            if (!Core.CheckInventory("Vampire Cohort Conquered", 500))
                Core.SellItem("Vampire Cohort Conquered", 0, true);
            ArmyHunt("maxius", new[] { "Ghoul Minion", "Vampire Minion" }, "Vampire Cohort Conquered", ClassType.Farm, false, 500);

            if (!Core.CheckInventory("Dracolich Contract", 500))
                Core.SellItem("Spirit Cohort Conquered", 0, true);
            ArmyHunt("curseshore", new[] { "Escaped Ghostly Zardman", "Escaped Wendighost", "Escaped Dai Tenghost" }, "Spirit Cohort Conquered", ClassType.Farm, false, 500);

            if (!Core.CheckInventory("Dragon Cohort Conquered", 500))
                Core.SellItem("Dragon Cohort Conquered", 0, true);
            ArmyHunt("dragonbone", new[] { "Bone Dragonling", "Dark Fire", }, "Dragon Cohort Conquered", ClassType.Farm, false, 500);

            if (!Core.CheckInventory("Doomwood Cohort Conquered", 500))
                Core.SellItem("Doomwood Cohort Conquered", 0, true);
            ArmyHunt("doomwood", new[] { "Doomwood Soldier", "Doomwood Bonemuncher", "Doomwood Ectomancer", "Undead Paladin", "Doomwood Treeant" }, "Doomwood Cohort Conquered", ClassType.Farm, false, 500);
            Core.EnsureComplete(6898);
            Bot.Wait.ForPickup("Conquest Wreath");
            Core.Logger($"Completed x{i++}");
        }
    }

    public void ArmyLF3(int quant = 10)
    {
        Core.FarmingLogger("Exalted Crown", quant);
        Core.RegisterQuests(6899);
        Core.AddDrop(LF3);
        while (!Bot.ShouldExit && !Core.CheckInventory("Exalted Crown", quant))
        {
            Core.BuyItem("underworld", 216, "Hooded Legion Cowl");
            /*This is the only not prefarmed item left to get*/
            ArmyDarkTokenOfDage(100);
            if (!Core.CheckInventory("Legion Token", 4000))
                ArmyLTs(4000);
            Bot.Wait.ForPickup("Exalted Crown");
        }
        Core.CancelRegisteredQuests();
    }

    public void ArmyEvilGoodRepMax(int rank = 10)
    {
        ArmyEvilGoodRank4();
        ArmyEvilGoodRankMax();
    }

    public void ArmyEvilGoodRank4()
    {
        if (Farm.FactionRank("Good") >= 4 && Farm.FactionRank("Evil") >= 4)
            return;

        Core.RegisterQuests(364, 369); //Youthanize 364, That Hero Who Chases Slimes 369
        Farm.ToggleBoost(BoostType.Reputation);
        while (!Bot.ShouldExit && (Farm.FactionRank("Good") < 4 && Farm.FactionRank("Evil") < 4))
            ArmyHunt("swordhavenbridge", new[] { "Slime" }, "Slime in a Jar", ClassType.Farm, true, 6);
        Core.CancelRegisteredQuests();
        Farm.ToggleBoost(BoostType.Reputation, false);
    }

    public void ArmyEvilGoodRankMax()
    {
        if (Farm.FactionRank("Good") >= 10 && Farm.FactionRank("Evil") >= 10)
            return;

        Core.RegisterQuests(367, 372); //Bone-afide 367, Tomb with a View 372
                                       //ArmyHunt("castleundead", new[] {"Skeletal Viking", "Skeletal Warrior"}, 372, new[] {"Chaorrupted Skull"}, true, 5);
        Farm.ToggleBoost(BoostType.Reputation);
        while (!Bot.ShouldExit && (Farm.FactionRank("Good") < 10 && Farm.FactionRank("Evil") < 10))
            ArmyHunt("castleundead", new[] { "Skeletal Viking", "Skeletal Warrior" }, "Replacement Tibia", ClassType.Farm, true, 6);
        Core.CancelRegisteredQuests();
        Farm.ToggleBoost(BoostType.Reputation, false);
    }

    public void ArmyGoldFarm(int quant = 100000000)
    {
        if (Bot.Player.Gold >= quant)
            return;

        Farm.ToggleBoost(BoostType.Gold);
        Core.RegisterQuests(3991, 3992);
        while (!Bot.ShouldExit && Bot.Player.Gold < quant)
            ArmyHunt("battlegrounde", new[] { "Living Ice", "Ice Lord", "Ice Demon", "Glacial Horror", "Icy Dragon", "Permafrost Pummeler", "Icy Banshee", "Frozen Deserter" }, "Battleground E Opponent Defeated", ClassType.Farm, true, 10);
        /*Farms 500k extra just to be safe with enhancement costs*/
        Farm.ToggleBoost(BoostType.Gold, false);
        Core.CancelRegisteredQuests();
    }

    public void ArmyDageFavor(int quant = 3000)
    {
        if (!Core.CheckInventory("Dage's Favor", quant))
            Core.SellItem("Dage's Favor", 0, true); //Cannot survive soloing these monsters without the full army
        else return;
        ArmyHunt("evilwarnul", new[] { "Skeletal Warrior", "Skull Warrior" }, "Dage's Favor", ClassType.Farm, false, quant);
    }

    public void ArmyEmblemOfDage(int quant = 500)
    {
        if (!Core.CheckInventory("Emblem of Dage", quant))
            Core.SellItem("Emblem of Dage", 0, true); //Cannot survive soloing these monsters without the full army
        else return;
        Core.FarmingLogger("Emblem of Dage", quant);
        Core.AddDrop("Emblem of Dage", "Legion Seal", "Gem of Mastery");
        Adv.BestGear(GearBoost.gold);
        Core.RegisterQuests(4742);
        while (!Bot.ShouldExit && !Core.CheckInventory("Emblem of Dage", quant))
        {
            ArmyHunt("shadowblast", new[] { "Carnage" }, "Legion Seal", ClassType.Farm, false, 25);
            ArmyHunt("shadowblast", new[] { "Shadowrise Guard" }, "Gem of Mastery", ClassType.Farm, false, 1);
        }
        Core.CancelRegisteredQuests();
    }

    public void ArmyDiamondTokenOfDage(int quant = 300)
    {
        if (!Core.CheckInventory("Diamond Token of Dage", quant))
            Core.SellItem("Diamond Token of Dage", 0, true); //Cannot survive soloing these monsters without the full army
        else return;
        if (!Core.CheckInventory("Legion Token", 50))
            ArmyLTs(50);
        /*Sell any existing Defeated Makai to sync up army before farming bosses, log in SellItem*/
        if (Core.CheckInventory("Defeated Makai"))
            Core.SellItem("Defeated Makai", 0, true);
        Core.FarmingLogger("Diamond Token of Dage", quant);
        Core.AddDrop("Diamond Token of Dage", "Legion Token");
        Core.RegisterQuests(4743);
        while (!Bot.ShouldExit && !Core.CheckInventory("Diamond Token of Dage", quant))
        {
            Core.KillMonster("tercessuinotlim-999999", "m2", "Spawn", "Dark Makai", "Defeated Makai", 25, false, false, false);
            Adv.BestGear(GearBoost.Chaos);
            ArmyHunt("aqlesson", new[] { "Carnax" }, "Carnax Eye", ClassType.Solo, true, 1);
            ArmyHunt("deepchaos", new[] { "Kathool" }, "Kathool Tentacle", ClassType.Solo, true, 1);
            ArmyHunt("dflesson", new[] { "Fluffy the Dracolich" }, "Fluffy's Bones", ClassType.Solo, true, 1);
            Adv.BestGear(GearBoost.Dragonkin);
            ArmyHunt("lair", new[] { "Red Dragon" }, "Red Dragon's Fang", ClassType.Solo, true, 1);
            Adv.BestGear(GearBoost.Human);
            ArmyHunt("bloodtitan", new[] { "Blood Titan" }, "Blood Titan's Blade", ClassType.Solo, true, 1);
        }
        Core.CancelRegisteredQuests();
    }

    // public void ArmyLegionRound4Medal()
    // {
    //     if (Core.CheckInventory("Legion Round 4 Medal"))
    //         return;

    //     Core.AddDrop(legionMedals);
    //     Core.Logger("Farming Legion Round 4 Medal");

    //     /*Sell existing medals to sync up army. Not sure how to implement foreach to replace this mess*/
    //     foreach (string item in legionMedals)
    //         Core.SellItem(item, 0, true);

    //     Core.RegisterQuests(4738, 4739, 4740, 4741);
    //     if (!Core.CheckInventory("Legion Round 1 Medal"))
    //     {
    //         ArmyHunt("shadowblast", new[] { "Caesaristhedark" }, "Nation Rookie Defeated", ClassType.Farm, true, 5);
    //         ArmyHunt("shadowblast", new[] { "Shadowrise Guard" }, "Shadowscythe Rookie Defeated", ClassType.Farm, true, 5);
    //         Bot.Wait.ForDrop("Legion Round 1 Medal");
    //         Core.Logger("Medal 1 acquired");
    //     }
    //     if (!Core.CheckInventory("Legion Round 2 Medal"))
    //     {
    //         ArmyHunt("shadowblast", new[] { "Carnage" }, "Nation Veteran Defeated", ClassType.Farm, true, 7);
    //         ArmyHunt("shadowblast", new[] { "Doombringer" }, "Shadowscythe Veteran Defeated", ClassType.Farm, true, 7);
    //         Bot.Wait.ForDrop("Legion Round 2 Medal");
    //         Core.Logger("Medal 2 acquired");
    //     }
    //     if (!Core.CheckInventory("Legion Round 3 Medal"))
    //     {
    //         ArmyHunt("shadowblast", new[] { "Minotaurofwar" }, "Nation Elite Defeated", ClassType.Farm, true, 10);
    //         ArmyHunt("shadowblast", new[] { "Draconic Doomknight" }, "Shadowscythe Elite Defeated", ClassType.Farm, true, 10);
    //         Bot.Wait.ForDrop("Legion Round 3 Medal");
    //         Core.Logger("Medal 3 acquired");
    //     }
    //     ArmyHunt("shadowblast", new[] { "Thanatos" }, "Thanatos Vanquished", ClassType.Solo, true, 1);
    //     Bot.Wait.ForDrop("Legion Round 4 Medal");
    //     Core.Logger("Medal 4 acquired");
    //     Core.CancelRegisteredQuests();
    // }

    public void ArmyDarkTokenOfDage(int quant = 600)
    {
        if (!Core.CheckInventory("Dark Token", quant))
            Core.SellItem("Dark Token", 0, true);
        else return;

        Core.FarmingLogger("Dark Token", quant);
        Core.AddDrop("Dark Token");
        Adv.BestGear(GearBoost.Human);
        Core.RegisterQuests(6248, 6249, 6251);
        while (!Bot.ShouldExit && !Core.CheckInventory("Dark Token", quant))
            ArmyHunt("seraphicwardage", new[] { "Seraphic Commander, Seraphic Soldier" }, "Seraphic Commanders Slain", ClassType.Farm, true, 6);
        Core.CancelRegisteredQuests();
    }

    public void ArmyLTs(int quant = 25000)
    {
        if (Core.CheckInventory("Legion Token", quant))
            return;
        Core.FarmingLogger("Legion Token", quant);
        Core.AddDrop("Legion Token");
        Core.EquipClass(ClassType.Farm);
        Adv.BestGear(GearBoost.Human);
        Core.RegisterQuests(4849);
        Army.SmartAggroMonStart("dreadrock", new[] { "Fallen Hero", "Hollow Wraith", "Legion Sentinel", "Shadowknight", "Void Mercenary" });
        while (!Bot.ShouldExit && !Core.CheckInventory("Legion Token", quant))
            Bot.Combat.Attack("*");
        Core.CancelRegisteredQuests();
        Army.AggroMonStop(true);
    }

    void ArmyHunt(string map = null, string[] monsters = null, string item = null, ClassType classType = ClassType.Farm, bool isTemp = false, int quant = 1)
    {
        Core.PrivateRooms = true;
        Core.PrivateRoomNumber = Army.getRoomNr();

        // if (Core.CheckInventory(item, quant))
        //     return;
        // Let the while loop check, so it doesn't stuck, waiting for players

        if (item == null)
            return;

        Core.AddDrop(item);

        Core.EquipClass(classType);
        Core.FarmingLogger(item, quant);

        Core.Join(map);
        WaitCheck();

        Army.SmartAggroMonStart(map, monsters);

        while (!Bot.ShouldExit && !Core.CheckInventory(item, quant))
            Bot.Combat.Attack("*");

        Army.AggroMonStop(true);
        Core.JumpWait();
    }

    void WaitCheck()
    {
        while (Bot.Map.PlayerCount < Bot.Config.Get<int>("armysize"))
        {
            Core.Logger($"Waiting for the squad. [{Bot.Map.PlayerNames.Count}/{Bot.Config.Get<int>("armysize")}]");
            Bot.Sleep(5000);
        }
        Core.Logger($"Squad All Gathered [{Bot.Map.PlayerNames.Count}/{Bot.Config.Get<int>("armysize")}]");
        Bot.Sleep(3500); //To make sure everyone attack at the same time, to avoid deaths
    }
}
