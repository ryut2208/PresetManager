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

namespace PresetManager
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var presets = new List<Model.Preset>
            {
                new Model.Preset {
                    Title = "BBCF", Explain = "説明", Characters = new List<string>
                    {
                        "a"
                    }
                }
            };
            titleListView.DataContext = presets;
        }

        /// <summary>
        /// プリセットを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// メニューの終了をクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMenuClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// タイトル一覧の行が選択されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void titleListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (Model.Preset)titleListView.SelectedItem;
            TitleField.Text = item.Title;
            ExplainField.Text = item.Explain;
        }
    }
}
