using PinkWpf.Windows;
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

namespace PinkWpf.Tests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //this.InstallModule<ResizableWindowModule>();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var module = this.InstallModule<SystemMenuModule>();
            module.SystemMenu[module.SystemMenu.Count - 1] = new SystemMenuItem() { Header = "Hello world" };
            var firstItem = module.SystemMenu[0];
            firstItem.Header = "First item";
            var item = new SystemMenuItem() { Header = "No, I'm f&irst" };
            item.Checked = true;
            //item.Disabled = true;
            module.SystemMenu.Insert(4, item);
            item.SubMenu = SystemMenu.CreatePopup();
            item.SubMenu.Add(new SystemMenuItem() { Header = "Test1", Id = 12312 });
            item.SubMenu.Add(new SystemMenuItem() { Header = "Test2", Break = MenuBreak.Break });
            item.SubMenu[0].Click += MainWindow_Click;
            item.SubMenu[1].Default = true;
            item.Default = true;
            item.Click += Item_Click;

            module.SystemMenu.First(x => x.Header.Replace("&", "") == "Minimise").Click += MainWindow_Click1;
        }

        private void Item_Click(object? sender, EventArgs e)
        {
            
        }

        private void MainWindow_Click1(object? sender, EventArgs e)
        {
            
        }

        private void MainWindow_Click(object sender, EventArgs e)
        {
            var item = (SystemMenuItem)sender;
            item.Checked = !item.Checked;
        }

        private void OnMinClick(object sender, RoutedEventArgs e)
        {
            var menu = SystemMenu.CreatePopup();
            menu.Add(new SystemMenuItem() { Header = "Test1" });
            menu.Add(new SystemMenuItem() { Header = "Test2", Id = 3215 });
            menu[1].Click += OnMainWindow_Click;
            menu[1].Checked = debug;
            menu[0].Default = true;
            menu.Show(this, 100, 100);
            //menu[1].Default = true;
            //WindowState = WindowState.Minimized;
        }

        bool debug;

        private void OnMainWindow_Click(object? sender, EventArgs e)
        {
            var item = (SystemMenuItem)sender;
            debug = !item.Checked;
        }

        private void OnMaxClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void OnRestoreClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }
    }
}
