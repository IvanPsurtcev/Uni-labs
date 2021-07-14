#define _CRT_SECURE_NO_WARNINGS
#include "histogram.h"
#include <iostream>
#include <iomanip>
#include <algorithm>

using namespace std;

///////////////////////////////////////////////////////////////////
// ��������������� �������
void AddBar(Histogram &historgam, const char *strName);
bool AppendBar(Histogram &historgam, const char *strName);

///////////////////////////////////////////////////////////////////
// ���������� ������� �� ����� histogram.h


//-----------------------------------------------------------------
// ������� AddBlock ����������� �������� ������� strName ���������� historgam �� 1.
// ���� ������� � ����� ������ �� ����������, �� ���������.
void AddBlock(Histogram &historgam, const char *strName){
	if(!AppendBar(historgam, strName)){
		AddBar(historgam, strName);
	}
}

//-----------------------------------------------------------------
// ������� PrintHistogram ������� ���������� �� �����.
void PrintHistogram(const Histogram &histogram){
	for(int i=0; i<histogram.nBarsSize; i++){
		cout << setw(histogram.maxLength) << setfill(' ') << setiosflags(ios::left) << histogram.BarNames[i] << '|';
		//cout.setf
		cout << setfill(histogram.chBlock) << setw(10*histogram.Bars[i]/histogram.nMaxBar) << "" << /*ends <<*/ endl;
	}
}

///////////////////////////////////////////////////////////////////
// ���������� ��������������� �������

//-----------------------------------------------------------------
// ������� AppendBar ����������� �������� ������� strName ���������� historgam �� 1.
bool AppendBar(Histogram &historgam, const char *strName){
	for(int i=0; i<historgam.nBarsSize; i++){
		if(strcmp(historgam.BarNames[i], strName)==0){
			historgam.nMaxBar = max(historgam.nMaxBar, ++historgam.Bars[i]);
			return true;
		}
	}
	return false;
}

// ������� AddBar ������� � ���������� historgam ����� ������� � ������ strName.
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
	historgam.maxLength = std::max(static_cast<int>(strlen(strName)), historgam.maxLength); // ��������� �� ������������ ����� ������
	strcpy(newNames[historgam.nBarsSize], strName);
	newBars[historgam.nBarsSize] = 1;

	historgam.nBarsSize++;
	historgam.BarNames = newNames;
	historgam.Bars = newBars;
}

