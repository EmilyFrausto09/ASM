using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ASM
{
    public class Variable
    {
        public enum TipoDato
        {
            Char, Int, Float
        }
        TipoDato tipo;
        string nombre;
        float valor;
        public Variable(TipoDato tipo, string nombre, float valor = 0)
        {
            this.tipo = tipo;
            this.nombre = nombre;
            this.valor = valor;
        }
        
        // Método original
        public void setValor(float valor, StreamWriter log, int fila, int columna){
            if (valorToTipoDato(valor) <= tipo){
                this.valor = valor;    
            } else {
                throw new Error("Semántico: no se puede asignar un " + valorToTipoDato(valor) + " a un " + tipo, log, fila, columna);
            }
        }
        
        // Sobrecarga para solo valor
        public void setValor(float valor){
            this.valor = valor;
        }
        
        // Sobrecarga para valor y tipo
        public void setValor(float valor, TipoDato maximoTipo){
            if (maximoTipo <= tipo){
                this.valor = valor;    
            } else {
                // Aquí no podemos lanzar una excepción con log, fila y columna
                // porque no tenemos esos datos, así que lanzamos una excepción genérica
                throw new Exception("Semántico: no se puede asignar un " + maximoTipo + " a un " + tipo);
            }
        }
        
        public static TipoDato valorToTipoDato(float valor){
            if(!float.IsInteger(valor)){
                return TipoDato.Float;
            } else if(valor <= 255){
                return TipoDato.Char;
            } else if(valor <= (65535)){
                return TipoDato.Int;
            } else {
                return TipoDato.Float;
            }
        }
        
        public float getValor()
        {
            return valor;
        }
        
        public string getNombre()
        {
            return nombre;
        }
        
        public TipoDato GetTipoDato(){
            return tipo;
        }
    }
}