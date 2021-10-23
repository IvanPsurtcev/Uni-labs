#include <stdio.h>
#include <math.h>

int main()
{
  int x,y;
  double z,a,b;
  double a_num, a_deg;// Типы переменных

  printf("Введите x: ");
  scanf("%d",&x);
  printf("Введите y: ");
  scanf("%d",&y);
  printf("Введите z: ");
  scanf("%lf",&z);
  printf("x = %10d\ny = %10d\nz = %10.5E\n",x,y,z);// ввод и вывод переменных

  a_num = (1+pow(x, 1.0/3))*pow(z,2);// числитель a
  a_deg = log(sin(6*pow(y,2))+9) / log(2);// знаменатель a
  a = a_num/a_deg;// число a
  b = (pow(cos(x),2)-exp(y + 2));

  printf("a = %.4lf, b = %.4lf", a,b);// вывод a и b
  printf("\nx (oct) = %o, y (hex) = %x",x,y);// запись чисел в восьмеричной и шестнадцатеричной форме
  return 0;

}
