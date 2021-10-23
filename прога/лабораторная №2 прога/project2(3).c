#include <stdio.h>
#include <math.h>
int ivan(double x, double y)
{
   return((y>=0)&&(pow(x,2)+pow(y,2)<=4)&&(pow(x,2)+pow(y,2)>=1));//логическое выражение
}
int main(void)
{
     double x, y;
    printf("Введите абсциссу точки: ");
    scanf("%lf", &x);
    printf("Введите ординату точки: ");
    scanf("%lf", &y);
    //Ввод координаты точек
    
    if(ivan(x,y))
    {
        printf("Данная точка принадлежит заштрихованной области");
    }
    else
    {
        printf("Данная точка не принадлежит заштрихованной области");
    }
   
}
