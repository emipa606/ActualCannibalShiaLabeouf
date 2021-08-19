using RimWorld;
using Verse;

namespace ActualCannibalShiaLabeouf
{
    [DefOf]
    public static class DefOfs_ShiaSurprise
    {
        public static FactionDef HollywoodCannibals;

        //public static FactionDef SpacerHostile;
        public static PawnKindDef ShiaKind;
        public static BodyTypeDef Male;
        public static PawnsArrivalModeDef EdgeWalkIn;
        public static LetterDef ThreatSmall;
        public static TaleDef Raid;
    }

    public static class Consts_ShiaSurprise
    {
        public const string path = "Things/Pawn/Humanlike/Heads/Male/Male_Narrow_Wide";
        public const string letterLabel = "Shia Surprise!";

        public const string letterText =
            "You're walking in the woods. There's no one around and your phone is dead. Out of the corner of your eye, you spot him: Shia Labeouf!";
    }
}