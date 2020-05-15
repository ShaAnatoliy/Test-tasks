public ICommand StartCountCmd { get { return new RelayCommand((obj) => StartCountRunCmd()); } }
private void StartCountRunCmd()
{
	// OpenDir
}

// ----------------------------------------------------------
/*
А я бы сделал так:
Есть к примеру у нас строка:
*/
string str = "Словом можно убить, словом можно спасти.";

/*
Нам надо для начала перевести ее в нижний регистр (ToLower()), 
далее разбить на слова. Как на мой взгляд это хорошо делает Regex.Split 
(ибо ему не надо задавать все знаки для "разбития"). 
Ну и дальше, имея массив всех слов, мы можем сгруппировать их 
и составить новую коллекцию, которая будет содержать кол-во найденных слов и само слово. 
Исходя из этого, наша строка будет примерно следующей:
*/
var words = Regex.Split(str.ToLower(), @"\W+")
    .Where(x => !string.IsNullOrEmpty(x))
    .GroupBy(g => g)
    .Select(s => new {Word = s.Key, Count = s.Count()});

// Собственно все. Можно теперь смело выводить все, что нам требуется:

var count = words.FirstOrDefault(x => x.Word == "словом")?.Count;
Если нам не нужно знать о других словах, то можно немного переписать:

var words = Regex.Split(str.ToLower(), @"\W+")
    .Where(x => x == "словом")
    .GroupBy(g => g)
    .Select(s => new {Word = s.Key, Count = s.Count()})
    .FirstOrDefault()?.Count;
/* * Select в данном примере является неким удобством, 
что бы мы знали и слово и кол-во найденных соответствий. 
На выходе получим int?, который вернет число найденных слов, 
либо null, если такого слова не найдено.
Ну, если нужно просто кол-во слов, то можно вообще поступить следующим образом: */

int words = Regex.Split(str.ToLower(), @"\W+").Count(x => x == "словом");

/*
После where groupby уже не нужен. достаточно просто <count> взять

В моих вариантах я для удобства вывожу Word и Count, без GroupBy этого вроде не сделать. 

Select в данном примере является неким удобством, что бы мы знали и слово 
и кол-во найденных соответствий - ты уже знаешь слово, потому что используешь 
его в where, именно поэтому groupby не нужен: будет всегда только одна группа
*/