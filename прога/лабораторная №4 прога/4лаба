#include <stdio.h>
#include <math.h>
#include <stdlib.h>//библиотека для завершения программы в функции
#define K 100

double max(double* array, int size, int Y) {//Функция для рассчитывания max
    int i;//объявление переменных типа int
    double dmax = -100000,sum=0;//объявление переменных типа double

    for(i = 0; i < size; i++) {//цикл для определения максимального элемента 
        if(array[i] >= -Y && array[i] <= Y && array[i] > dmax)//условие для нахождения max
        dmax = array[i];//обновление max
    }
    return dmax;// возвращение суммы
    }
     

double srednee(double* array, int size) {//Функция для рассчитывания суммы
    int i, bolmax_i;//объявление переменных типа int
    double m = -100000,sum=0,sredne;//объявление переменных типа double
    for(i = 0; i < size; i++) {
        if(array[i] < 0 && array[i] > m) {//условие для нахождения максимального
            m = array[i];//обновление максимального
            bolmax_i = i;//запоминание порядка максимального
        }
    }
    if(m == -100000) return 0;
    if(bolmax_i == size - 1) return 0;
    for(i = bolmax_i+1; i < size; i++) {//цикл для нахождения суммы
        sum += array[i];//суммирование
    }
    sredne = sum / (size-(bolmax_i+1));//рассчитывание среднего
    return sredne;// возвращение суммы
} 

void print_array(double* array, int size) {//функция для считывания массива
    int i;//объявление переменных типа int
    for(i = 0; i < size; i++) {//цикл для считывания
        printf("Input %d-th elemet:", i+1);//вывод номера элемента который нужно считать
        scanf("%lf", &array[i]);//считывание элемента
    }
}

int main() {//основная функция
    int k,Y;//объявление переменных типа int
    double arr[K], res1, res2;//объявление переменных типа double
    do{
        printf("Input size of array ( < %d, > 1):", K);//вывод указания ввода значения массива
        scanf("%d", &k);//ввод значения массива
    }while(k <= 1 || k > K);//условие существования массива
    
    printf("Input Y :");//вывод указания ввода значения Y
    scanf("%d", &Y);//ввод Y
    
    print_array(arr, k);//считывание массива
    res1 = max(arr, k, Y);//рассчитывание суммы
    if(res1 == -100000) printf("no elements\n");
    else printf("max = %lf\n", res1);//вывод max
    res2 = srednee(arr, k);//рассчитывание среднего
    printf("srednee = %lf", res2);//вывод среднего
    return 0;//завершение прогаммы
}

