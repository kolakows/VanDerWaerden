namespace VanDerWaerden
{
    public static class Constants
    {
        public const MoveSelection moveSelection = MoveSelection.MostVisited;
    }

    public enum MoveSelection
    {
        MostVisited,
        BestScore,
    }
}