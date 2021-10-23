#include <stdio.h>
#include <string.h>
#include <stdlib.h>


void f(char* s){
    int i = 0;
    int what=1;
    while(what==1){
        char a[13];
        int j = 0;
        while(s[i]!=' ' && s[i]!='.'){
            a[j]=s[i];
            i++;
            j++;
        }
        if(j%2==1){
            int k = j/2;
            for(int h = 0; h < j; h++){
                if(h!=k){
                    printf("%c",a[h]);
                }
            }
            printf(" ");
        }
        if(s[i] == '.'){
            what=0;
        }
        while(s[i]==' '){
            i++;
        }
    }
}

void check(char* str){
    int i = 0;
    int len = strlen(str);//нахождения длины строки для проверки ввода
    
    for(i=0;i<len;i++){
        if(((str[i]>=65)&&(str[i]<=90))||((str[i]>=97)&&(str[i]<=122))||(str[i]==' ')||(str[i]=='.')){}
        else{
            puts("У вас есть запрещенные символы");
            exit(0);
        }
    }
    if(str[len-1]!='.'){
        printf("Вы не закончили строку точкой, программа завершает работу");//вывод ошибки
        exit(0);//конец программы
    }
    char str1[310];//
    strcpy(str1, str);
    char * pch;//переменная слова
    char word[30][10];//массив слов
    pch =strtok(str1," .");//разделение массива на слова
    while(pch!=NULL){
        if(strlen(pch)>10)//условие правильности слова
        {
            printf("У вас слова слишком большие");//вывод ошибки
            exit(0);//заканчивание программы
        }
        strcpy(word[i++],pch);//копирование слов в массив слов
        pch =strtok (NULL," .");//переход к следующему слову
    }
}

int main()

{
char s[400];
printf("Введите строку : ");//вывод условия

gets(s); //считывание строки
check(s);
f(s);
return 0;

}
