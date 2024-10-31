using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrutalItems
{
    public static class Synergies
    {
        public static void Init()
        {
            // Version 1.0.0
            CreateSynergy("First Come First Serve", CustomSynergyTypeE.FIRST_COME_FIRST_SERVE,
                [Ids["modernmedicine"]],
                [Ids["trepanation"], AlphaBulletsId, LowerCaseRId]);

            CreateSynergy("High Focus", CustomSynergyTypeE.HIGH_FOCUS,
                [Ids["shardofnowak"]],
                [TableTechSightId, YellowChamberId, CogOfBattleId, BloodyEyeId, LiquidValkyrieId]);

            CreateSynergy("Standard Deviation", CustomSynergyTypeE.STANDARD_DEVIATION,
                [Ids["chainofcommand"], Ids["dewormingpills"]]);

            CreateSynergy("Stressful Focus", CustomSynergyTypeE.STRESSFUL_FOCUS,
                [Ids["shardofnowak"]],
                [RiddleOfLeadId, GildedHydraId, BulletIdolId, HoleyGrailId]);

            CreateSynergy("Focused Fury", CustomSynergyTypeE.FOCUSED_FURY,
                [Ids["shardofnowak"]],
                [HomingBulletsId, CrutchId, ScopeId, LaserSightId, AngryBulletsId]);

            CreateSynergy("Persistence of Time", CustomSynergyTypeE.PERSISTENCE_OF_TIME,
                [Ids["prussianblue"]],
                [BlueGuonStoneId, BulletTimeId, AgedBellId, SuperHotWatchId, ElderBlankId]);

            CreateSynergy("Glass Case", CustomSynergyTypeE.GLASS_CASE,
                [Ids["certificateofexemption"]],
                [GlassGuonStoneId]);

            // Add to DB
            AddSynergiesToDB();
        }
    }
}
