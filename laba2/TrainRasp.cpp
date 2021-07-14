#include "TrainRasp.h"
#include <fstream>
#include <string>

std::istream& operator>>(std::istream& in, TrainRasp& t)
{
	in >> t.num >> t.type >> t.timeForTravel >> t.timeDeparture; // ��� ����� ��������������� �������� ������� �����, ����� ���, �����...
	return in;
}

std::ostream& operator<<(std::ostream& out, TrainRasp& t) // ���������� ��������� ������ 
{
	out << t.num << '\t' << t.type << '\t' << t.timeForTravel << '\t' << t.timeDeparture << '\n'; // \t ��������� ������� � excel, \n - ������
	return out;
}

bool operator<(TrainRasp& t1, TrainRasp& t2) // ���������� ��������� ��������� 
{
	if (t1.timeDeparture == t2.timeDeparture)
	{
		return t1.timeForTravel < t2.timeForTravel;
	}
	return t1.timeDeparture < t2.timeDeparture;
}

bool operator>(TrainRasp& t1, TrainRasp& t2) 
{
	if (t1.timeDeparture == t2.timeDeparture)
	{
		return t1.timeForTravel > t2.timeForTravel;
	}
	return t1.timeDeparture > t2.timeDeparture;
}

bool operator>=(TrainRasp& t1, TrainRasp& t2)
{
	return t1.timeDeparture >= t2.timeDeparture;
}

bool operator<=(TrainRasp& t1, TrainRasp& t2)
{
	return t1.timeDeparture <= t2.timeDeparture;
}