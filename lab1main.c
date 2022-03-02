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

int s = 0; // sum of bytes

int main()
{
	struct tm file_time;
    	struct stat file_stat;

	time_t t = time(NULL);
 	struct tm cur_time = *localtime(&t);

	char buf[128];
	int p[2];
	memset(&buf, 0, 128);

	pipe(p);

	if(fork() == 0) //child
	{
		close(1);
		dup(p[1]);
		close(p[0]);
		close(p[1]);
		system("ls -u");
		exit(0);
	}
	else // parent
	{
		wait(NULL);
		close(0);
		dup(p[0]);
		close(p[0]);
		close(p[1]);
		/*printf("%d-%d-%d-%d-%d\n",
		 cur_time.tm_year,
		 cur_time.tm_yday,
		 cur_time.tm_hour,
		 cur_time.tm_min,
		 cur_time.tm_sec);*/
		while(scanf("%s", buf) != EOF)
		{
			stat(buf, &file_stat);
			file_time = *gmtime(&(file_stat.st_atime));
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
				s += file_stat.st_size;
				printf("%s\n", buf);
				sleep(1);
			}
			memset(&buf, 0, 128);
		}
	printf("Total bytes:%d\n",s);
	}
	return 0;
}
