using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

static void Main(string[] args)
{
    int altura,i,j;
    char x=0, y=10, z=2;
    char c;

    c = (char) (100+200);
    c = (char) (100+200);

    Console.WriteLine("Valor de altura = ");
    altura = Console.ReadLine();

    //para que de 61 la altura debe ser igual a 5
    x = (char)((3 + altura) * 8 - (10 - 4) / 2); // = 61
    x--;
    x+=(char) (altura*8);
    x*=2;
    x/=(y-6);

    for (i = 1; i<=altura; i++)
    {
        for (j = 1; j<=i; j++)
        {
            if (j%2==0)
                Console.Write("*");
            else
                Console.Write("-");
        }
        Console.WriteLine("");
    }
    i = 0;
    do
    {
        Console.Write("-");
        i++;
    }
    while (i<altura*2);
    Console.WriteLine("");
    for (i = 1; i<=altura; i++)
    {
        j = 1;
        while (j<=i)
        { 
            Console.Write(""+j);
            j++;
        }
        Console.WriteLine("");
    }
    i = 0;
    do
    {
        Console.Write("-");
        i++;
    }
    while (i<altura*2);
    Console.WriteLine("");
}