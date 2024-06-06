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
using MahApps.Metro.IconPacks;


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


