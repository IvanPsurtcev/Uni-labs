#define _CRT_SECURE_NO_WARNINGS
#include "histogram.h"
#include <iostream>
#include <iomanip>
#include <algorithm>

using namespace std;

///////////////////////////////////////////////////////////////////
// Вспомогательные функции
void AddBar(Histogram &historgam, const char *strName);
bool AppendBar(Histogram &historgam, const char *strName);

///////////////////////////////////////////////////////////////////
// реализация функций из файла histogram.h


//-----------------------------------------------------------------
// Функция AddBlock увеличивает значение столбца strName гистограмы historgam на 1.
// Если столбца с таким именем не существует, он создается.
void AddBlock(Histogram &historgam, const char *strName){
	if(!AppendBar(historgam, strName)){
		AddBar(historgam, strName);
	}
}

//-----------------------------------------------------------------
// Функция PrintHistogram выводит гистограму на экран.
void PrintHistogram(const Histogram &histogram){
	for(int i=0; i<histogram.nBarsSize; i++){
		cout << setw(histogram.maxLength) << setfill(' ') << setiosflags(ios::left) << histogram.BarNames[i] << '|';
		//cout.setf
		cout << setfill(histogram.chBlock) << setw(10*histogram.Bars[i]/histogram.nMaxBar) << "" << /*ends <<*/ endl;
	}
}

///////////////////////////////////////////////////////////////////
// реализация вспомогательных функций

//-----------------------------------------------------------------
// Функция AppendBar увеличивает значение столбца strName гистограмы historgam на 1.
bool AppendBar(Histogram &historgam, const char *strName){
	for(int i=0; i<historgam.nBarsSize; i++){
		if(strcmp(historgam.BarNames[i], strName)==0){
			historgam.nMaxBar = max(historgam.nMaxBar, ++historgam.Bars[i]);
			return true;
		}
	}
	return false;
}

// Функция AddBar создает в гистограме historgam новый столбец с именем strName.
//-----------------------------------------------------------------
void AddBar(Histogram &historgam, const char *strName){
	char **newNames = new char *[historgam.nBarsSize+1];
	int   *newBars  = new int[historgam.nBarsSize+1];
	if(historgam.nBarsSize > 0){
		for(int i=0; i<historgam.nBarsSize; i++){
			newNames[i] = historgam.BarNames[i];
			newBars[i] = historgam.Bars[i];
		}
		delete[] historgam.BarNames;
		delete[] historgam.Bars;
	}

	newNames[historgam.nBarsSize] = new char[strlen(strName)+1];
	historgam.maxLength = std::max(static_cast<int>(strlen(strName)), historgam.maxLength); // Проверяем на максимальную длину строки
	strcpy(newNames[historgam.nBarsSize], strName);
	newBars[historgam.nBarsSize] = 1;

	historgam.nBarsSize++;
	historgam.BarNames = newNames;
	historgam.Bars = newBars;
}

