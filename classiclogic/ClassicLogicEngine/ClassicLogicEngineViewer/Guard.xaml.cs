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
using Point = ClassicLogic.Engine.Point;

namespace ClassicLogicEngineViewer
{
  /// <summary>
  /// Логика взаимодействия для Guard.xaml
  /// </summary>
  public partial class Guard : UserControl, INotifyPropertyChanged
  {
    public static readonly DependencyProperty HasGoldProperty = DependencyProperty.Register("HasGold", typeof(Visibility), typeof(Guard), new PropertyMetadata(default(Visibility)));
    public event PropertyChangedEventHandler PropertyChanged;

    public Guard()
    {
      InitializeComponent();
    }

    public int Id { get; set; }
    public Point Position { get; set; }

    public Visibility HasGold
    {
      get { return (Visibility)GetValue(HasGoldProperty); }
      set
      {
        SetValue(HasGoldProperty, value);
        OnPropertyChanged(nameof(HasGold));
      }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
