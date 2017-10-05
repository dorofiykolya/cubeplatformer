using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ClassicLogic.Engine;
using ClassicLogicEngineViewer.Annotations;

namespace ClassicLogicEngineViewer
{
  /// <summary>
  /// Логика взаимодействия для Tile.xaml
  /// </summary>
  public partial class Tile : UserControl, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(Tile), new PropertyMetadata(default(Brush)));
    private TileType _type;
    public static readonly DependencyProperty IsGoldProperty = DependencyProperty.Register("IsGold", typeof(Visibility), typeof(Tile), new PropertyMetadata(default(Visibility)));

    public Tile()
    {
      InitializeComponent();
    }

    public Brush Fill
    {
      get { return (Brush)GetValue(FillProperty); }
      set
      {
        SetValue(FillProperty, value);
        OnPropertyChanged(nameof(Fill));
      }
    }

    public TileType Type
    {
      get { return _type; }
      set
      {
        _type = value;
        switch (value)
        {
          case TileType.EMPTY_T:
            Fill = Brushes.White;
            break;
          case TileType.SOLID_T:
            Fill = Brushes.Black;
            break;
          case TileType.BLOCK_T:
            Fill = Brushes.SaddleBrown;
            break;
          case TileType.GOLD_T:
            Fill = Brushes.White;
            break;
          case TileType.BAR_T:
            Fill = Brushes.DeepSkyBlue;
            break;
          case TileType.LADDR_T:
            Fill = Brushes.LightBlue;
            break;
          case TileType.TRAP_T:
            Fill = Brushes.Aquamarine;
            break;
          case TileType.HLADR_T:
            Fill = Brushes.Green;
            break;
        }
        IsGold = value == TileType.GOLD_T ? Visibility.Visible : Visibility.Hidden;
      }
    }

    public Visibility IsGold
    {
      get { return (Visibility)GetValue(IsGoldProperty); }
      set
      {
        SetValue(IsGoldProperty, value);
        OnPropertyChanged(nameof(IsGold));
      }
    }


    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
