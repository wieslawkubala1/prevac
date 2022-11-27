1. Przy pierwszym uruchomieniu programu tworzona jest baza danych w SQLite służąca do zapisywania danych zwracanych z Weather Forecast API.
2. W bazie zapisywana jest ostatnia aktualna pozycja i ostatnia aktualna prognoza pogody dla pozycji K2.
3. Na wykresie wyświetlają się trzy wartości temperatury: wartość aktualna, minimalna i maksymalna. 
   Często te wskazania pokrywają się, dlatego wykres zlewa się w jeden, ale pobierane są wszystkie trzy wartości.
4. Najpierw należy kliknąć przycisk "1. Get data from weather API and save to database". Program skomunikuje się z usługą przez protokół http i zapisze wyniki do bazy danych.
5. Następnie proszę kliknąć przycisk "2. Draw chart from data in database". Program wyświetli wykres z wykorzystaniem darmowej biblioteki do wykresów dla WPF: OxyPlot.
https://oxyplot.readthedocs.io/en/latest/getting-started/hello-wpf.html
https://oxyplot.github.io/

6. TODO: raportowanie przez bazę temperatury w formie procedury wbudowanej.