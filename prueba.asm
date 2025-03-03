; Archivo: prueba.cpp
; Fecha y hora: 03/03/2025 11:39:38 a. m.
; ----------------------------------
SEGMENT .TEXT
GLOBAL MAIN
MAIN:
; Asignacion de x26
   MOV EAX, 200
   PUSH EAX
   POP EAX
    MOV DWORD [x26], EAX
	; Do
; Asignacion de x26
    MOV EAX, x26
    PUSH EAX
   MOV EAX, 1
   PUSH EAX
   POP EBX
   POP EAX
   ADD EAX,EBX
   PUSH EAX
   POP EAX
    MOV DWORD [x26], EAX
    MOV EAX, x26
    PUSH EAX
   MOV EAX, 211
   PUSH EAX
   POP EBX
   POP EAX
   CMP EAX, EBX
	JAE brinco_do_1
	RET
SECTION .DATA
x26 DW 0
