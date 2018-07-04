﻿using System;
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
        ObservableCollection<Model.Preset> presets;
        /// <summary>
        ///  読み込んだファイルのパス
        /// </summary>
        private string filePath;
        /// <summary>
        /// タイトル一覧で選択されたタイトル
        /// </summary>
        private Model.Preset selectedItem;
        /// <summary>
        /// タイトル一覧で選択されたタイトルのインデックス
        /// </summary>
        private int selectedItemIndex;

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
                    var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Model.Preset>));
                    presets = (ObservableCollection<Model.Preset>)serializer.ReadObject(memoryStream);
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
            var item = (Model.Preset)titleListView.SelectedItem;
            if(item == null)
            {
                return;
            }
            // 選択項目を保持する
            selectedItem = item;
            selectedItemIndex = titleListView.SelectedIndex;

            TitleField.Text = item.Title;
            ExplainField.Text = item.Explain;
            // TODO:キャラクターを表示する
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
                var serializer = new DataContractJsonSerializer(typeof(ObservableCollection<Model.Preset>));
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
            if(selectedItem == null)
            {
                return;
            }
            selectedItem.Title = ((TextBox)sender).Text;
            presets.RemoveAt(selectedItemIndex);
            presets.Insert(selectedItemIndex, selectedItem);
        }

        /// <summary>
        /// 説明欄が変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExplainField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(selectedItem==null)
            {
                return;
            }
            selectedItem.Explain = ((TextBox)sender).Text;
            presets[selectedItemIndex] = selectedItem;
        }

        /// <summary>
        /// キャラ欄でEnterを押したとき。キャラを追加する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CharacterField_KeyDown(object sender, KeyEventArgs e)
        {
            if(selectedItem==null)
            {
                return;
            }
            if( e.Key == Key.Return)
            {
                selectedItem.Characters.Add(((TextBox)sender).Text);
                ((TextBox)sender).Clear();
            }
        }
    }
}
