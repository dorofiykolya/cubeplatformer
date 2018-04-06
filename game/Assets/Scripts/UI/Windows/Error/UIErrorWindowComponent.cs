using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Windows
{
  public class UIErrorWindowComponent : UIWindowComponent
  {
    [SerializeField]
    private Text _title;

    [SerializeField]
    private Text _message;

    public void SetTitle(string title)
    {
      _title.text = title;
    }

    public void SetMessage(string message)
    {
      _message.text = message;
    }
  }
}
