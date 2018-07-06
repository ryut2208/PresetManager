using System;
using System.Collections;
using System.Collections.ObjectModel;
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
using System.Runtime.Serialization.Json;
using System.IO;

namespace PresetManager
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Model.Preset> presets;
        /// <summary>
        ///  読み込んだファイルのパス
        /// </summary>
        private string filePath;
        /// <summary>
        /// タイトル一覧で選択されたタイトル
        /// </summary>
        private Model.Preset selectedTitle;
        /// <summary>
        /// タイトル一覧で選択されたタイトルのインデックス
        /// </summary>
        private int selectedTitleIndex;
        private String selectedCharacter;
        private int selectedCharacterIndex;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// プリセットを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenMenuClick(object sender, RoutedEventArgs e)
        {
            // ダイアログを開く
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                // フィルタ
                Filter = "JSONファイル(*.json)|*.json",
                FilterIndex = 1
            };

            if (dialog.ShowDialog() == true)
            {
                this.IsEnabled = false;
                ReadJson(dialog.FileName);
                this.IsEnabled = true;
            }

            foreach(var preset in presets)
            {
                titleListView.Items.Add(preset.Title);
            }
        }

        /// <summary>
        /// JSONを読み込む
        /// </summary>
        /// <param name="filePath"></param>
        private void ReadJson(string filePath)
        {
            this.filePath = filePath;
            var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(filePath, Encoding.UTF8);
            using (parser)
            {
                var jsonStr = parser.ReadToEnd();

                using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr)))
                using (var streamReader = new StreamReader(memoryStream))
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<Model.Preset>));
                    presets = (List<Model.Preset>)serializer.ReadObject(memoryStream);
                }
            }
            // TODO: ソート出来るようにする
            // presets.Sort((lhs, rhs) => lhs.Title.CompareTo(rhs.Title));
            // 読み込んだデータを反映させる
            titleListView.DataContext = presets;
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
        private void TitleListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = titleListView.SelectedIndex;
            if(index <0)
            {
                return;
            }
            // 選択項目を保持する
            selectedTitle = presets[index];
            selectedTitleIndex = index;

            TitleField.Text = selectedTitle.Title;
            ExplainField.Text = selectedTitle.Explain;
            CharacterField.Clear();
            
            // キャラクターの表示
            CharacterListView.Items.Clear();
            foreach(var character in selectedTitle.Characters)
            {
                CharacterListView.Items.Add(character);
            }
        }

        /// <summary>
        /// 新規タイトルの追加をクリックしたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewTitleMenuClick(object sender, RoutedEventArgs e)
        {
            if (presets == null)
            {
                return;
            }
            presets.Add(new Model.Preset());
            titleListView.Items.Add("");
            titleListView.ScrollIntoView(presets.Last().Title);
        }

        /// <summary>
        /// 上書き保存をクリックしたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OverwriteMenuClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.filePath))
            {
                return;
            }
            using (var memoryStream = new MemoryStream())
            using (var streamReader = new StreamReader(memoryStream))
            {
                var serializer = new DataContractJsonSerializer(typeof(List<Model.Preset>));
                serializer.WriteObject(memoryStream, presets);
                var jsonStr = Encoding.UTF8.GetString(memoryStream.ToArray());
                File.WriteAllText(filePath, jsonStr);
            }
        }

        /// <summary>
        /// タイトル欄が変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TitleField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(selectedTitle == null)
            {
                return;
            }
            selectedTitle.Title = ((TextBox)sender).Text;
            presets.RemoveAt(selectedTitleIndex);
            presets.Insert(selectedTitleIndex, selectedTitle);
        }

        /// <summary>
        /// 説明欄が変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExplainField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(selectedTitle==null)
            {
                return;
            }
            selectedTitle.Explain = ((TextBox)sender).Text;
            presets[selectedTitleIndex] = selectedTitle;
        }

        /// <summary>
        /// キャラ欄でEnterを押したとき。キャラを追加する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CharacterField_KeyDown(object sender, KeyEventArgs e)
        {
            if(selectedTitle==null)
            {
                return;
            }
            if(e.Key == Key.Return)
            {
                var fieldText = ((TextBox)sender).Text;
                if (!String.IsNullOrEmpty(selectedCharacter))
                {
                    // 選択されたキャラがある場合は編集
                    selectedTitle.Characters.RemoveAt(selectedCharacterIndex);
                    selectedTitle.Characters.Insert(selectedCharacterIndex, fieldText);
                    CharacterListView.Items[selectedCharacterIndex] = fieldText;
                    selectedCharacter = null;
                    selectedCharacterIndex = -1;
                }
                else
                {
                    selectedTitle.Characters.Add(fieldText);
                    CharacterListView.Items.Add(fieldText);
                }
                ((TextBox)sender).Clear();
            }
        }

        /// <summary>
        /// キャラを選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CharacterListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (string)CharacterListView.SelectedItem;
            if(item == null)
            {
                return;
            }
            selectedCharacter = item;
            selectedCharacterIndex = CharacterListView.SelectedIndex;
            CharacterField.Text = item;
        }

        /// <summary>
        /// タイトル選択のコンテキストメニューで削除を選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteTitleMenu_Click(object sender, RoutedEventArgs e)
        {
            presets.RemoveAt(selectedTitleIndex);
            titleListView.Items.RemoveAt(selectedTitleIndex);
            selectedTitle = null;
            selectedTitleIndex = -1;
        }
    }
}
