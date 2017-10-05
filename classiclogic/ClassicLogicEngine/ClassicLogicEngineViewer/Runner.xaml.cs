using System;
using System.Collections.Generic;
using System.Linq;
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
using Point = ClassicLogic.Engine.Point;

namespace ClassicLogicEngineViewer
{
  /// <summary>
  /// Логика взаимодействия для Runner.xaml
  /// </summary>
  public partial class Runner : UserControl
  {
    public Runner()
    {
      InitializeComponent();
    }

    public Point Position { get; set; }
  }
}
