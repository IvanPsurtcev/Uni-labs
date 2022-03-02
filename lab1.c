#include <stdio.h>
#include <stdlib.h> // system
#include <sys/types.h>
#include <unistd.h> // pipe, fork, close, dup, sleep
#include <time.h> //gmtime, localtime
#include <signal.h>
#include <setjmp.h> // sigjmp_buf, siglongjmp, sigsetjmp
#include <sys/stat.h> // stat
#include <string.h> //memset
#include <sys/wait.h>

sigjmp_buf abuf; /*store information about the proccess state before
interaption */

int s = 0; // sum of bytes
int k = 0; // counter of Interaptions

void handl()
{
	k += 1;
	if (k % 2 == 0)
	{
		printf("\nBytes: %d\n", s);
	}
	siglongjmp(abuf, 1);
}

int main()
{
	struct tm file_time;
    	struct stat file_stat;

	time_t t = time(NULL);
 	struct tm cur_time = *localtime(&t);

	char buf[128];
	int p[2]; //межпроцессный канал
	memset(&buf, 0, 128); // зануляем 128 байт у буфера

	struct sigaction prep;
	prep.sa_handler = handl;
	prep.sa_flags = 0;
	sigprocmask(0,0,&prep.sa_mask);
	sigaction(SIGINT,&prep, 0); // описываем как обрабатываем сигнал SIGINT
	sigsetjmp(abuf, 1);

	pipe(p); // открыли межпроцессный канал

	if(fork() == 0) //child
	{
		close(1);
		dup(p[1]); // перенаправляем стандартный вывод в родительский поток
		close(p[0]);
		close(p[1]);
		system("ls -u");// отправляется в родительский поток
		exit(0);
	}
	else // parent
	{
		wait(NULL); // ждем пока отработает дочерний поток
		close(0); // закрываем поток ввода 
		dup(p[0]); // принимаем запись из дочернего потока
		close(p[0]);
		close(p[1]);
		/*printf("%d-%d-%d-%d-%d\n",
		 cur_time.tm_year,
		 cur_time.tm_yday,
		 cur_time.tm_hour,
		 cur_time.tm_min,
		 cur_time.tm_sec);*/
		while(scanf("%s", buf) != EOF) //считываем из потока ввода по одной строчке
		{
			stat(buf, &file_stat); // статистика о файле
			file_time = *gmtime(&(file_stat.st_atime)); // запись времени
			/*printf("%s: %d-%d-%d-%d-%d\n",
			 buf,
			 foo.tm_year,
			 foo.tm_yday,
			 foo.tm_hour,
			 foo.tm_min,
			 foo.tm_sec);*/
			if(cur_time.tm_year == file_time.tm_year &&
			   cur_time.tm_mon == file_time.tm_mon &&
			   cur_time.tm_mday == file_time.tm_mday)
			{
				s += file_stat.st_size; // добавляем вес файла к переменной
				printf("%s\n", buf);
				sigsetjmp(abuf, 1);
				sleep(1);
			}
			memset(&buf, 0, 128); // очищаем буфер
		}
	printf("Total bytes:%d\n",s);
	}
	return 0;
}
