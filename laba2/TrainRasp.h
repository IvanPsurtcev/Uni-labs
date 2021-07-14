#pragma once
#include <fstream>
struct TrainRasp // ������� ������� �������
{
	int num; // number
	int type; //1 - fast, 2 - passanger, 0 - not valid
	int timeForTravel; //in minutes
	int timeDeparture; //in minutes
};
// ���������� ����������
std::istream& operator>>(std::istream& in, TrainRasp& t);
std::ostream& operator<<(std::ostream& out, TrainRasp& t);
bool operator<(TrainRasp& t1, TrainRasp& t2);
bool operator>(TrainRasp& t1, TrainRasp& t2);
bool operator>=(TrainRasp& t1, TrainRasp& t2);
bool operator<=(TrainRasp& t1, TrainRasp& t2);


