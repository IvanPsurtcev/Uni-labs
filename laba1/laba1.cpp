#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
#include <iomanip>
#include <algorithm>
#include "histogram.h"

void SortByVal(Histogram& historgam, bool bAscending = true); // ���������� �������� �����������
bool ComparePairGreater(std::pair<int, char*> l, std::pair<int, char*> r); // ���������� - �������, ������� ���������� ��� ����
bool ComparePairLess(std::pair<int, char*> l, std::pair<int, char*> r); // ���������� - ���������� ��������
char* RandString(int& cmpMaxLen, int strMinLen = 1, int strMaxLen = 8); //���������� ��������� ������
void AddRandomBlocks(Histogram& hist);//������� ��������� �����������
void RandStringHist(char* randomString, int strLength); //������� ��� ������� F


int main() 
{

	std::srand(time(NULL));// ������� ��������� �����
	const char* const strProgramName = "Histogramer 1.1";

	std::cout << std::setfill('*') << std::setw(strlen(strProgramName) + 4) << "" << std::endl;
	std::cout << '*' << std::setfill(' ') << std::setw(strlen(strProgramName) + 3) << '*' << std::endl;
	std::cout << "* " << strProgramName << " *" << std::endl;
	std::cout << '*' << std::setfill(' ') << std::setw(strlen(strProgramName) + 3) << '*' << std::endl;
	std::cout << std::setfill('*') << std::setw(strlen(strProgramName) + 4) << "" << std::endl << std::endl;

	Histogram myBarChart;// �������� �����������

	InitHistogram(myBarChart); // ������������� �����������*
	AddRandomBlocks(myBarChart); // �������� ��������� �����������

	std::cout << "Before sorting: \n";
	PrintHistogram(myBarChart); //������� �����������(���������� �� �������)*

	SortByVal(myBarChart); //��������� ������� ����������� (true-�� �����������, false-�� ��������)

	std::cout << "\nAfter sorting: \n";
	PrintHistogram(myBarChart);
	DestroyHistogram(myBarChart); // ���������� �����������*

	int strLength = 0; 
	char* randomString = RandString(strLength, 16, 32); // ������� ��������� ������� �������
	
	RandStringHist(randomString, strLength);

	delete[] randomString;

	return 0;
}

char* RandString(int& cmpMaxLen, int strMinLen, int strMaxLen) // ������� ��������� ������
{
	char length = rand() % (strMaxLen - strMinLen) + strMinLen; //������� ��������� ����� �� strMinLength �� strMaxLength
	if (length > cmpMaxLen) // cmpMaxLen - ���������� � ������������ ������ ������, ������� ��� �����������
	{
		cmpMaxLen = length;
	}

	char* str = new char[length + 1]; 

	for (int i = 0; i < length; ++i) 
	{
		str[i] = ' ' + rand() % 94; // �� ������� �� ~
	}
	str[length] = 0;

	return str;
}

void AddRandomBlocks(Histogram& hist) // ��������� �������� �����������
{
	char count = 4 + rand() % 10; // ���������� ��������
	for (char i = 0; i < count; ++i)
	{
		char* str = RandString(hist.maxLength); // ��������� ���� ������ ��������� 
		char size = 1 + rand() % 15; // ������� ������� �����
		for (char j = 0; j < size; ++j)
		{
			AddBlock(hist, str); // ��������� ������ 
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

void SortByVal(Histogram& historgam, bool bAscending) //��������� ������� �����������
{
	std::pair<int, char*>* pairArray = new std::pair<int, char*>[historgam.nBarsSize]; // ������� ������ ���(���-�� ������� - ��������)
	for (int i = 0; i < historgam.nBarsSize; ++i)
	{
		pairArray[i] = std::pair<int, char*>(historgam.Bars[i], historgam.BarNames[i]); // ���������� ������� ���
	}

	if (bAscending) // ���������� �� �����������, ��������
	{
		std::sort(pairArray, pairArray + historgam.nBarsSize, ComparePairGreater);
	}
	else
	{
		std::sort(pairArray, pairArray + historgam.nBarsSize, ComparePairLess);
	}

	for (int i = 0; i < historgam.nBarsSize; ++i) // �� ���������������� ������� � �����������
	{
		historgam.Bars[i] = pairArray[i].first;
		historgam.BarNames[i] = pairArray[i].second;
	}

	delete[] pairArray;
}

void RandStringHist(char* randomString, int strLength)
{
	Histogram strHist; // �������� ����� �����������
	InitHistogram(strHist);
	for (int i = 0; i < strLength; ++i)
	{
		char* tmpStr = new char[2]; //������� ������� �� 2� ��������(����� � \0)
		if ((randomString[i] > 'a' && randomString[i] < 'z') || (randomString[i] > 'A' && randomString[i] < 'Z') || (randomString[i] > '0' && randomString[i] < '9'))
		{
			tmpStr[0] = randomString[i]; //���������� ����� � ������� �� 2� ��������
			tmpStr[1] = 0;
			AddBlock(strHist, tmpStr); // ��������� 1 � ������� �������
		}
		delete[] tmpStr;
	}

	std::cout << "\nString: " << randomString << std::endl;
	SortByVal(strHist); // ��������� �����������
	PrintHistogram(strHist);

	DestroyHistogram(strHist);
}