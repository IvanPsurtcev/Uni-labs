#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <iomanip>
#include <algorithm>
#include "histogram.h"

void SortByVal(Histogram& historgam, bool bAscending = true); // сортировка столбцов гистограммы
bool ComparePairGreater(std::pair<int, char*> l, std::pair<int, char*> r); // компаратор - функция, которая сравнивает две пары
bool ComparePairLess(std::pair<int, char*> l, std::pair<int, char*> r); // компаратор - сравнивает наоборот
char* RandString(int& cmpMaxLen, int strMinLen = 1, int strMaxLen = 8); //генерирует рандомную строку
void AddRandomBlocks(Histogram& hist);//создает рандомную гистограмму
void RandStringHist(char* randomString, int strLength); //функция для задания F


int main() 
{

	std::srand(time(NULL));// создает рандомные числа
	const char* const strProgramName = "Histogramer 1.1";

	std::cout << std::setfill('*') << std::setw(strlen(strProgramName) + 4) << "" << std::endl;
	std::cout << '*' << std::setfill(' ') << std::setw(strlen(strProgramName) + 3) << '*' << std::endl;
	std::cout << "* " << strProgramName << " *" << std::endl;
	std::cout << '*' << std::setfill(' ') << std::setw(strlen(strProgramName) + 3) << '*' << std::endl;
	std::cout << std::setfill('*') << std::setw(strlen(strProgramName) + 4) << "" << std::endl << std::endl;

	Histogram myBarChart;// создание гистограммы

	InitHistogram(myBarChart); // инициализация гистограммы*
	AddRandomBlocks(myBarChart); // рандомно заполняет гистограмму

	std::cout << "Before sorting: \n";
	PrintHistogram(myBarChart); //выводит гистограмму(отредачили по заданию)*

	SortByVal(myBarChart); //сортирует столбцы гистограммы (true-по возрастанию, false-по убыванию)

	std::cout << "\nAfter sorting: \n";
	PrintHistogram(myBarChart);
	DestroyHistogram(myBarChart); // уничтожает гистограмму*

	int strLength = 0; 
	char* randomString = RandString(strLength, 16, 32); // создает рандомную длинную строчку
	
	RandStringHist(randomString, strLength);

	delete[] randomString;

	return 0;
}

char* RandString(int& cmpMaxLen, int strMinLen, int strMaxLen) // создает рандомную строку
{
	char length = rand() % (strMaxLen - strMinLen) + strMinLen; //создает случайное число от strMinLength до strMaxLength
	if (length > cmpMaxLen) // cmpMaxLen - переменная с максимальной длиной строки, которая уже встречалась
	{
		cmpMaxLen = length;
	}

	char* str = new char[length + 1]; 

	for (int i = 0; i < length; ++i) 
	{
		str[i] = ' ' + rand() % 94; // от пробела до ~
	}
	str[length] = 0;

	return str;
}

void AddRandomBlocks(Histogram& hist) // заполняет рандомно гистограмму
{
	char count = 4 + rand() % 10; // количество столбцов
	for (char i = 0; i < count; ++i)
	{
		char* str = RandString(hist.maxLength); // создается одна строка рандомная 
		char size = 1 + rand() % 15; // сколько пунктов будет
		for (char j = 0; j < size; ++j)
		{
			AddBlock(hist, str); // добавляет пункты 
		}
	}
}

bool ComparePairGreater(std::pair<int, char*> l, std::pair<int, char*> r)
{
	return l.first < r.first;
}
bool ComparePairLess(std::pair<int, char*> l, std::pair<int, char*> r)
{
	return l.first > r.first;
}

void SortByVal(Histogram& historgam, bool bAscending) //сортирует столбцы гистограммы
{
	std::pair<int, char*>* pairArray = new std::pair<int, char*>[historgam.nBarsSize]; // создает массив пар(кол-во пунктов - столбцец)
	for (int i = 0; i < historgam.nBarsSize; ++i)
	{
		pairArray[i] = std::pair<int, char*>(historgam.Bars[i], historgam.BarNames[i]); // заполнение массива пар
	}

	if (bAscending) // сортировка по возрастанию, убыванию
	{
		std::sort(pairArray, pairArray + historgam.nBarsSize, ComparePairGreater);
	}
	else
	{
		std::sort(pairArray, pairArray + historgam.nBarsSize, ComparePairLess);
	}

	for (int i = 0; i < historgam.nBarsSize; ++i) // из отсортированного массива в гистограмму
	{
		historgam.Bars[i] = pairArray[i].first;
		historgam.BarNames[i] = pairArray[i].second;
	}

	delete[] pairArray;
}

void RandStringHist(char* randomString, int strLength)
{
	Histogram strHist; // создание новой гистограммы
	InitHistogram(strHist);
	for (int i = 0; i < strLength; ++i)
	{
		char* tmpStr = new char[2]; //создает строчку из 2х символов(буква и \0)
		if ((randomString[i] > 'a' && randomString[i] < 'z') || (randomString[i] > 'A' && randomString[i] < 'Z') || (randomString[i] > '0' && randomString[i] < '9'))
		{
			tmpStr[0] = randomString[i]; //записываем букву в строчку из 2х символов
			tmpStr[1] = 0;
			AddBlock(strHist, tmpStr); // добавляет 1 к нужному столбцу
		}
		delete[] tmpStr;
	}

	std::cout << "\nString: " << randomString << std::endl;
	SortByVal(strHist); // сортирует гистограмму
	PrintHistogram(strHist);

	DestroyHistogram(strHist);
}