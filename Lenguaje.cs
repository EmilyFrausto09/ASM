using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace ASM
{
    public class Lenguaje : Sintaxis
    {
        private int ifContador, whileContador, doWhileContador, forContador;
        Stack<float> s;
        List<Variable> l;
        Variable.TipoDato maximoTipo;
        public Lenguaje() : base()
        {
            s = new Stack<float>();
            l = new List<Variable>();
            maximoTipo = Variable.TipoDato.Char;
            log.WriteLine("Constructor lenguaje");
            ifContador = whileContador = doWhileContador = forContador = 1;

        }
        public Lenguaje(string nombre) : base(nombre)
        {
            s = new Stack<float>();
            l = new List<Variable>();
            maximoTipo = Variable.TipoDato.Char;
            log.WriteLine("Constructor lenguaje");
            ifContador = whileContador = doWhileContador = forContador = 1;
        }

        private void displayStack()
        {
            Console.WriteLine("Contenido del stack: ");
            foreach (float elemento in s)
            {
                Console.WriteLine(elemento);
            }
        }

        private void displayLista()
        {
            asm.WriteLine("SECTION .DATA");
            log.WriteLine("Lista de variables: ");
            foreach (Variable elemento in l)
            {
                log.WriteLine($"{elemento.getNombre()} {elemento.GetTipoDato()} {elemento.getValor()}");
                asm.WriteLine($"{elemento.getNombre()} DW 0");
            }
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            // Declaramos cadenas necesarias para E/S en la sección .data
            asm.WriteLine("SECTION .DATA");
            asm.WriteLine("fmt_int_in DB \"%d\", 0");
            asm.WriteLine("fmt_int_out DB \"%d\", 10, 0");
            asm.WriteLine("fmt_str_out DB \"%s\", 10, 0");
            asm.WriteLine("buffer TIMES 256 DB 0");
            asm.WriteLine();
            asm.WriteLine("SECTION .text");
            asm.WriteLine("EXTERN printf");
            asm.WriteLine("EXTERN scanf");
            asm.WriteLine("EXTERN gets");
            asm.WriteLine("GLOBAL _start");
            asm.WriteLine("_start:");

            if (Contenido == "using")
            {
                Librerias();
            }
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables(true); //primer error
            }
            Main();
            asm.WriteLine("\tRET");
            displayLista();
        }
        //Librerias -> using ListaLibrerias; Librerias?

        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (Contenido == "using")
            {
                Librerias();
            }
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?

        private void Variables(bool ejecuta)
        {
            Variable.TipoDato t = Variable.TipoDato.Char;
            switch (Contenido)
            {
                case "int": t = Variable.TipoDato.Int; break;
                case "float": t = Variable.TipoDato.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(ejecuta, t);
            match(";");
            if (Clasificacion == Tipos.TipoDato)
            {
                Variables(ejecuta);
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }
        //ListaIdentificadores -> identificador (= Expresion)? (,ListaIdentificadores)?
        private void ListaIdentificadores(bool ejecuta, Variable.TipoDato t)
        {
            if (l.Find(variable => variable.getNombre() == Contenido) != null)
            {
                throw new Error($"La variable {Contenido} ya existe", log, linea, columna);
            }
            Variable v = new Variable(t, Contenido);
            l.Add(v);
            match(Tipos.Identificador);
            if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
                    {
                        match("Read");
                        asm.WriteLine($"; Lectura de un caracter para {v.getNombre()}");
                        asm.WriteLine("   PUSH EAX");          // Guardar valor actual de EAX
                        asm.WriteLine("   MOV EAX, 3");        // Syscall para read
                        asm.WriteLine("   MOV EBX, 0");        // Stdin
                        asm.WriteLine("   MOV ECX, buffer");   // Buffer
                        asm.WriteLine("   MOV EDX, 1");        // Leer un byte
                        asm.WriteLine("   INT 80h");           // Llamada al sistema
                        asm.WriteLine("   MOV AL, [buffer]");  // Mover el byte leído a AL
                        asm.WriteLine("   MOV BYTE [" + v.getNombre() + "], AL"); // Guardar en variable
                        asm.WriteLine("   POP EAX");           // Restaurar EAX

                        int r = Console.Read();
                        if (maximoTipo > Variable.valorToTipoDato(r))
                        {
                            throw new Error("Tipo Dato. No esta permitido asignar un valor " + maximoTipo + "a una variable " + Variable.valorToTipoDato(r), log, linea, columna);
                        }
                        v.setValor(r);
                    }
                    else
                    {
                        match("ReadLine");
                        asm.WriteLine($"; Lectura de una línea para {v.getNombre()}");
                        asm.WriteLine("   PUSH EAX");          // Guardar registros
                        asm.WriteLine("   PUSH EBX");
                        asm.WriteLine("   PUSH ECX");
                        asm.WriteLine("   PUSH EDX");
                        asm.WriteLine("   PUSH buffer");       // Pasar buffer como parámetro
                        asm.WriteLine("   CALL gets");         // Llamar a gets
                        asm.WriteLine("   ADD ESP, 4");        // Limpiar stack
                        asm.WriteLine("   PUSH fmt_int_in");   // Formato para scanf
                        asm.WriteLine("   PUSH buffer");       // Buffer con entrada
                        asm.WriteLine("   CALL scanf");        // Leer número
                        asm.WriteLine("   ADD ESP, 8");        // Limpiar stack
                        asm.WriteLine("   MOV EAX, [buffer]"); // Valor leído a EAX
                        asm.WriteLine("   MOV [" + v.getNombre() + "], EAX"); // Guardar en variable
                        asm.WriteLine("   POP EDX");           // Restaurar registros
                        asm.WriteLine("   POP ECX");
                        asm.WriteLine("   POP EBX");
                        asm.WriteLine("   POP EAX");

                        string? r = Console.ReadLine();
                        if (float.TryParse(r, out float valor))
                        {
                            if (maximoTipo > Variable.valorToTipoDato(valor))
                            {
                                throw new Error("Tipo Dato. No esta permitido asignar un valor " + maximoTipo + "a una variable " + Variable.valorToTipoDato(valor), log, linea, columna);
                            }
                            v.setValor(valor);
                        }
                        else
                        {
                            throw new Error("Sintaxis. No se ingresó un número ", log, linea, columna);
                        }
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    asm.WriteLine($"; Asignacion de {v.getNombre()}");

                    Expresion();
                    float r = s.Pop();
                    asm.WriteLine("   POP EAX");
                    asm.WriteLine($"    MOV DWORD [{v.getNombre()}], EAX");
                    v.setValor(r, maximoTipo);
                }
            }
            if (Contenido == ",")
            {
                match(",");
                ListaIdentificadores(ejecuta, t);
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones(bool ejecuta)
        {
            match("{");
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);
            if (Contenido != "}")
            {
                ListaInstrucciones(ejecuta);
            }
            else
            {
                match("}");
            }
        }

        //Instruccion -> console | If | While | do | For | Variables | Asignación
        private void Instruccion(bool ejecuta)
        {
            if (Contenido == "Console")
            {
                console(ejecuta);
            }
            else if (Contenido == "if")
            {
                If(ejecuta);
            }
            else if (Contenido == "while")
            {
                While(ejecuta);
            }
            else if (Contenido == "do")
            {
                Do(ejecuta);
            }
            else if (Contenido == "for")
            {
                For(ejecuta);
            }
            else if (Clasificacion == Tipos.TipoDato)
            {
                Variables(ejecuta);
            }
            else
            {
                Asignacion(ejecuta);
                match(";");
            }
        }
        //Asignacion -> Identificador = Expresion; (DONE)
        /*
        Id++ (DONE)
        Id-- (DONE)
        Id IncrementoTermino Expresion (DONE)
        Id IncrementoFactor Expresion (DONE)
        Id = Console.Read() (DONE)
        Id = Console.ReadLine() (DONE)
        */
        private void Asignacion(bool ejecuta)
        {
            // Cada vez que haya una asignación, reiniciar el maximoTipo
            maximoTipo = Variable.TipoDato.Char;
            float r;
            Variable? v = l.Find(variable => variable.getNombre() == Contenido);
            if (v == null)
            {
                throw new Error("Sintaxis: La variable " + Contenido + " no está definida", log, linea, columna);
            }
            match(Tipos.Identificador);

            if (Contenido == "++")
            {
                match("++");
                r = v.getValor() + 1;
                asm.WriteLine($"   ADD [{v.getNombre()}], 1");//aqui
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "--")
            {
                match("--");
                r = v.getValor() - 1;
                asm.WriteLine($"   SUB [{v.getNombre()}], 1");//aqui
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "=")
            {
                match("=");

                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
                    {
                        match("Read");
                        asm.WriteLine($"; Lectura de un caracter para {v.getNombre()}");
                        asm.WriteLine("   PUSH EAX");          // Guardar valor actual de EAX
                        asm.WriteLine("   MOV EAX, 3");        // Syscall para read
                        asm.WriteLine("   MOV EBX, 0");        // Stdin
                        asm.WriteLine("   MOV ECX, buffer");   // Buffer
                        asm.WriteLine("   MOV EDX, 1");        // Leer un byte
                        asm.WriteLine("   INT 80h");           // Llamada al sistema
                        asm.WriteLine("   MOV AL, [buffer]");  // Mover el byte leído a AL
                        asm.WriteLine("   MOV BYTE [" + v.getNombre() + "], AL"); // Guardar en variable
                        asm.WriteLine("   POP EAX");           // Restaurar EAX
                        
                        int res = Console.Read();
                        if (maximoTipo > Variable.valorToTipoDato(res))
                        {
                            throw new Error("Tipo Dato. No esta permitido asignar un valor " + maximoTipo + "a una variable " + Variable.valorToTipoDato(res), log, linea, columna);
                        }
                        v.setValor(res);
                    }
                    else
                    {
                        match("ReadLine");
                        asm.WriteLine($"; Lectura de una línea para {v.getNombre()}");
                        asm.WriteLine("   PUSH EAX");          // Guardar registros
                        asm.WriteLine("   PUSH EBX");
                        asm.WriteLine("   PUSH ECX");
                        asm.WriteLine("   PUSH EDX");
                        asm.WriteLine("   PUSH buffer");       // Pasar buffer como parámetro
                        asm.WriteLine("   CALL gets");         // Llamar a gets
                        asm.WriteLine("   ADD ESP, 4");        // Limpiar stack
                        asm.WriteLine("   PUSH fmt_int_in");   // Formato para scanf
                        asm.WriteLine("   PUSH buffer");       // Buffer con entrada
                        asm.WriteLine("   CALL scanf");        // Leer número
                        asm.WriteLine("   ADD ESP, 8");        // Limpiar stack
                        asm.WriteLine("   MOV EAX, [buffer]"); // Valor leído a EAX
                        asm.WriteLine("   MOV [" + v.getNombre() + "], EAX"); // Guardar en variable
                        asm.WriteLine("   POP EDX");           // Restaurar registros
                        asm.WriteLine("   POP ECX");
                        asm.WriteLine("   POP EBX");
                        asm.WriteLine("   POP EAX");
                        
                        string? res = Console.ReadLine();
                        if (float.TryParse(res, out float valor))
                        {
                            if (maximoTipo > Variable.valorToTipoDato(valor))
                            {
                                throw new Error("Tipo Dato. No esta permitido asignar un valor " + maximoTipo + "a una variable " + Variable.valorToTipoDato(valor), log, linea, columna);
                            }
                            v.setValor(valor);
                        }
                        else
                        {
                            throw new Error("Sintaxis. No se ingresó un número ", log, linea, columna);
                        }
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    asm.WriteLine($"; Asignacion de {v.getNombre()}");

                    Expresion();
                    r = s.Pop();
                    asm.WriteLine("   POP EAX");
                    asm.WriteLine($"    MOV DWORD [{v.getNombre()}], EAX");
                    v.setValor(r, maximoTipo);

                    if (ejecuta)
                    {
                        v.setValor(r, maximoTipo); //3 error corregito
                    }
                }
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                r = v.getValor() + s.Pop();
                asm.WriteLine("   POP EAX");
                asm.WriteLine($"   ADD [{v.getNombre()}], EAX");//aqui
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                r = v.getValor() - s.Pop();
                asm.WriteLine("   POP EAX");
                asm.WriteLine($"   SUB [{v.getNombre()}], EAX");//aqui
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                r = v.getValor() * s.Pop();
                asm.WriteLine("   POP EBX");
                asm.WriteLine($"   MOV EAX, [{v.getNombre()}]");
                asm.WriteLine($"   MUL EBX");
                asm.WriteLine($"   MOV [{v.getNombre()}], EAX");//aqui
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                r = v.getValor() / s.Pop();
                asm.WriteLine("   POP EBX");
                asm.WriteLine($"   MOV EAX, [{v.getNombre()}]");
                asm.WriteLine($"   DIV EBX");
                asm.WriteLine($"   MOV [{v.getNombre()}],EAX");//aqui
                v.setValor(r, maximoTipo);
            }
            else if (Contenido == "%=")
            {
                match("%=");
                Expresion();
                r = v.getValor() % s.Pop();
                asm.WriteLine("   POP EBX");
                asm.WriteLine($"   MOV EAX, [{v.getNombre()}]");
                asm.WriteLine($"   DIV EBX");
                asm.WriteLine($"   MOV [{v.getNombre()}],EDX");//aqui
                v.setValor(r, maximoTipo);
            }
            else
            {
                match("ReadLine");
                string? read = Console.ReadLine();
                float result;

                if (float.TryParse(read, out result))
                {
                    if (ejecuta)
                    {
                        v.setValor(result, maximoTipo);
                    }
                }
                else
                {
                    throw new Error("Sintaxis: sólo se pueden ingresar números", log, linea, columna);
                }
            }
        }
        /*If -> if (Condicion) bloqueInstrucciones | instruccion
        (else bloqueInstrucciones | instruccion)?*/
        private void If(bool ejecuta2)
        {
            match("if");
            match("(");

            asm.WriteLine("; If");
            string etiqueta = $"brinco_if_{ifContador++}";

            bool ejecuta = Condicion(etiqueta) && ejecuta2;

            match(")");

            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }

            asm.WriteLine($"{etiqueta}");

            if (Contenido == "else")
            {
                match("else");
                bool ejecutarElse = ejecuta2 && !ejecuta; // Solo se ejecuta el else si el if no se ejecutó
                if (Contenido == "{")
                {
                    BloqueInstrucciones(ejecutarElse);
                }
                else
                {
                    Instruccion(ejecutarElse);
                }
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion(string etiqueta, bool esDo = false)
        {
            maximoTipo = Variable.TipoDato.Char;
            Expresion();
            float valor1 = s.Pop();
            string operador = Contenido;
            match(Tipos.OperadorRelacional);
            maximoTipo = Variable.TipoDato.Char;
            Expresion();
            float valor2 = s.Pop();
            asm.WriteLine("   POP EBX");
            asm.WriteLine("   POP EAX");
            asm.WriteLine("   CMP EAX, EBX");

            if (esDo)
            {
                switch (operador) //cambiar el switch dejandolo como estan los casos
                {
                    case ">":
                        asm.WriteLine($"\tJNA {etiqueta}"); // <=
                        return valor1 > valor2;
                    case ">=":
                        asm.WriteLine($"\tJB {etiqueta}"); // <
                        return valor1 >= valor2;
                    case "<":
                        asm.WriteLine($"\tJAE {etiqueta}"); // >=
                        return valor1 < valor2;
                    case "<=":
                        asm.WriteLine($"\tJA {etiqueta}"); // >
                        return valor1 <= valor2;
                    case "==":
                        asm.WriteLine($"\tJNE {etiqueta}"); // !=
                        return valor1 == valor2;
                    default:
                        asm.WriteLine($"\tJE {etiqueta}"); // ==
                        return valor1 != valor2;
                }
            }
            else
            {
                switch (operador)
                {
                    case ">":
                        asm.WriteLine($"\tJNA {etiqueta}"); // <=
                        return valor1 > valor2;
                    case ">=":
                        asm.WriteLine($"\tJB {etiqueta}"); // <
                        return valor1 >= valor2;
                    case "<":
                        asm.WriteLine($"\tJAE {etiqueta}"); // >=
                        return valor1 < valor2;
                    case "<=":
                        asm.WriteLine($"\tJA {etiqueta}"); // >
                        return valor1 <= valor2;
                    case "==":
                        asm.WriteLine($"\tJNE {etiqueta}"); // !=
                        return valor1 == valor2;
                    default:
                        asm.WriteLine($"\tJE {etiqueta}"); // ==
                        return valor1 != valor2;
                }
            }
        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool ejecuta)
        {
            match("while");
            match("(");
            //Cambiar esto y en todos los ciclos que llevan una condicion
            Condicion("");
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
        }
        /*Do -> do bloqueInstrucciones | intruccion 
        while(Condicion);*/
        private void Do(bool ejecuta)
        {
            match("do");
            asm.WriteLine("\t; Do");
            string etiqueta = $"brinco_do_{doWhileContador++}";
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
            match("while");
            match("(");
            Condicion(etiqueta);
            match(")");
            match(";");
        }
        /*For -> for(Asignacion; Condicion; Asignacion) 
        BloqueInstrucciones | Intruccion*/
        private void For(bool ejecuta)
        {
            string etiqueta = ""; // hacer un jump_for 
            match("for");
            match("(");
            Asignacion(ejecuta);
            match(";");
            Condicion(etiqueta);
            match(";");
            Asignacion(ejecuta);
            match(")");
            if (Contenido == "{")
            {
                BloqueInstrucciones(ejecuta);
            }
            else
            {
                Instruccion(ejecuta);
            }
        }
        //Console -> Console.(WriteLine|Write) (cadena? concatenaciones?);
       private void console(bool ejecuta)
{
    bool isWriteLine = false;
    match("Console");
    match(".");
    if (Contenido == "WriteLine")
    {
        match("WriteLine");
        isWriteLine = true;
        asm.WriteLine("; Console.WriteLine");
    }
    else
    {
        match("Write");
        asm.WriteLine("; Console.Write");
    }
    match("(");
    string tempStrLabel = "temp_str_" + new Random().Next(1000, 9999);
    
    if (Clasificacion == Tipos.Cadena)
    {
        string contenido = Contenido.Trim('"').Replace("\"", "\\\"").Replace("\n", "', 10, '");
        asm.WriteLine($"{tempStrLabel} DB '{contenido}', 0");
        match(Tipos.Cadena);
    }
    else
    {
        asm.WriteLine($"{tempStrLabel} DB ' ', 0"); // Evitar DB vacío
    }
    
    asm.WriteLine(" PUSH EAX");
    asm.WriteLine(" PUSH fmt_str_out");
    asm.WriteLine($" PUSH {tempStrLabel}");
    asm.WriteLine(" CALL printf");
    asm.WriteLine(" ADD ESP, 8");
    asm.WriteLine(" POP EAX");
    
    match(")");
    match(";");
    
    if (ejecuta)
    {
        if (isWriteLine)
            Console.WriteLine(tempStrLabel);
        else
            Console.Write(tempStrLabel);
    }
}

private void ReadConsole(Variable v)
{
    asm.WriteLine($"; Lectura de un caracter para {v.getNombre()}");
    asm.WriteLine(" PUSH EAX");
    asm.WriteLine(" MOV EAX, 3");
    asm.WriteLine(" MOV EBX, 0");
    asm.WriteLine(" MOV ECX, buffer");
    asm.WriteLine(" MOV EDX, 1");
    asm.WriteLine(" INT 80h");
    asm.WriteLine(" MOV AL, [buffer]");
    asm.WriteLine($" MOV BYTE [{v.getNombre()}], AL");
    asm.WriteLine(" POP EAX");
}

private void ReadLineConsole(Variable v)
{
    asm.WriteLine($"; Lectura de una línea para {v.getNombre()}");
    asm.WriteLine(" PUSH EAX");
    asm.WriteLine(" PUSH buffer");
    asm.WriteLine(" CALL gets");
    asm.WriteLine(" ADD ESP, 4");
    asm.WriteLine(" PUSH fmt_int_in");
    asm.WriteLine(" PUSH buffer");
    asm.WriteLine(" CALL scanf");
    asm.WriteLine(" ADD ESP, 8");
    asm.WriteLine(" MOV EAX, [buffer]");
    asm.WriteLine($" MOV [{v.getNombre()}], EAX");
    asm.WriteLine(" POP EAX");
}

        // Concatenaciones -> Identificador|Cadena ( + concatenaciones )?
        private string Concatenaciones()
        {
            string resultado = "";
            if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == Contenido);
                if (v != null)
                {
                    resultado = v.getValor().ToString(); // Obtener el valor de la variable y convertirla
                    
                    // Código para imprimir el valor de la variable
                    asm.WriteLine($"; Impresión del valor de {v.getNombre()}");
                    asm.WriteLine("   PUSH EAX");
                    asm.WriteLine("   PUSH fmt_int_out");  // Formato para enteros
                    asm.WriteLine($"   PUSH DWORD [{v.getNombre()}]");
                    asm.WriteLine("   CALL printf");
                    asm.WriteLine("   ADD ESP, 8");
                    asm.WriteLine("   POP EAX");
                }
                else
                {
                    throw new Error("La variable " + Contenido + " no está definida", log, linea, columna);
                }
                match(Tipos.Identificador);
            }
            else if (Clasificacion == Tipos.Cadena)
            {
                resultado = Contenido.Trim('"');
                // Para cadenas, generamos un label temporal y lo imprimimos
                string tempLabel = $"str_{new Random().Next(1000, 9999)}";
                string contenido = resultado.Replace("\"", "\\\"").Replace("\n", "', 10, '");
                
                asm.WriteLine($"{tempLabel} DB '{contenido}', 0");
                asm.WriteLine("   PUSH EAX");
                asm.WriteLine("   PUSH fmt_str_out");
                asm.WriteLine($"   PUSH {tempLabel}");
                asm.WriteLine("   CALL printf");
                asm.WriteLine("   ADD ESP, 8");
                asm.WriteLine("   POP EAX");
                
                match(Tipos.Cadena);
            }
            if (Contenido == "+")
            {
                match("+");
                resultado += Concatenaciones();  // // Acumula el siguiente fragmento de concatenación
            }
            return resultado;
        }
        //Main -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            BloqueInstrucciones(true);
        }
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (Clasificacion == Tipos.OperadorTermino)
            {
                string operador = Contenido;
                match(Tipos.OperadorTermino);
                Termino();
                //Console.Write(operador + " ");
                float n1 = s.Pop();
                asm.WriteLine("   POP EBX");
                float n2 = s.Pop();
                asm.WriteLine("   POP EAX");
                switch (operador)
                {
                    case "+":
                        s.Push(n2 + n1);
                        asm.WriteLine("   ADD EAX,EBX");
                        asm.WriteLine("   PUSH EAX");
                        break;
                    case "-":
                        s.Push(n2 - n1);
                        asm.WriteLine("   SUB EAX,EBX");
                        asm.WriteLine("   PUSH EAX");
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (Clasificacion == Tipos.OperadorFactor)
            {
                string operador = Contenido;
                match(Tipos.OperadorFactor);
                Factor();
                //Console.Write(operador + " ");
                float n1 = s.Pop();
                asm.WriteLine("   POP EBX");
                float n2 = s.Pop();
                asm.WriteLine("   POP EAX");
                switch (operador)
                {
                    case "*":
                        s.Push(n2 * n1);
                        asm.WriteLine("   MUL EBX");
                        asm.WriteLine("   PUSH AX");
                        break;
                    case "/":
                        s.Push(n2 / n1);
                        asm.WriteLine("   DIV EBX");
                        asm.WriteLine("   PUSH EAX");
                        break;
                    case "%":
                        s.Push(n2 % n1);
                        asm.WriteLine("   DIV EBX");
                        asm.WriteLine("   PUSH EBX");
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
                //Si el tipo de dato del número es mayor al tipo de dato actual, cambiarlo
                if (maximoTipo < Variable.valorToTipoDato(float.Parse(Contenido)))
                {
                    maximoTipo = Variable.valorToTipoDato(float.Parse(Contenido));
                }
                s.Push(float.Parse(Contenido));
                asm.WriteLine("   MOV EAX, " + Contenido);
                asm.WriteLine("   PUSH EAX");
                //Console.Write(Contenido + " ");
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                Variable? v = l.Find(variable => variable.getNombre() == Contenido);
                if (v == null)
                {
                    throw new Error("Sintaxis: la variable " + Contenido + " no está definida", log, linea, columna);
                }
                if (maximoTipo < v.GetTipoDato())
                {
                    maximoTipo = v.GetTipoDato();
                }
                s.Push(v.getValor());
                asm.WriteLine("    MOV EAX, " + Contenido);
                asm.WriteLine("    PUSH EAX");
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Variable.TipoDato tipoCasteo = Variable.TipoDato.Char;
                bool huboCasteo = false;
                if (Clasificacion == Tipos.TipoDato)
                {
                    switch (Contenido)
                    {
                        case "int": tipoCasteo = Variable.TipoDato.Int; break;
                        case "float": tipoCasteo = Variable.TipoDato.Float; break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                    huboCasteo = true;
                }
                Expresion();
                if (huboCasteo)
                {
                    maximoTipo = tipoCasteo;
                    float r = s.Pop();
                    asm.WriteLine("   POP");
                    switch (tipoCasteo)
                    {
                        case Variable.TipoDato.Int: r = (r % 65536); break;
                        case Variable.TipoDato.Char: r = (r % 256); break;
                    }
                    s.Push(r);
                    asm.WriteLine("   PUSH");
                }
                match(")");
            }
        }
        /*SNT = Producciones = Invocar el metodo
        ST  = Tokens (Contenido | Classification) = Invocar match    Variables -> tipo_dato Lista_identificadores; Variables?*/
    }
}