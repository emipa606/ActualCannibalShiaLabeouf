using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
//"You're walking in the woods. 
// There's no one around and your phone is dead.
// Out of the corner of your eye, you spot him: Shia Labeouf!";

namespace ActualCannibalShiaLabeouf
{
    [StaticConstructorOnStartup]
    public class IncidentWorker_ShiaSurprise : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Log.Message("Shia TryExecuteWorker Launched");
            parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            parms.points = 50;
            if (!TryResolveRaidFaction(parms))
            {
                Log.Error("Failed to resolve shia raid faction");
                return false;
            }

            Find.TickManager.slower.SignalForceNormalSpeedShort();
            Find.StoryWatcher.statsRecord.numRaidsEnemy++;

            var list = new List<Pawn>();
            Log.Message("Trying to make a shia");
            var shia = GenerateShia(parms);
            list.Add(shia);
            Log.Message("Made a shia");

            if (list.Count == 0)
            {
                Log.Error("Got no pawns spawning raid from parms " + parms);
                return false;
            }

            if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
            {
                Log.Error("Failed to resolve raid spawn center");
                return false;
            }

            TargetInfo target = shia;
            //TargetInfo targetTest = new TargetInfo(parms.spawnCenter,map,false);
            parms.raidArrivalMode.Worker.Arrive(list, parms);

            PawnComponentsUtility.AddComponentsForSpawn(shia);
            //Is the below lines even needed?
            //DropPodUtility.DropThingsNear(parms.spawnCenter, map, list.Cast<Thing>(), parms.raidPodOpenDelay, false, true, true);
            //target = new TargetInfo(parms.spawnCenter, map, false);

            //message player
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Points = " + parms.points.ToString("F0"));
            foreach (var current2 in list)
            {
                var str = current2.equipment?.Primary == null
                    ? "unarmed"
                    : current2.equipment.Primary.LabelCap;
                stringBuilder.AppendLine(current2.KindLabel + " - " + str);
            }

