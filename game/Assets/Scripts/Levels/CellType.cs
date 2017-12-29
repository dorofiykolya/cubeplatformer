namespace Game
{
  public enum CellType
  {
    Empty = 1,
    Solid = 1 << 1,
    Block = 1 << 2,
    Ladder = 1 << 3,
    Rope = 1 << 4,
    Trap = 1 << 5,
    HideLadder = 1 << 6,
    Coin = 1 << 7,
    Guard = 1 << 8,
    Reborn = 1 << 9,
    Player = 1 << 10,
    TeleportEnter = 1 << 11,
    TeleportExit = 1 << 12,
    GuardSpawnPoint = 1 << 13,
    PlayerSpawnPoint = 1 << 14
  }
}