using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace WordsCount
{
	/// <summary>
	/// ViewModel основного окна
	/// </summary>
	public class MainWinVM : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		internal void OnPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public readonly MainWindow mainWindow;

		public MainWinVM()
		{
			this.mainWindow = null;
		}

		public MainWinVM(MainWindow mainWindow)
		{
			this.mainWindow = mainWindow;
		}

		private static readonly List<WordCount> NOTWORDS = new List<WordCount> { new WordCount() { Word = "Нет слов", Count = 0 } };

		public string OpenDir { get; set; } = "";
		public string ShowMessage { get; set; } = "";
		public bool EnableButtons { get; set; } = true;
		public int BorderThick { get; set; } = 0;
		public List<string> ListFiles { get; set; }
		public List<WordCount> TopNnWords { get; set; } = NOTWORDS;
		public List<WordCount> TopNnFromFiles { get; set; }
		public int CountCharsInWord { get; set; } = 7;

		public ICommand OpenFolderCmd { get { return new RelayCommand((obj) => OpenFolderRunCmd()); } }

		private void OpenFolderRunCmd()
		{
			System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
			folderBrowser.Description = "Где текстовые файлы?";
			folderBrowser.SelectedPath = Environment.CurrentDirectory;
			folderBrowser.ShowNewFolderButton = false;
			System.Windows.Forms.DialogResult result = folderBrowser.ShowDialog();
			if (string.IsNullOrWhiteSpace(folderBrowser.SelectedPath) ||
				(result != System.Windows.Forms.DialogResult.OK && result != System.Windows.Forms.DialogResult.Yes))
			{
				return;
			}
			OpenDir = folderBrowser.SelectedPath;
			OnPropertyChanged("OpenDir");
		}

		public ICommand StartCountCmd { get { return new RelayCommand((obj) => StartCountRunCmd()); } }

		private void StartCountRunCmd()
		{
			if (string.IsNullOrWhiteSpace(OpenDir))
				return;

			// Заблочить кнопки..
			EnableButtons = false;
			OnPropertyChanged("EnableButtons");
			BorderThick = 2;
			OnPropertyChanged("BorderThick");

			string[] arrFiles = Directory.GetFiles(OpenDir, "*.txt");

			if (arrFiles.Length == 0)
			{
				ShowMessage = "*.TXT файлы не найдены!";
			}
			else
			{
				ShowMessage = $"Процесс обработки. Количество файлов: {arrFiles.Length} ...";
			}
			OnPropertyChanged("ShowMessage");

			ListFiles = arrFiles.ToList(); // Передача списка файлов

			// Данные предыдущего сеанса не нужны
			TopNnWords = NOTWORDS;
			OnPropertyChanged("TopNnWords");
			TopNnFromFiles = new List<WordCount> { };

			ThreadsOps thopers = new ThreadsOps(this);
			thopers.StartThreadsCmd();
		}

		/**
		 * Все файлы обработаны - подвести итог
		*/
		public void EndProcess()
		{
			// РаЗаблочить кнопки..
			EnableButtons = true;
			OnPropertyChanged("EnableButtons");
			BorderThick = 0;
			OnPropertyChanged("BorderThick");

			// string filename = Path.GetFileName(file);
			using (TextWriter tw = new StreamWriter(@"D:\Dev2020\csharpp\Всего.txt"))
			{
				foreach (var s in TopNnFromFiles)
					tw.WriteLine($"{s.Word} = ${s.Count}");
			}

			var topnn = TextWorker.GetTopWords(TopNnFromFiles, 10);

			// Показать результат работы
			if (topnn.Count() > 0)
			{
				TopNnWords = new List<WordCount> { };
				TopNnWords.AddRange(topnn);
			}
			else
				TopNnWords = NOTWORDS;

			OnPropertyChanged("TopNnWords");
		}

	}
}