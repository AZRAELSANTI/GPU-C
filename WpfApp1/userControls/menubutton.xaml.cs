using MahApps.Metro.IconPacks;
using System.Windows;
using System.Windows.Controls;


namespace WpfApp1.userControls
{
    /// <summary>
    /// Логика взаимодействия для menubutton.xaml
    /// </summary>
    public partial class menubutton : UserControl
    {
        public menubutton()
        {
            InitializeComponent();
        }
         public PackIconMaterialKind Icon
        {
            get { return (PackIconMaterialKind)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }

        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(PackIconMaterialKind), typeof(menubutton));

            
        public bool IsActive

       {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        


        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("isActive", typeof(bool), typeof(menubutton));

       
    }
}


