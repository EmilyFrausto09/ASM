; Archivo: prueba.cpp
; Fecha y hora: 10/03/2025 10:41:18 a. m.
; ----------------------------------
SEGMENT .TEXT
GLOBAL MAIN
MAIN:
SECTION .DATA
fmt_int_in DB "%d", 0
fmt_int_out DB "%d", 10, 0
fmt_str_out DB "%s", 10, 0
buffer TIMES 256 DB 0

SECTION .text
EXTERN printf
EXTERN scanf
EXTERN gets
GLOBAL _start
_start:
; Asignacion de x
   MOV EAX, 0
   PUSH EAX
   POP EAX
    MOV DWORD [x], EAX
; Asignacion de y
   MOV EAX, 10
   PUSH EAX
   POP EAX
    MOV DWORD [y], EAX
; Asignacion de z
   MOV EAX, 2
   PUSH EAX
   POP EAX
    MOV DWORD [z], EAX
; Asignacion de c
   MOV EAX, 100
   PUSH EAX
   MOV EAX, 200
   PUSH EAX
   POP EBX
   POP EAX
   ADD EAX,EBX
   PUSH EAX
   POP
   PUSH
   POP EAX
    MOV DWORD [c], EAX
; Asignacion de c
   MOV EAX, 100
   PUSH EAX
   MOV EAX, 200
   PUSH EAX
   POP EBX
   POP EAX
   ADD EAX,EBX
   PUSH EAX
   POP
   PUSH
   POP EAX
    MOV DWORD [c], EAX
; Console.WriteLine
temp_str_8638 DB 'Valor de altura = ', 0
 PUSH EAX
 PUSH fmt_str_out
 PUSH temp_str_8638
 CALL printf
 ADD ESP, 8
 POP EAX
; Lectura de una línea para altura
   PUSH EAX
   PUSH EBX
   PUSH ECX
   PUSH EDX
   PUSH buffer
   CALL gets
   ADD ESP, 4
   PUSH fmt_int_in
   PUSH buffer
   CALL scanf
   ADD ESP, 8
   MOV EAX, [buffer]
   MOV [altura], EAX
   POP EDX
   POP ECX
   POP EBX
   POP EAX
