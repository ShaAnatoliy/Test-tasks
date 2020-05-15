using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WordsCount
{
	public class TextWorker
	{
		public void CountWords(string pathFile, List<WordCount> listW, int countCharsInWord)
		{
			string str = GetTextFromFile(pathFile);

			listW.AddRange(GetTopWords(str, countCharsInWord, 10));
		}

		private static string GetTextFromFile(string pathFile)
		{
			string line = "";

			try
			{   // Open the text file using a stream reader.
				using (StreamReader sr = new StreamReader(pathFile))
				{
					// Read the stream to a string, and write the string to the console.
					line = sr.ReadToEnd();
				}
			}
			catch { line = $"ОшибочнаяОшибкаЧтения \n\nфайла: {pathFile}"; }

			return line;
		}

		public static List<WordCount> GetTopWords(string text, int countCharsInWord, int countOnTop)
		{
			// TODO добавить вводный параметр countCharsInWord на форме MainWindow

			// Расчёт количества вхождений
			var words = Regex.Split(text.ToLower(), @"\W+")
				.Where(x => !string.IsNullOrEmpty(x) && x.Length > countCharsInWord)
				.GroupBy(g => g)
				.Select(s => new WordCount() { Word = s.Key, Count = s.Count() });

			// Объявить победителей в текущем тексте
			var lw = words.ToList();
			var topnn = lw.OrderByDescending(o => o.Count).Take(countOnTop);

			return topnn.ToList();
		}

		public static List<WordCount> GetTopWords(List<WordCount> listWords, int countOnTop)
		{
			var words = listWords.GroupBy(g => g.Word)
				.Select(w => new WordCount() { Word = w.Key, Count = w.Sum(s => s.Count) });

			// Объявить победителей в текущем тексте
			var lw = words.ToList();
			var topnn = lw.OrderByDescending(o => o.Count).Take(countOnTop);

			return topnn.ToList();
		}
	}
}
