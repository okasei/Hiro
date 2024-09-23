using Hiro.Helpers.Plugin;
using Hiro.ModelViews;
using System.Threading;
using System.Windows;

namespace Hiro.Tests
{
    /// <summary>
    /// PluginTest.xaml 的交互逻辑
    /// </summary>
    public partial class PluginTest : Window
    {
        Thread? _thread = null;
        HiroPlugin? plugin;
        public PluginTest()
        {
            InitializeComponent();
            //plugin = HPluginManager.CreateOrLoad(@"C:\Rex\Documents\VSProjects\HiroPlugins\HFiler\HFiler\bin\x64\Release\net8.0-windows\HFiler.hdll");
        }

        private void PressBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(plugin?.ProcessRequest(Para.Text.ToString(), null) ?? "未响应");
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (plugin?.Unload() == false)
                MessageBox.Show("插件卸载失败!");
        }
    }
}
