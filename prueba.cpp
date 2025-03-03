using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

static void Main(string[] args)
{
  int x26 = 200;

  do {
    x26 = x26 + 1;
  } while(x26 < 211);

  /*if(1 > 2) {
    x26 = 100;
  }

  if(1 >= 2) {
    x26 = 100;
  }

  if(2 < 1) {
    x26 = 100;
  }

  if(2 <= 1) {
    x26 = 100;
  }

  if(1 == 2) {
    x26 = 100;
  }

  if(1 != 2) {
    x26 = 300;
  }*/

  /*if((3 + 5) * 8 - (10 - 4) / 2 == 61) {
    if(1 == 2) {
      x26 = 100;
    }
  }*/

  /*char a = 9; //a = 9
  a *= 1 + 2 + 4 + 3; // a = 90
  a /= 5 + 30 - 3 * 2 + 1; // a = 3
  a++;
  a %= 1 + 2 + 1 - 1; // a = 1*/

  //float b;
  /*
  //PRUEBAS CON CHAR
  char a;

  //char + int = int -> Error
  a = 1;
  int b = 2;
  a = a + b; //Error
  a = (char) (a + b); //Bien

  //char + float = float -> Error
  a = 1;
  float c = 3;
  a = a + c; //Error
  a = (int) (a + c); //Error
  a = (char) (a + c); //Bien

  //char + char = char -> Bien
  a = 1;
  char d = 4;
  a = a + d;

  //PRUEBAS CON INT
  int e = 256;

  //int + char = int -> Bien
  char f = 2;
  e = e + f; //Bien

  //int + float = float -> Error
  e = 256;
  float g = 5;
  e = e + g; // Error
  e = (int) (e + g) // Bien

  //int + int = int -> Bien
  e = 256;
  int h = 2;
  e = e + h; //Bien
  e = (char) (e + h) // Bien
  */

  /*if(c != 5) {
    Console.WriteLine("La suma es distinta de 5");

    if(a == 7) {
      Console.WriteLine("La variable a es igual a 7");
    } else {
      Console.WriteLine("Else ejecutado");
    }
  } else {
    Console.WriteLine("La suma de a + b = " + c);
  }*/

  /*int b = Console.ReadLine();
  int a = (3 + 5) * 8 - (10 - 2*b) / b; // 61
  a--;
  a+=40;
  a*=2;
  a--;
  a-=99;

  int n = 5;

  for(b = 100; a < n; a++) {
    b++;
    while( b != 5 ) {
      if(n == 5) {
        Console.WriteLine("5 es igual a " + n + " " + a + " " + b );
      } else {
        Console.WriteLine("5 es diferente a " + n);
      }
    }
  }

  if(a % 2 != 0) {
    Console.WriteLine("Es impar " + a);
    if(b == 2) {
      Console.WriteLine("b es igual a 2");
    } else if( b > 3) {
      Console.WriteLine("b es mayor a 3 y vale " + b);
    }
    else {
      Console.WriteLine("b no es igual a 2 y vale " + b);
    }
  } else {
    Console.WriteLine("Es impar");
  }*/
}