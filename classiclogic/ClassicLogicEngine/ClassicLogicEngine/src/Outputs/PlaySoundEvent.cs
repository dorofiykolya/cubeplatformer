using ClassicLogic.Engine;

namespace ClassicLogic.Outputs
{
  public class PlaySoundEvent : OutputEvent
  {
    public Sounds Sound;
    public bool Loop;

    public void Set(Sounds sound, bool loop)
    {
      Sound = sound;
      Loop = loop;
    }
  }
}
