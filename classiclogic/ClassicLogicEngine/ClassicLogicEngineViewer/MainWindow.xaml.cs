using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ClassicLogic.Engine;
using ClassicLogic.Outputs;
using ClassicLogic.Utils;
using ClassicLogicEngineViewer.Annotations;
using ClassicLogicEngineViewer.Commands;
using Point = ClassicLogic.Engine.Point;

namespace ClassicLogicEngineViewer
{
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    public static readonly DependencyProperty IsPlayProperty = DependencyProperty.Register("IsPlay", typeof(bool), typeof(MainWindow), new PropertyMetadata(default(bool)));
    private readonly Dictionary<Type, PlayCommand> _commands = new Dictionary<Type, PlayCommand>();

    private readonly Engine _engine;
    private readonly DispatcherTimer _timer;
    private readonly TileContainer _tileContainer;

    public MainWindow()
    {
      InitializeComponent();

      _tileContainer = new TileContainer(Canvas, TopCanvas);

      KeyDown += OnKeyDown;

      var tick = 0;
      _timer = new DispatcherTimer(DispatcherPriority.Background, Dispatcher);
      _timer.Interval = TimeSpan.FromSeconds(1 / 30.0);
      _timer.Tick += (sender, args) =>
      {
        if (IsPlay)
        {
          Tick(++tick);
        }
      };
      _timer.Start();

      _engine = new Engine(new StringLevelReader(LevelsData.Level0));

      AddCommand<InitializeEvent, InitializeCommand>();
      AddCommand<MoveRunnerEvent, MoveRunnerCommand>();
      AddCommand<ShowHideLadderEvent, ShowHideLadderCommand>();
      AddCommand<MoveGuardEvent, MoveGuardCommand>();
      AddCommand<GuardHasGoldEvent, GuardHasGoldCommand>();
      AddCommand<AddGoldEvent, AddGoldCommand>();
      AddCommand<RemoveGoldEvent, RemoveGoldCommand>();
      AddCommand<StartFillHoleEvent, StartFillHoleCommand>();
      AddCommand<EndFillHoleEvent, EndFillHoleCommand>();
      AddCommand<ShowTrapEvent, ShowTrapCommand>();
      AddCommand<StartDiggingEvent, StartDiggingCommand>();
      AddCommand<StopDiggingEvent, StopDiggingCommand>();
      AddCommand<DiggingCompleteEvent, DiggingCompleteCommand>();
      AddCommand<DigHoleProcessEvent, DigHoleProcessCommand>();
      AddCommand<FillHoleProcessEvent, FillHoleProcessCommand>();
      AddCommand<RunnerDeadEvent, RunnerDeadCommand>();
    }

    public bool IsPlay
    {
      get { return (bool)GetValue(IsPlayProperty); }
      set { SetValue(IsPlayProperty, value); OnPropertyChanged(nameof(IsPlay)); }
    }

    private void AddCommand<T, T1>() where T1 : PlayCommand<T>, new() where T : OutputEvent
    {
      _commands[typeof(T)] = new T1();
    }

    private void Tick(int i)
    {
      _engine.FastForward(i);
      while (_engine.Output.Count != 0)
      {
        var evt = _engine.Output.Dequeue();
        PlayCommand command;
        if (_commands.TryGetValue(evt.GetType(), out command))
        {
          command.Execute(evt, _tileContainer);
        }
        _engine.Output.Return(evt);
      }
    }

    private void OnKeyDown(object sender, KeyEventArgs evt)
    {
      switch (evt.Key)
      {
        case Key.Left:
          _engine.SetAction(InputAction.Left);
          break;
        case Key.Right:
          _engine.SetAction(InputAction.Right);
          break;
        case Key.Up:
          _engine.SetAction(InputAction.Up);
          break;
        case Key.Down:
          _engine.SetAction(InputAction.Down);
          break;
        case Key.Z:
          _engine.SetAction(InputAction.DigLeft);
          break;
        case Key.X:
          _engine.SetAction(InputAction.DigRight);
          break;
        default:
          _engine.SetAction(InputAction.Unknown);
          break;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
