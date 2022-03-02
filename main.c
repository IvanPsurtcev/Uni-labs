#include <stdio.h>
#include <sys/types.h>
#include <unistd.h>
#include <stdlib.h>
#include <string.h>
#include <signal.h>
#include <sys/wait.h>
#include <sys/stat.h>
#include <time.h>

#define BUFFER_SIZE 4

typedef struct FilesList *PFile;
struct FilesList //определяем структуру с данными о файлах
{
    char *filename;
    off_t filesize;
    time_t last_access;
    PFile next;
};

PFile head = NULL; //создаем список файлов
void add_file(char *filename);
void interrupt();
void print_list(int mode);

void child(int pd) /*функция, выполняемая в процессе-потомке*/
{
    close(1);                       //закрываем файл стандартного вывода (запись)
    dup2(pd, 1);                    //закрываем стандартный вывод и дублируем его каналом на запись
    close(pd);                      //закрываем ненужную копию
    execlp("ls", "ls", "-1", NULL); //передаем список файлов текущего каталога в одноколоночном формате
}

void parent(int pd) /*функция, выполняемая в процессе-родителе*/
{
    char *str = NULL;
    size_t str_size = 0;
    pid_t cp;
    wait(&cp);                //ожидаем окончания процесса-потомка
    char buffer[BUFFER_SIZE]; //создаем буферный массив определенного размера
    ssize_t nbytes;

    sleep(1);

    while ((nbytes = read(pd, buffer, BUFFER_SIZE)) > 0) //читаем из межпроцессного канала в буфер определенное число символов (не более BUFFER_SIZE)
    {
        str = (char *)realloc(str, str_size + nbytes); //записываем считанные символы в конец имеющейся считанной строки
        memcpy(str + str_size, buffer, nbytes);        //копируем новую строку
        str_size += nbytes;                            //обновляем размер строки
        char *ps = strchr(str, '\n');                  //ищем в строке символ переноса строки, которым оказываются разделены имена файлов
        if (ps)                                        //если такой нашелся
        {
            size_t copy_size = ps - str + 1;                //смотрим на длину имени
            char *new_filename = (char *)malloc(copy_size); //выделяем память на новое имя
            memcpy(new_filename, str, copy_size);           //копируем имя в новую переменную
            new_filename[copy_size - 1] = '\0';             //ставим в конце символ конца строки (заменяем '\n' на '\0')
            add_file(new_filename);                         //добавляем имя в список имен

            str_size -= copy_size;                //уменьшаем длину строки, в которую "читаем" из буфера
            char *tmp = (char *)malloc(str_size); //выделяем новую память нового размера (укороченную)
            memcpy(tmp, ps + 1, str_size);        //копируем туда строку без обработанного имени
            free(str);                            //освобождаем старую память
            str = tmp;                            //прсваиваем новой копии старое имя для устройства работы
        }

        memset(buffer, 0, BUFFER_SIZE); //заполняем буфер нулями
        sleep(1);
        //printf("*resuming*\n");
    }
    close(pd); //закрываем файл вывода
}

void add_file(char *filename) /*функция добавления имени файла в список имен*/
{
    //смотрим текущую дату в своем регионе
    struct tm ctm = *localtime(&(time_t){time(NULL)});
    ctm.tm_hour = 0;
    ctm.tm_min = 0;
    ctm.tm_sec = 0;
    time_t current_date = mktime(&ctm);

    struct stat buff;
    PFile pfile;

    stat(filename, &buff);              //считываем данные о файле в специальную структуру
    off_t filesize = buff.st_size;      //запоминаем размер файла
    time_t last_access = buff.st_atime; //запоминаем время последнего доступа к файлу
    if (last_access > current_date)     //если время последнего доступа попадает в текущую дату
    {
        if ((pfile = (struct FilesList *)malloc(sizeof(struct FilesList))) == NULL)
            exit(EXIT_FAILURE); //выход с ошибкой (если нельзя выделить память)
        //иначе записываем данные в структуру
        pfile->filename = filename;
        pfile->filesize = filesize;
        pfile->last_access = last_access;
        pfile->next = NULL;
        if (head == NULL) //если ранее список был пуст, то прсто добавляем структуру в список
            head = pfile;
        else
        {
            PFile cur = head;
            while ((cur->next != NULL) && cur->last_access > pfile->last_access) //проходим по всем временам доступа, чтобы найти место файла (сортируем)
                cur = cur->next;
            //вставляем структуру на нужное место в списке
            pfile->next = cur->next;
            cur->next = pfile;
        }
    }
}

void interrupt() /*функция, обрабатывающая прерывания*/
{
    static unsigned cnt = 0; //задаем счетчик числа прерываний
    cnt++;                   //увеличиваем счетчик на 1
    if (cnt % 2 == 0)        //если число прерываний кратно двум
    {
        printf("Вывожу список на текущий момент:\n");
        print_list(1); //выводим список на текущий момент
    }
    else
    {
        printf("Продолжаю работу после сигнала прерывания.\n");
    }
}

void print_list(int mode) /*функция, выводящая список файлов на экран*/
{
    PFile cur = head;
    if (head == NULL)
    {
        printf("Список пуст.\n");
    }
    if (mode == 0) //mode=0 выводит только имена файлов
    {
        while (cur)
        {
            printf("%s\n", cur->filename);
            cur = cur->next;
        }
    }
    else //иначе выводим имя и размер файла
    {
        while (cur)
        {
            printf("%s\t%ld\n", cur->filename, cur->filesize);
            cur = cur->next;
        }
    }
}

int main()
{

    //signal(SIGINT, interrupt); //устанавливаем обработчик прерываний
    struct sigaction new_action;
    new_action.sa_handler = interrupt;      //устанавливаем функцию, которая будет обрабатывать сигнал
    sigprocmask(0, 0, &new_action.sa_mask); //устанавливаем сигнальную маску
    //sigemptyset(&new_action.sa_mask);       //выставляем пустую маску
    //sigaddset(&new_action.sa_mask, SIGINT); //добавляем SIGINT, чтобы во время обработки SIGINT не обрабатывать SIGINT второй раз
    new_action.sa_flags = 0;              //флаги выставляем нулевыми
    sigaction(SIGINT, &new_action, NULL); //устанавливаем обработчик сигнала в случае прихода сингала прерывания SIGINT

    int pipe_ds[2]; //задаем дескриптор межпроцессного канала

    if (pipe(pipe_ds)) //открываем канал
    {
        printf("ERROR: cannot open pipe\n");
        exit(EXIT_FAILURE); //выход с ошибкой в случае неудачи
    }

    pid_t pid = fork(); //создание процесса-потомка
    if (pid == -1)      //в случае неудачи
    {
        printf("ERROR: cannot create child process\n");
        exit(EXIT_FAILURE); //выход с ошибкой
    }
    if (pid == 0) //процесс-потомок
    {
        close(pipe_ds[0]); //закрываем канал на чтение
        child(pipe_ds[1]); //выполняем функцию потомка (передаем дескриптор канала на запись потомку)
    }
    else //процесс-родитель
    {
        close(pipe_ds[1]);  //закрываем канал на запись
        parent(pipe_ds[0]); //выполняем функцию родителя (передаем дескриптор канала на чтение родителю)
        print_list(0);      //в конце выполнения выводим весь список файлов
    }
    exit(EXIT_SUCCESS); //завершение программы
}
