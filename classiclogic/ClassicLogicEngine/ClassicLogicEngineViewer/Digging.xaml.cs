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
using ClassicLogicEngineViewer.Annotations;

namespace ClassicLogicEngineViewer
{
  /// <summary>
  /// Логика взаимодействия для Digging.xaml
  /// </summary>
  public partial class Digging : UserControl, INotifyPropertyChanged
  {
    private double _ratio;

    public Digging()
    {
      InitializeComponent();
    }


    public double Ratio
    {
      get { return _ratio; }
      set
      {
        _ratio = value;
        Background = new SolidColorBrush(GetColor(value));
        OnPropertyChanged(nameof(Ratio));
        OnPropertyChanged(nameof(Background));
      }
    }

    public Color GetColor(double ratio)
    {
      Color startColor = Colors.Black;
      Color endColor = Colors.White;
      return Color.FromArgb(255,
          (byte)Math.Round(startColor.R * (1 - ratio) + endColor.R * ratio),
          (byte)Math.Round(startColor.G * (1 - ratio) + endColor.G * ratio),
          (byte)Math.Round(startColor.B * (1 - ratio) + endColor.B * ratio));

    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
