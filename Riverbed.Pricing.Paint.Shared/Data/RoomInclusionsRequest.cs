namespace Riverbed.Pricing.Paint.Shared.Data
{
    /// <summary>
    /// Request payload to update inclusion flags (walls, ceilings, doors, windows, baseboards, crown moldings)
    /// across all rooms in a project. Any property left null will not be changed.
    /// </summary>
    public sealed class RoomInclusionsRequest
    {
        public bool? IncludeWalls { get; set; }
        public bool? IncludeCeilings { get; set; }
        public bool? IncludeDoors { get; set; }
        public bool? IncludeWindows { get; set; }
        public bool? IncludeBaseboards { get; set; }
        public bool? IncludeCrownMoldings { get; set; }
    }

    public sealed class InclusionsUpdateResult
    {
        public int projectId { get; set; }
        public int updatedRooms { get; set; }
    }
}
