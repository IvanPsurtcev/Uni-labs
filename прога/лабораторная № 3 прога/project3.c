#include "math.h"
#include "stdio.h"
int main()
{
 double X, Eps, S=0.0;
 double D=-1;
 int a, N, i=0;
 double R;
 printf("Введи аргумент X от 0 до 2: ");
 scanf("%lf",&X);
 //R=X;//Первый член ряда.
 if(X>2.0 || X<0.0)
 {
 printf("Вы не правильно ввели значение ");
 return 0;
 }
 printf("С заданной точностью введи 1, если для заданного количества ряда введи 2:\n");
 scanf("%d",&a);
 if(a==1){
      N=0;// - номера члена ряда
      printf("Введи погрешность Eps: ");
      scanf("%lf",&Eps);
     
     while (fabs(D)>=Eps)
     {
     
     N=N+1; //Увеличение номера члена ряда.
     D=D*(double)(1.0-X); //Рекуррентная формула вычисления
     //числителя члена ряда.
     D=D/(double)(N); //Вычисление N-го члена ряда.
     //R = D;
     S=S+D;//Включение очередного члена ряда в сумму
     D=D*N;
     }
     printf(" S = %lf\n",S);
     //приближенное значение функции ln(X),
    return 0;
  }
  else if(a==2)
  { printf("Введи количество членов ряда:");
  scanf("%d",&N);
     while(i<N)
     {
         D=D*(1-X)/(i+1);
         S+=D;
         D = D*(i+1);
         i++;
     } 
     printf(" S = %lf\n",S);
  }
  else{
  printf("Такого нет варианта!");
  return 0;
  }}
