using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.userControls
{
    /// <summary>
    /// Логика взаимодействия для UCGPU.xaml
    /// </summary>
    public partial class UCGPU : UserControl
    {
        public UCGPU()
        {
            InitializeComponent();
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }

        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(UCGPU));

        public string Number
        {
            get { return (string)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }

        }
        public static readonly DependencyProperty NumberProperty =
            DependencyProperty.Register("Number", typeof(string), typeof(UCGPU));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }

        }
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(UCGPU));

        public bool IsTechnical
        {
            get { return (bool)GetValue(IsTechnicalProperty); }
            set { SetValue(IsTechnicalProperty, value); }

        }
        public static readonly DependencyProperty IsTechnicalProperty =
            DependencyProperty.Register(" IsTechnical", typeof(bool), typeof(UCGPU));
    }
}
