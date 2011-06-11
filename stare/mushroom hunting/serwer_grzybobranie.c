#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <errno.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <signal.h>
#include <time.h>

#include "zasady.h"
#define LICZBA_POL 61

#define LABEL_LEN 10

typedef int bool_t;
int id;

void rzut_minus(int rzut);
void rzut_plus(int rzut);
void draw_everything();
void inicjuj_pola();
void znaleziono(int nagroda);
void kostki();
void sprzatanie(int sig);
void send_to_all(int e);
void zakoncz_gre();
void close_all_cid();

struct wspolrzedne
{
  int x, y;
};

struct DaneGraczy
{
  int numer, pozycja, punkty;
  char label[LABEL_LEN];
};

struct Nagrody
{
  bool_t czy_jest;
  int nagroda;
};


/************************
type  =  0; //connect
	     1; //wait for your turn
	     2; //your turn
	     4; //disconnect
************************/
/**komunikaty serwera*/
typedef struct _msg {
  int type;
  union {
    int id;
    int liczba_oczek;
  };
} gm;

/**komunikaty serwera*/
typedef struct _pola {
   int type;
   struct DaneGraczy gracze[ILOSC_GRACZY+1];
   struct Nagrody pola[LICZBA_POL+1];
   int kostka; //mowi co ostatnio wyrzucono
   int czyj_ruch;
   int kolejek_do_konca;
} pl;

/**takie komunikaty wysylaja gracze*/
typedef struct _client_msg {
  int type;
  int kostka;
} cm;

int sid;
gm game_msg;
pl plansza_msg;
cm client_msg;
int cid[ILOSC_GRACZY];

int main(int argc, char **argv) {
	int c, ret, end, i, kolejka, tura;
	struct sockaddr_in sin, cin;
	socklen_t cinlen;

	srand(time(0));

	if(ILOSC_GRACZY>4 || ILOSC_GRACZY < 2){
	  printf("\nPodano niepoprawna ilosc graczy (%d), gra dla 2-4 graczy\n", ILOSC_GRACZY);
	  return 0;
	}

	/**obsluga sygnalu SIGINT*/
	signal(SIGINT,sprzatanie);

	if((sid = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP)) < 0) {
		printf("Socket error: %s\n", strerror(errno));
		return 1;
	}

	memset(&sin, 0, sizeof(sin));

	sin.sin_family = AF_INET;
	sin.sin_port = htons(PORT);
	sin.sin_addr.s_addr = INADDR_ANY;

	if(bind(sid, (struct sockaddr *)&sin, sizeof(sin)) != 0) {
		printf("Bind error: %s\n", strerror(errno));
		return 1;
	}

	printf("\nIP addr: %s\n\n", inet_ntoa(sin.sin_addr));

	if(listen(sid, 5) != 0)  {
		printf("Listen error: %s\n", strerror(errno));
		return 1;
	}

while(1){

	for(i=1;i <= ILOSC_GRACZY; i++){
	  plansza_msg.gracze[i].numer = 1;
	  plansza_msg.gracze[i].pozycja = LICZBA_POL;
	  plansza_msg.gracze[i].punkty = 0;
	  snprintf(plansza_msg.gracze[i].label, LABEL_LEN, "Punkty: %d", 0);
	}

	inicjuj_pola();
	puts("\nOczekuje na graczy...\n==================================================\n");
	/**logowanie graczy*/
	    i=0;
	    while(i < ILOSC_GRACZY){
		if((cid[i] = accept(sid, (struct sockaddr *)&cin, &cinlen)) < 0) {
			printf("accept error: %s\n", strerror(errno));
			continue;
		}
		else {
			puts("zalogowany!");
			printf("client_ipaddr: %s\n", inet_ntoa(cin.sin_addr));
			game_msg.type = 0;
			game_msg.id = i;
			c = send(cid[i], &game_msg, sizeof(game_msg), 0);
			ret = recv(cid[i], &client_msg, sizeof(client_msg), 0);
			if(ret == -1) 
			{
				printf("recv error: %s\n", strerror(errno)); close_all_cid(); end = 1;
			}
			if(ret == 0) 
			{
				printf("recv error: disconnect\n"); close_all_cid(); end = 1;
			}

			c = send(cid[i], &plansza_msg, sizeof(plansza_msg), 0);
			++i;
		}
		sleep(1);
		puts("czekam na więcej graczy...");
	}
	printf("zebrano graczy!\nrozpoczynamy gre...\n");
	

	/** Obsluga gry, petla glowna samej gry*/
	end = kolejka = tura = 0;

	while(end != 1){
		  tura++;
		  plansza_msg.kolejek_do_konca = ILOSC_KOLEJEK - tura;
		  printf("\n~~~kolejek_do_konca: %d\n",plansza_msg.kolejek_do_konca);

		  if(tura >= ILOSC_KOLEJEK) { end = 1; game_msg.type = 4; }
	  
		  for(i = 0; i < ILOSC_GRACZY; i++){

		  game_msg.type = 2;
		  c = send(cid[i], &game_msg, sizeof(game_msg), 0);
		  if(c == -1) {
		    printf("write error: %s\n", strerror(errno)); close_all_cid(); end = 1;
		  }

		  send_to_all(i);

		  printf("zezwalam graczowi [%d] na ruch, czekam...\n", i+1);

		  ret = recv(cid[i], &client_msg, sizeof(client_msg), 0);
		  if(ret == -1) 
		  {
			printf("recv error: %s\n", strerror(errno)); close_all_cid(); end = 1;
			}
		  if(ret == 0) 
			{
			printf("recv error: disconnect\n"); close_all_cid(); end = 1;
		  }
		  printf("dostalem odpowiedz od gracza [%d], chce rzucac: %d\n", i+1, client_msg.kostka);

		  game_msg.liczba_oczek = (rand()%6)+1;
		  printf("wyrzucil::::[%d]\n", game_msg.liczba_oczek);

		  plansza_msg.czyj_ruch=i+1;

		  c = send(cid[i], &game_msg, sizeof(game_msg), 0);
		  if(c == -1) {
		    printf("write error: %s\n", strerror(errno)); close_all_cid(); end = 1;
		  }

		  c = send(cid[i], &plansza_msg, sizeof(plansza_msg), 0);
		  if(c == -1) {
		    printf("write error: %s\n", strerror(errno)); close_all_cid(); end = 1;
		  }

		  ret = recv(cid[i], &plansza_msg, sizeof(plansza_msg), 0);
		  if(ret == -1) 
		  {
			printf("recv error: %s\n", strerror(errno)); close_all_cid(); end = 1;
			}
		  if(ret == 0) 
			{
			printf("recv error: disconnect\n"); close_all_cid(); end = 1;
		  }

	  }

	}


	/** koniec petli glownej gry*/
	zakoncz_gre();
	puts("Koniec gry...");
	sleep(2);
	
}
	if(close(sid) == -1) {
		printf("close error: %s\n", strerror(errno));
		return 1;
	}
	return 0;
}


