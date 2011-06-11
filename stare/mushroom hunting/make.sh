gcc -Wall -g zasady.h serwer_grzybobranie.c -o serwer_grzybobranie
gcc -Wall -g -L/usr/X11R6/lib -lX11 zasady.h klient_grzybobranie.c -o klient_grzybobranie