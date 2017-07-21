using UnityEngine;
using System.Collections;
using Game.Views.Components;

namespace Game.Managers
{

  public class GamePlayersManager : GameManager
  {
    public CellPlayerContentComponent CreatePlayer(int playerId, Transform levelTransform)
    {
      return GameObject.Instantiate(Resources.Load<CellPlayerContentComponent>("Players/Player_01"), levelTransform);
    }
  }
}