            PawnRelationUtility.Notify_PawnsSeenByPlayer(list, out _);
            Find.LetterStack.ReceiveLetter(Consts_ShiaSurprise.letterLabel, Consts_ShiaSurprise.letterText,
                DefOfs_ShiaSurprise.ThreatSmall, target, parms.faction, null, null, stringBuilder.ToString());
            TaleRecorder.RecordTale(DefOfs_ShiaSurprise.Raid);
            parms.raidStrategy.Worker.MakeLords(parms, list);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons, OpportunityType.Critical);
            //Lord lord = LordMaker.MakeNewLord(parms.faction, new LordJob_AssaultColony(parms.faction, false, false, false, true, false), map, list);
            //Does not seem to be a 1.0 counterpart to the below line
            //AvoidGridMaker.RegenerateAvoidGridsFor(parms.faction, map);


            SoundDef.Named("Shia").PlayOneShot(SoundInfo.OnCamera());
            return true;
        }

        private bool TryResolveRaidFaction(IncidentParms parms)
        {
            parms.faction = Find.FactionManager.FirstFactionOfDef(DefOfs_ShiaSurprise.HollywoodCannibals);
            if (parms.faction == null)
            {
                Log.Message("Trying to generate new faction");
                var newfac =
                    FactionGenerator.NewGeneratedFaction(
                        new FactionGeneratorParms(DefOfs_ShiaSurprise.HollywoodCannibals));
                Find.FactionManager.Add(newfac);
                parms.faction = newfac;
                if (newfac != null && newfac.def == DefOfs_ShiaSurprise.HollywoodCannibals)
                {
                    Log.Message("Generated successfully");
                }
                else
                {
                    return false;
                }
            }

            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }


        private Pawn GenerateShia(IncidentParms parms)
        {
            if (parms.faction == null || PawnKindDef.Named("ShiaKind") == null)
            {
                Log.Error("shia mod cant find faction");
            }

            /*	public PawnGenerationRequest(
                    PawnKindDef kind, Faction faction = null, PawnGenerationContext context = PawnGenerationContext.NonPlayer, 
                    int tile = -1, bool forceGenerateNewPawn = false, bool newborn = false, bool allowDead = false,
                    bool allowDowned = false, bool canGeneratePawnRelations = true, bool mustBeCapableOfViolence = false,
                    float colonistRelationChanceFactor = 1f, bool forceAddFreeWarmLayerIfNeeded = false, bool allowGay = true,
                    bool allowFood = true, bool inhabitant = false, bool certainlyBeenInCryptosleep = false, Predicate<Pawn> validator = null, 
                    float? fixedBiologicalAge = null, float? fixedChronologicalAge = null, Gender? fixedGender = null, float? fixedMelanin = null, 
                    string fixedLastName = null)

    */
            Log.Message("Request");
            var request = new PawnGenerationRequest(
                DefOfs_ShiaSurprise.ShiaKind,
                parms.faction,
                PawnGenerationContext.NonPlayer,
                parms.target.Tile,
                true,
                false,
                false,
                false,
                false,
                true,
                0f, //no one is related to Shia
                true,
                false, //not gay i think ;)
                false,
                false,
                true,
                false,
                false,
                false,
                0,
                0,
                null, //validatorPre and Post Gear, Predicate<Pawn> null for now
                1,
                null, //post version of above
                null,
                null,
                null,
                0,
                31,
                31,
                Gender.Male,
                0.2691779f, //skin
                "LaBeouf");

            var pawn = PawnGenerator.GeneratePawn(
                request);

            pawn.health = new Pawn_HealthTracker(pawn);
            pawn.story.traits = new TraitSet(pawn);
            pawn.story.hairColor = Color.black;
            pawn.Name = new NameTriple("Shia", "Shia", "LaBeouf");
            pawn.story.hairDef = DefDatabase<HairDef>.GetNamed("Topdog");
            pawn.story.bodyType = DefOfs_ShiaSurprise.Male;
            typeof(Pawn_StoryTracker).GetField("headGraphicPath", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(pawn.story, Consts_ShiaSurprise.path);
            pawn.story.crownType = CrownType.Average;
            Log.Message("Setting skills");
            foreach (var current3 in pawn.skills.skills)
            {
                if (current3.def == SkillDefOf.Melee)
                {
                    current3.Level = 20;
                }
            }

            //Get closest backstory strings
            var childB = "ChildStar51";
            var adultB = "Actor76";


            BackstoryDatabase.TryGetWithIdentifier(childB, out var bsc);
            pawn.story.childhood = bsc;
            BackstoryDatabase.TryGetWithIdentifier(adultB, out var bsa);
            pawn.story.adulthood = bsa;

            /*
            String childB = BackstoryDatabase.GetIdentifierClosestMatch(rawChildB);
            String adultB = BackstoryDatabase.GetIdentifierClosestMatch(rawAdultB);
            bool childBSet = false;
            bool adultBSet = false;
            */


            //Get and set backstories old method
            /*
            foreach (KeyValuePair<string, Backstory> kvp in BackstoryDatabase.allBackstories)
            {
                //check child
                if((kvp.Key == childB || kvp.Value.identifier == childB) && !childBSet)
                {
                    pawn.story.childhood = kvp.Value;
                    childBSet = true;
                }

                //check adult
                if((kvp.Key == adultB || kvp.Value.identifier == adultB) && !adultBSet)
                {
                    pawn.story.adulthood = kvp.Value;
                    adultBSet = true;
                }
            }

            if(!childBSet)
            {
                Log.Warning("Shia childhood backstory not set");
            }
            if(!adultBSet)
            
                Log.Warning("Shia adulthood backstory not set");
            }
            */


            //Beta variant
            //pawn.story.childhood = BackstoryDatabase.allBackstories.Values.ToList<Backstory>().Find((Backstory b) => b.identifier.Equals("ChildStar51"));
            //pawn.story.adulthood = BackstoryDatabase.allBackstories.Values.ToList<Backstory>().Find((Backstory b) => b.identifier.Equals("Actor76"));

            pawn.story.traits.allTraits.RemoveAll(
                _ => true);
            pawn.story.traits.allTraits.Add(new Trait(TraitDefOf.Cannibal));
            pawn.story.traits.allTraits.Add(new Trait(TraitDefOf.Bloodlust));
            pawn.story.traits.allTraits.Add(new Trait(TraitDefOf.Psychopath));

            return pawn;
        }

        private void TryMakeShiaHead()
        {
            //Type db = typeof(GraphicDatabaseHeadRecords);
            //Type type = db.GetNestedType("HeadGraphicRecord");
            //typeof(GraphicDatabaseHeadRecords).GetField("heads")
            //type.
            //var record = Activator.CreateInstance(type);
            //(List<Type>)((db.GetField("heads",BindingFlags.Static | BindingFlags.NonPublic)).GetRawConstantValue()).Add(record);
        }
    }
}