void inicjuj_pola()
{
  int a,b;
  printf("==================================================\nInicjuje pola...\n");
  printf("==================================================\nILOSC GRACZY: %d\nILOSC_KOLEJEK: %d\n", ILOSC_GRACZY, ILOSC_KOLEJEK);

  for(a = 1; a < LICZBA_POL; a++)
  {
  b = (rand()%10);
  
  if(b >= 8){ //prawdopodobienstwo wystapienia grzyba - 80%
    plansza_msg.pola[a].czy_jest = 0;
    plansza_msg.pola[a].nagroda = 0;
    }
  else{
    plansza_msg.pola[a].czy_jest = 1;
    b = (rand()%3);
    if(b >= 1) plansza_msg.pola[a].nagroda = (rand()%3)+1; //czy muchomor czy dobry, punkty
      else plansza_msg.pola[a].nagroda = (-1)*((rand()%2)+1);
    }
  }
  plansza_msg.pola[0].czy_jest = 0;
  plansza_msg.pola[0].nagroda = 0;
  plansza_msg.pola[LICZBA_POL].czy_jest = 0;
  plansza_msg.pola[LICZBA_POL].nagroda = 0;
  
  /*drukowanie w terminalu wartosci pol na planszy*/
//   printf("[nr_pola] : czy_jest : wartosc\n");
//   for(a = 1; a <= LICZBA_POL; a++){
//   printf("||[%d]\t: %d \t: %d\t||\n", a, plansza_msg.pola[a].czy_jest, plansza_msg.pola[a].nagroda);
//   }
}


void send_to_all(int e){
  int i, c;
  game_msg.type = 1;
  plansza_msg.czyj_ruch = e+1;

  for(i = 0; i < ILOSC_GRACZY; i++){
    if(i != e){

      c = send(cid[i], &game_msg, sizeof(game_msg), 0);
      if(c == -1) {
	    printf("write error: %s\n", strerror(errno)); close_all_cid();
		    }
}
      c = send(cid[i], &plansza_msg, sizeof(plansza_msg), 0);
      if(c == -1) {
	    printf("write error: %s\n", strerror(errno)); close_all_cid();
		    }

	    
      }
}

void zakoncz_gre(){
  int i, c;
  game_msg.type = 4;
  game_msg.liczba_oczek = plansza_msg.gracze[0].punkty;
  printf("\n[*]*[*]*[*]*[*] najlepszy(0): %d\n", game_msg.liczba_oczek);
  for(i = 1; i <= ILOSC_GRACZY; i++){
    if(plansza_msg.gracze[i].punkty >= game_msg.liczba_oczek){
      game_msg.liczba_oczek = plansza_msg.gracze[i].punkty;
      printf("\n[*]*[*]*[*]*[*] najlepszy(0): %d\n", game_msg.liczba_oczek);
    }
  }
  
    for(i = 0; i < ILOSC_GRACZY; i++){
      c = send(cid[i], &game_msg, sizeof(game_msg), 0);
      if(c == -1) {
	    printf("write error: %s\n", strerror(errno)); close_all_cid();
		    }
     }
      c = send(cid[i], &plansza_msg, sizeof(plansza_msg), 0);
      if(c == -1) {
	    printf("write error: %s\n", strerror(errno)); close_all_cid();
		    }
}

void close_all_cid(){
    int i;
    for(i = 0; i < ILOSC_GRACZY; i++){
      close(cid[i]);
     }

}

void sprzatanie(int sig) {
	puts("sprzatanie..................");
	close(sid);
	exit(0);
}