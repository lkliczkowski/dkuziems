Program: "Grzybobranie", C, Xlib
Autor programu: Dariusz Kuziemski

===================================================================
Krótki opis programu
===================================================================
Gra planszowa (2-4 graczy) Grzybobranie "urozmaicona" o możliwość wyboru 
kierunku ruchu przed rzutem kostką oraz zapętloną trasą ("Las"), ilość pól
61, z losową konfiguracją gdzie są "Grzyby dobre" i "Muchomory" inicjowaną
przy starcie gry przez serwer. Szansa na wystąpienie "Grzyba" na 
poszczególnych polach wynosi 80%.
- Grzyby dobre: od +1 do +3 punkty
- Muchomory: od -1  do -2 punktów
W czasie gry gracze wybierają tylko "którą kostką chcą rzucać" co determinuje
kierunek ruchu ich "koszyka".

===================================================================
Mechanizm komunikacji
===================================================================
Interfejs w Xlib, ruchy graczy zarządzane są przez serwer za pomocą komunikatów gniazd BSD.


===================================================================
Opis użytkowania programu
===================================================================
W pliku "zasady.h" ustawiamy stałe, które będą obowiązywać:
- IP SERWERA (na których zostanie uruchomiony serwer)
- PORT
oraz 
- ILOSC_GRACZY (od 2-4)
- ILOSC_KOLEJEK

Kompilujemy za pomocą poleceń:
gcc -Wall -g zasady.h serwer_grzybobranie.c -o serwer_grzybobranie
gcc -Wall -g -L/usr/X11R6/lib -lX11 zasady.h klient_grzybobranie.c -o klient_grzybobranie
(do kompilacji można takż ewykorzystać srypt "make.sh" po nadaniu mu
uprawnień do wykonania ("chmod +x make.sh"))

Uruhamiamy "./serwer_grzybobranie" jako aplikacje serwera,
"./klient_grzybobranie" jako klienta gry.


===================================================================
Sytuacje wyjątkowe
===================================================================
- Po stronie Klienta

Sytuacja: Serwer nie został uruchomiony/jest niedostępny
Zachowanie: Otrzymujemy komunikat "connect error: Connection refused"

Sytuacja: Aplikacja serwera zostanie zatrzymana (np. CTRL+C)
Zachowanie: Klient otrzymuje komunikat "recv error: disconnect"

Sytuacja: Nie można się połączyć z Xserwerem
Zachowanie: Otrzymujemy komunikat "cannot connect to X server"


- Po stronie serwera

Sytuacja: W pliku "zasady.h" została podana nieprawidłowa ilość graczy
Zachowanie: Serwer zwróci komunikat "Podano niepoprawna ilosc graczy 
(%d), gra dla 2-4 graczy" i nie rozpocznie działania

Sytuacja: Otrzymanie sygnału SIGINT (wciśnięcie CTRL-C przez użytkownika).
Zachowanie programu: Zamykamy połączenia, gracze otrzymują komunikaty
"recv error: disconnect"

Sytuacja: Więcej użytkowników chce dołaczyć do gry niż przewiduje ILOSC_GRACZY
Zachowanie programu: Serwer nie akceptuje połączeń do czasu zakończenia 
rozpoczętej już gry, aplikacja nadliczbowego klienta oczekuje do zakończenia
aktualnej rozgrywki, dopiero gdy zostanie dopuszczona do nowej gry otrzymuje
informacje, że została zalogowana, dlugosc kolejki oczekujacych na polaczenie jest ustawiona na 5

Sytuacja: Aplikacja jednego z graczy przerwie działanie
Zachowanie programu: Aktualna rozgrywka zostaje zakończona, pozostali gracze
zostają rozłączeni, otrzymują komunikat "recv error: disconnect"
