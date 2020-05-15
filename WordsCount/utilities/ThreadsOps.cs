using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WordsCount
{
	public class ThreadsOps
	{
		public delegate void CountWords(string pathFile, List<WordCount> lstW, int countCharsInWord);

		private readonly MainWinVM wpfContext;

		private int TotalProcessed = 0;

		public ThreadsOps(MainWinVM WpfContext)
		{
			wpfContext = WpfContext;
		}

		public void StartThreadsCmd()
		{
			foreach (var file in wpfContext.ListFiles)
			{
				TextWorker txw = new TextWorker();
				CountWords cwrDelegat = new CountWords(txw.CountWords);

				// Запуск процесса фоновой обработки файла
				var dsp = wpfContext.mainWindow.Dispatcher.BeginInvoke(cwrDelegat, 
					file, wpfContext.TopNnFromFiles, wpfContext.CountCharsInWord);

				dsp.Completed += Dsp_Completed;
			}
		}

		private void Dsp_Completed(object sender, EventArgs e)
		{
			TotalProcessed++;

			if (wpfContext.ListFiles.Count == TotalProcessed)
			{
				// Все файлы обработаны - подвести итог
				wpfContext.EndProcess();
			}
		}

	}
}
