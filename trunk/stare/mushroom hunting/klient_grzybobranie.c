#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <errno.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <X11/Xlib.h>
#include <signal.h>

#include "zasady.h"
#define LICZBA_POL 61

#define LABEL_LEN 10

typedef int bool_t;
int id;

void rzut_minus(int i);
void rzut_plus(int i);
void draw_everything();
void inicjuj_pola();
void znaleziono(int nagroda);
void kostki();
void sprzatanie(int sig);
void send_to_all(int cid[]);
void wyswietl();
void kto_teraz();
void print_end();
void draw_everything_end();

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
msg.type  =  0; //connect
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

/**komunikaty o stanie planszy i graczy*/
typedef struct _pola {
   int type;
   struct DaneGraczy gracze[ILOSC_GRACZY+1];
   struct Nagrody pola[LICZBA_POL+1];
   int kostka; //mowi co ostatnio wyrzucono
   int czyj_ruch;
   int kolejek_do_konca;
//   bool_t czy_znaleziono; //mowi czy w ostatniom ruchu cos znaleziono
} pl;


/**takie komunikaty wysylaja gracze*/
typedef struct _client_msg {
  int type;
  int kostka;
} cm;

struct wspolrzedne locxy[] = {
      {275, 175},
      {200, 175},      {200, 250},      {200, 325},      {200, 400},
      {125, 400},      {125, 325},      {125, 250},      {125, 175},      {125, 100},      {125, 25},
      {50, 25},      {50, 100},       {50, 175},       {50, 250},       {50, 325},       {50, 400},
      {50, 475},      {125, 475},       {200, 475},      {275, 475},      {350, 475},      {425, 475},       {500, 475},      {575, 475},      {650, 475},      
      {750, 475},      {750, 400},      {750, 325},      {750, 250},      {750, 175},      {750, 100}, 
      {750, 25},      {675, 25},
      {600, 25},      {600, 100},
      {600, 175},
      {675, 175},      {675, 250},      {675, 325},
      {675, 400},
      {575, 400},      {575, 325},
      {575, 250},
      {500, 250},      {500, 175},      {500, 100},
      {500, 25},
      {425, 25},      {425, 100},      {425, 175},      {425, 250}, 
      {425, 325},
      {500, 325},
      {500, 400},      {425, 400},      {350, 400},
      {275, 400},      {275, 325},
      {275, 250},
      {350, 250},
      {350, 175},
};

int sid;
gm game_msg;
pl plansza_msg;
cm client_msg;

Display *mydisplay;
Window mywindow;
XSetWindowAttributes mywindowattributes;
XGCValues gcvalues;
GC gc;
Visual *myvisual;
int mydepth;
int myscreen;
Colormap mycolormap;
XColor mycolor,mycolor1,dummy;
XEvent myevent;
XFontStruct * font_struct;
char title[23];
int kostka_plus, kostka_minus;

char czyj_ruch[] = "Ruch gracza:  ";
char komunikat[] = "Czekaj...      ";
char wynik_kostki[] = "Oczekiwanie...";
char info_kolejki[14];
char tmp[] = " \0";

  /**Trasa */
  XPoint trasa[] = {
      {350, 175},
      {200, 175},
      {200, 400},
      {125, 400},
      {125, 25},
      {50, 25},
      {50, 475},
      {750, 475},
      {750, 25},
      {600, 25},
      {600, 175},
      {675, 175},
      {675, 400},
      {575, 400},
      {575, 250},
      {500, 250},
      {500, 25},
      {425, 25},
      {425, 325},
      {500, 325},
      {500, 400},
      {275, 400},
      {275, 250},
      {350, 250},
      {350, 175}
    };
  int nptrasa = sizeof(trasa)/sizeof(XPoint);


int main(int argc, char **argv) {
	int i, end = 0, ret, snd, done = 0, active = 0;
	struct sockaddr_in sin;



	if(signal(SIGINT, sprzatanie) == SIG_ERR) {
		fprintf(stderr, "signal error: SIGINT\n");
		exit(EXIT_FAILURE);
	}

	if((sid = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP)) < 0) {
		printf("socket error: %s\n", strerror(errno));
		return 1;
	}

	memset(&sin, 0, sizeof(sin));

	sin.sin_family = AF_INET;
	sin.sin_port = htons(PORT);
	sin.sin_addr.s_addr = inet_addr(ADRES_IP); // -1 error

	if(sin.sin_addr.s_addr == -1) {
		printf("sin.sin_addr.s_addr error\n");
		return 1;
	}

	if(connect(sid, (struct sockaddr *)&sin, sizeof(sin)) == -1) {
		printf("connect error: %s\n", strerror(errno));
		return 1;
	}

	printf("server works: inet_addr %s, port %d\n",
		inet_ntoa(sin.sin_addr), ntohs(sin.sin_port));



	/**X'y ****************************************************************/
	mydisplay = XOpenDisplay("");
	myscreen = DefaultScreen(mydisplay);
	if(mydisplay == NULL) {
	  fprintf(stderr, "cannot connect to X server\n");
	  exit(EXIT_FAILURE);
	}
	myvisual = DefaultVisual(mydisplay,myscreen);
	mydepth = DefaultDepth(mydisplay,myscreen);
	mywindowattributes.background_pixel = XWhitePixel(mydisplay,myscreen);
	mywindowattributes.override_redirect = False;

	
	mywindow = XCreateWindow(mydisplay,XRootWindow(mydisplay,myscreen),
				  0,0,800,600,10,mydepth,InputOutput,
				  myvisual,CWBackPixel|CWOverrideRedirect,
				  &mywindowattributes);

	XSelectInput(mydisplay, mywindow, ExposureMask | KeyPressMask |
			  ButtonPressMask | Button1MotionMask |
			  Button2MotionMask | StructureNotifyMask);
		  
			  
	gc = DefaultGC(mydisplay,myscreen);

	XFontStruct* font_info;
	  char* font_name = "*-helvetica-medium-r-*-14-*";
	  font_info = XLoadQueryFont(mydisplay, font_name);
	  if (!font_info) {
	    fprintf(stderr, "XLoadQueryFont: failed loading font %s\n", font_name);
	  }
	XSetFont(mydisplay, gc, font_info->fid);
	/** **********************************************************************/


	puts("Zglaszasz prosbe zalogowania...");
	puts("Oczekujesz...");
	ret = recv(sid, &game_msg, sizeof(game_msg), 0);
	id = game_msg.id+1;
	printf("\n=Zalogowano!=\n\n=Jestes Graczem: %d===", id);
	client_msg.type = 1;

	snd = send(sid, &client_msg, sizeof(client_msg), 0);
	if(snd == -1) {
	  printf("write error: %s\n", strerror(errno));
	}
	ret = recv(sid, &plansza_msg, sizeof(plansza_msg), 0);
	plansza_msg.kolejek_do_konca = ILOSC_KOLEJEK;

	XMapWindow(mydisplay,mywindow);
	sprintf(title, "Gracz %d - Grzybobranie", id);
	XStoreName(mydisplay, mywindow, title);

	draw_everything(); 
	XFlush(mydisplay);

	while(end == 0) {
		ret = recv(sid, &game_msg, sizeof(game_msg), 0);
		if(ret == -1) 
		{
			close(sid);
			printf("recv error: %s\n", strerror(errno));
			return 0;
		}
		if(ret == 0) 
		{
			close(sid);
			printf("recv error: disconnect\n");
			return 0;
		}

		    switch(game_msg.type) {
			    case 1:
				    ret = recv(sid, &plansza_msg, sizeof(plansza_msg), 0);
				    if(ret == -1) 
				    {
					    close(sid);
					    printf("recv error: %s\n", strerror(errno));
					    return 0;
				    }
				    if(ret == 0) 
				    {
					    close(sid);
					    printf("recv error: disconnect\n");
					    return 0;
				    }
				    printf("Czekasz...\n\n");

				    draw_everything();
				    wyswietl();
				    kostki();
				    XFlush(mydisplay);

			    break;
			    case 2:
				    /** Glowna petla gry dla gracza*/
				    ret = recv(sid, &plansza_msg, sizeof(plansza_msg), 0);
				    if(ret == -1) 
				    {
					    close(sid);
					    printf("recv error: %s\n", strerror(errno));
					    return 0;
				    }
				    if(ret == 0) 
				    {
					    close(sid);
					    printf("recv error: disconnect\n");
					    return 0;
				    }

				    printf("Twoja Tura!\n\n");
				    active = 1; done = 0;
				    game_msg.type = 1;
				    wyswietl();
				    draw_everything();
				    XFlush(mydisplay);

				    /** X-y*/
				    while (!done)
				    {
					XNextEvent(mydisplay,&myevent); 
					switch (myevent.type)
					{
					  case Expose:
					    draw_everything();

					  case ButtonPress:
					    draw_everything();
					    /**ruch do przodu + */
					    if(myevent.xbutton.x >= 650 && myevent.xbutton.x <= 700 
					      && myevent.xbutton.y >= 525 && myevent.xbutton.y <= 757 && active == 1)
					    {
					    client_msg.kostka=1;
					    active = 0;
					    done = 1;

					    }
					      /**ruch do tylu - */
					    if(myevent.xbutton.x >= 725 && myevent.xbutton.x <= 775 
					      && myevent.xbutton.y >= 525 && myevent.xbutton.y <= 757 && active == 1)
					    {
					    client_msg.kostka=2;
					    active = 0;
					    done = 1;
					    }
					    break;

					  case KeyPress:
						puts("ten przycisk nic nie robi...");
					  break;
					}
				    }
				    /** **/

				    snd = send(sid, &client_msg, sizeof(client_msg), 0);
				    if(snd == -1) {
				      printf("write error: %s\n", strerror(errno));
				    }
				    ret = recv(sid, &game_msg, sizeof(game_msg), 0);
				    ret = recv(sid, &plansza_msg, sizeof(plansza_msg), 0);
				    
				    if(client_msg.kostka == 1) rzut_plus(game_msg.liczba_oczek);
					    else
				    if(client_msg.kostka == 2) rzut_minus(game_msg.liczba_oczek);

				    snd = send(sid, &plansza_msg, sizeof(plansza_msg), 0);
				    draw_everything();
				    XFlush(mydisplay);
			break;

			case 4:
			      ret = recv(sid, &plansza_msg, sizeof(plansza_msg), 0);
			      game_msg.liczba_oczek = plansza_msg.gracze[1].punkty;
			      for(i = 1; i <= ILOSC_GRACZY; i++){
				if(plansza_msg.gracze[i].punkty >= game_msg.liczba_oczek){
				  game_msg.liczba_oczek = plansza_msg.gracze[i].punkty;
				  printf("\n[*]*[*]*[*]*[*] najlepszy(0): %d\n", game_msg.liczba_oczek);
/*				  game_msg.id = i;*/
				}
			      }

			      if(plansza_msg.gracze[id].punkty >= game_msg.liczba_oczek){
				printf("\nZwyciestwo!!!!!!!!!!!!\n");
				strcpy(komunikat, "Zwyciestwo!!!!!!!!!!!!");
			      }
			      else {
				printf("\nPrzegrana!!!!!!!!!!!!!\n");
				strcpy(komunikat, "Przegrana!!!!!!!!!!!!!");
			      }
			      //strcpy(komunikat, "NIC");
			      draw_everything_end();
			      XFlush(mydisplay);

			      sleep(5);

			      if(close(sid) == -1) {
				  printf("close error: %s\n", strerror(errno));
			      }

			      end = 1;

			default:
				printf("czekam...%d\n", game_msg.type);
		}

}

// 	if(close(sid) == -1) {
// 		printf("close error: %s\n", strerror(errno));
// 		return 1;
// 	}
wyswietl();
	XCloseDisplay(mydisplay);
	return 0;
}



void
rzut_plus(int i)
{
	  XColor blue, dummy; 
	  XAllocNamedColor(mydisplay,mycolormap,"Blue",&blue,&dummy); 

	  printf("Wyrzucono: [%d]\n", i);
	  sprintf(wynik_kostki, "Wyrzucono: [%d]", i);
	  kostka_plus = i;
	  plansza_msg.gracze[id].pozycja+=i; puts("+");
	  if(plansza_msg.gracze[id].pozycja>LICZBA_POL) plansza_msg.gracze[id].pozycja%=LICZBA_POL;
	  /*znajdujemy cos i zabieramy*/
	  if(plansza_msg.pola[plansza_msg.gracze[id].pozycja].czy_jest != 0){
	    plansza_msg.gracze[id].punkty += plansza_msg.pola[plansza_msg.gracze[id].pozycja].nagroda;
	    i = plansza_msg.pola[plansza_msg.gracze[id].pozycja].nagroda;
/*	    snprintf(plansza_msg.gracze[i].label, LABEL_LEN, "Punkty: %d", plansza_msg.gracze[id].punkty);*/
	    sprintf(plansza_msg.gracze[i].label, "Punkty: %d", plansza_msg.gracze[id].punkty);
	    plansza_msg.pola[plansza_msg.gracze[id].pozycja].czy_jest = plansza_msg.pola[plansza_msg.gracze[id].pozycja].nagroda = 0; 
	  draw_everything();
	  znaleziono(i);
	  sleep(1);
	  }
	  else draw_everything();

	  printf("pz: %d [%d,%d]", plansza_msg.gracze[id].pozycja, locxy[plansza_msg.gracze[id].pozycja].x, locxy[plansza_msg.gracze[id].pozycja].y);
}

void
rzut_minus(int i)
{
	  XColor blue, dummy; 
	  XAllocNamedColor(mydisplay,mycolormap,"Blue",&blue,&dummy); 

	  printf("Wyrzucono: [%d]\n", i);
	  sprintf(wynik_kostki, "Wyrzucono: [%d]", i);
	  kostka_minus = i;
	  plansza_msg.gracze[id].pozycja-=i; puts("-");
	  if(plansza_msg.gracze[id].pozycja<=0) { 
	    if(plansza_msg.gracze[id].pozycja==-1) 
	      plansza_msg.gracze[id].pozycja=61;
	    else
	      plansza_msg.gracze[id].pozycja=LICZBA_POL+plansza_msg.gracze[id].pozycja; 
		}
	  XSetForeground(mydisplay,gc,blue.pixel);

	  /*znajdujemy cos i zabieramy*/
	  if(plansza_msg.pola[plansza_msg.gracze[id].pozycja].czy_jest != 0){
	    plansza_msg.gracze[id].punkty += plansza_msg.pola[plansza_msg.gracze[id].pozycja].nagroda;
	    i = plansza_msg.pola[plansza_msg.gracze[id].pozycja].nagroda;
/*	    snprintf(plansza_msg.gracze[i].label, LABEL_LEN, "Punkty: %d", plansza_msg.gracze[id].punkty);*/
	    sprintf(plansza_msg.gracze[i].label, "Punkty: %d", plansza_msg.gracze[id].punkty);
	    plansza_msg.pola[plansza_msg.gracze[id].pozycja].czy_jest = plansza_msg.pola[plansza_msg.gracze[id].pozycja].nagroda = 0; 
	  draw_everything();
	  znaleziono(i);
	  sleep(1);
	  }
	  else draw_everything();

    printf("pz: %d [%d,%d]", plansza_msg.gracze[id].pozycja, locxy[plansza_msg.gracze[id].pozycja].x, locxy[plansza_msg.gracze[id].pozycja].y);
}        



void draw_everything(){
	    int i;
	    mycolormap = DefaultColormap(mydisplay,myscreen); //deklaracje kolorow        
	    XColor dodger_blue, sea_green, black, white, blue, red, gold, green, peru, orange_red, forest_green, dummy;              
	    XAllocNamedColor(mydisplay,mycolormap,"Dodger Blue",&dodger_blue,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Medium Sea Green",&sea_green,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Black",&black,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"White",&white,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Blue",&blue,&dummy);  
	    XAllocNamedColor(mydisplay,mycolormap,"Red",&red,&dummy);  
	    XAllocNamedColor(mydisplay,mycolormap,"Gold",&gold,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Lawn Green",&green,&dummy);    
	    XAllocNamedColor(mydisplay,mycolormap,"Peru",&peru,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Orange Red",&orange_red,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Forest Green",&forest_green,&dummy);

	    XSetForeground(mydisplay,gc,forest_green.pixel);
	    XFillRectangle(mydisplay,mywindow,gc,0,0,800,512); //podloze
	    XSetForeground(mydisplay,gc,white.pixel);
	    XFillRectangle(mydisplay,mywindow,gc,0,512,800,88); //hud
	    XSetForeground(mydisplay,gc,black.pixel);

	    /**linia oddzielajaca plansze od punktacji*/
	      XSetLineAttributes(mydisplay, gc, 4, LineOnOffDash, CapRound, JoinRound);
	      XDrawLine(mydisplay, mywindow, gc, 0, 512, 800, 512);
	      XDrawLine(mydisplay, mywindow, gc, 0, 600, 800, 600);
	      XDrawLine(mydisplay, mywindow, gc, 800, 0, 800, 600);
	    /** rysowanie trasy*/ 
	      XSetLineAttributes(mydisplay, gc, 2, LineDoubleDash, CapRound, JoinRound);
	      XDrawLines(mydisplay, mywindow, gc, trasa, nptrasa, CoordModeOrigin);
	    /**rysowanie pol*/ 
	      for(i=1; i<=LICZBA_POL; i++)
		XFillArc(mydisplay, mywindow, gc, locxy[i].x-(50/2), locxy[i].y-(50/2), 50, 50, 0, 360*64);
	    /** strzalki, start point*/ 
	    XSetLineAttributes(mydisplay, gc, 5, LineSolid, CapRound, JoinRound);
	    XSetForeground(mydisplay,gc,dodger_blue.pixel);
	      XDrawLine(mydisplay, mywindow, gc, 230, 175, 255, 165); //from-to
	      XDrawLine(mydisplay, mywindow, gc, 230, 175, 255, 185); //from-to
	    XSetForeground(mydisplay,gc,sea_green.pixel);
	      XDrawLine(mydisplay, mywindow, gc, 320, 175, 295, 165); //from-to
	      XDrawLine(mydisplay, mywindow, gc, 320, 175, 295, 185); //from-to
	  /**kosci*/
	  XSetLineAttributes(mydisplay, gc, 3, LineSolid, CapRound, JoinRound);
	    XSetForeground(mydisplay,gc,black.pixel);
	    XDrawRectangle(mydisplay, mywindow, gc, 649, 524, 52, 52);
	    XDrawRectangle(mydisplay, mywindow, gc, 724, 524, 52, 52);
		XSetForeground(mydisplay,gc,dodger_blue.pixel);
		XFillRectangle(mydisplay,mywindow,gc,650,525,50,50);
		XSetForeground(mydisplay,gc,sea_green.pixel);
		XFillRectangle(mydisplay,mywindow,gc,725,525,50,50);
		
	  /**pola nagrod*/
	  for(i = 1; i <= LICZBA_POL; i++)
	    if(plansza_msg.pola[i].czy_jest == 1){
	      sprintf(tmp, " %d ", plansza_msg.pola[i].nagroda);
	      if(plansza_msg.pola[i].nagroda>0){
	      XSetForeground(mydisplay,gc,peru.pixel);
	      XFillRectangle(mydisplay, mywindow, gc, locxy[i].x+5,locxy[i].y+5, 25, 25);
	      XDrawArc(mydisplay, mywindow, gc, locxy[i].x-(50/2), locxy[i].y-(50/2), 50, 50, 0, 360*64);
	      }
	      else{
	      XSetForeground(mydisplay,gc,orange_red.pixel);
	      XFillRectangle(mydisplay, mywindow, gc, locxy[i].x+5,locxy[i].y+5, 25, 25);
	      XDrawArc(mydisplay, mywindow, gc, locxy[i].x-(50/2), locxy[i].y-(50/2), 50, 50, 0, 360*64);
	    }
	  XSetForeground(mydisplay,gc,black.pixel);
	  XDrawString(mydisplay, mywindow, gc, locxy[i].x+12,locxy[i].y+23, tmp, 3);
	  }

	/**pola informacji graczy*/
	  XSetForeground(mydisplay,gc,blue.pixel);
	  XDrawRectangle(mydisplay, mywindow, gc, 215, 527, 80, 60);
	  XSetForeground(mydisplay,gc,red.pixel);
	  XDrawRectangle(mydisplay, mywindow, gc, 300, 527, 80, 60);

	  if(ILOSC_GRACZY >= 3){
	    XSetForeground(mydisplay,gc,gold.pixel);
	    XDrawRectangle(mydisplay, mywindow, gc, 385, 527, 80, 60);
	  }
	  if(ILOSC_GRACZY >= 4){
	    XSetForeground(mydisplay,gc,green.pixel);
	    XDrawRectangle(mydisplay, mywindow, gc, 470, 527, 80, 60);
	  }

	/**napisy*/
	  XSetForeground(mydisplay,gc,black.pixel);
	  sprintf(czyj_ruch, "Ruch gracza: %d", plansza_msg.czyj_ruch);
	  XDrawString(mydisplay, mywindow, gc, 25, 530, czyj_ruch, strlen(czyj_ruch));
	  XDrawString(mydisplay, mywindow, gc, 680, 590, wynik_kostki, strlen(wynik_kostki));

	  snprintf(plansza_msg.gracze[1].label, LABEL_LEN+1, "Punkty: %d ", plansza_msg.gracze[1].punkty);
	  XDrawString(mydisplay, mywindow, gc, 225, 565, plansza_msg.gracze[1].label, strlen(plansza_msg.gracze[1].label));
	  snprintf(plansza_msg.gracze[2].label, LABEL_LEN+1, "Punkty: %d ", plansza_msg.gracze[2].punkty);
	  XDrawString(mydisplay, mywindow, gc, 310, 565, plansza_msg.gracze[2].label, strlen(plansza_msg.gracze[2].label));
	  
	  if(ILOSC_GRACZY >= 3){
	  snprintf(plansza_msg.gracze[3].label, LABEL_LEN+1, "Punkty: %d ", plansza_msg.gracze[3].punkty);
	  XDrawString(mydisplay, mywindow, gc, 395, 565, plansza_msg.gracze[3].label, strlen(plansza_msg.gracze[3].label));
	  }
	  if(ILOSC_GRACZY >= 4){
	  snprintf(plansza_msg.gracze[4].label, LABEL_LEN+1, "Punkty: %d ", plansza_msg.gracze[4].punkty);
	  XDrawString(mydisplay, mywindow, gc, 480, 565, plansza_msg.gracze[4].label, strlen(plansza_msg.gracze[4].label));
	  }

	  /**oznaczamy gracza na planszy*/
	  XSetLineAttributes(mydisplay, gc, 4, LineDoubleDash, CapRound, JoinRound);

	  XSetForeground(mydisplay,gc,blue.pixel);
	  XDrawArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[1].pozycja].x-10-(30/2), 
					    locxy[plansza_msg.gracze[1].pozycja].y-10-(30/2), 30, 30, 0,360*64);
	  XFillArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[1].pozycja].x-10-(26/2), 
					    locxy[plansza_msg.gracze[1].pozycja].y-10-(26/2), 26, 26, 0,360*64);

	  XSetForeground(mydisplay,gc,red.pixel);
	  XDrawArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[2].pozycja].x+10-(30/2), 
					    locxy[plansza_msg.gracze[2].pozycja].y-10-(30/2), 30, 30, 0,360*64);
	  XFillArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[2].pozycja].x+10-(26/2), 
					    locxy[plansza_msg.gracze[2].pozycja].y-10-(26/2), 26, 26, 0,360*64);
	  
	  if(ILOSC_GRACZY >= 3){
	  XSetForeground(mydisplay,gc,gold.pixel);
	  XDrawArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[3].pozycja].x-10-(30/2), 
					    locxy[plansza_msg.gracze[3].pozycja].y+10-(30/2), 30, 30, 0,360*64);
	  XFillArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[3].pozycja].x-10-(26/2), 
					    locxy[plansza_msg.gracze[3].pozycja].y+10-(26/2), 26, 26, 0,360*64);
	  }
	  if(ILOSC_GRACZY >= 4){
	  XSetForeground(mydisplay,gc,green.pixel);
	  XDrawArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[4].pozycja].x+10-(30/2), 
					    locxy[plansza_msg.gracze[4].pozycja].y+10-(30/2), 30, 30, 0,360*64);
	  XFillArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[4].pozycja].x+10-(26/2), 
					    locxy[plansza_msg.gracze[4].pozycja].y+10-(26/2), 26, 26, 0,360*64);
	  }
	  XSetLineAttributes(mydisplay, gc, 5, LineSolid, CapRound, JoinRound);

	  if(plansza_msg.kolejek_do_konca > 3){
	  XSetForeground(mydisplay,gc,forest_green.pixel);
	  }
	  else
	  XSetForeground(mydisplay,gc,orange_red.pixel);

	  sprintf(info_kolejki,"Pozostalo: %d ", plansza_msg.kolejek_do_konca);
	  XDrawString(mydisplay, mywindow, gc, 25, 590, info_kolejki, sizeof(info_kolejki)-1);
	  

	  kostki();
	  kto_teraz();
/*	  printf("\n~~~kolejek_do_konca: %d\n",plansza_msg.kolejek_do_konca);*/
}

void draw_everything_end(){
	    int i;
	    mycolormap = DefaultColormap(mydisplay,myscreen); //deklaracje kolorow        
	    XColor dodger_blue, sea_green, black, white, blue, red, gold, green, peru, orange_red, forest_green, dummy;              
	    XAllocNamedColor(mydisplay,mycolormap,"Dodger Blue",&dodger_blue,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Medium Sea Green",&sea_green,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Black",&black,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"White",&white,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Blue",&blue,&dummy);  
	    XAllocNamedColor(mydisplay,mycolormap,"Red",&red,&dummy);  
	    XAllocNamedColor(mydisplay,mycolormap,"Gold",&gold,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Lawn Green",&green,&dummy);    
	    XAllocNamedColor(mydisplay,mycolormap,"Peru",&peru,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Orange Red",&orange_red,&dummy);
	    XAllocNamedColor(mydisplay,mycolormap,"Forest Green",&forest_green,&dummy);

	    XSetForeground(mydisplay,gc,forest_green.pixel);
	    XFillRectangle(mydisplay,mywindow,gc,0,0,800,512); //podloze
	    XSetForeground(mydisplay,gc,white.pixel);
	    XFillRectangle(mydisplay,mywindow,gc,0,512,800,88); //hud
	    XSetForeground(mydisplay,gc,black.pixel);

	    /**linia oddzielajaca plansze od punktacji*/
	      XSetLineAttributes(mydisplay, gc, 4, LineOnOffDash, CapRound, JoinRound);
	      XDrawLine(mydisplay, mywindow, gc, 0, 512, 800, 512);
	      XDrawLine(mydisplay, mywindow, gc, 0, 600, 800, 600);
	      XDrawLine(mydisplay, mywindow, gc, 800, 0, 800, 600);
	    /** rysowanie trasy*/
	      XSetLineAttributes(mydisplay, gc, 2, LineDoubleDash, CapRound, JoinRound);
	      XDrawLines(mydisplay, mywindow, gc, trasa, nptrasa, CoordModeOrigin);
	    /**rysowanie pol*/
	      for(i=1; i<=LICZBA_POL; i++)
		XFillArc(mydisplay, mywindow, gc, locxy[i].x-(50/2), locxy[i].y-(50/2), 50, 50, 0, 360*64);
	    /** strzalki, start point*/
	    XSetLineAttributes(mydisplay, gc, 5, LineSolid, CapRound, JoinRound);
	    XSetForeground(mydisplay,gc,dodger_blue.pixel);
	      XDrawLine(mydisplay, mywindow, gc, 230, 175, 255, 165); //from-to
	      XDrawLine(mydisplay, mywindow, gc, 230, 175, 255, 185); //from-to
	    XSetForeground(mydisplay,gc,sea_green.pixel);
	      XDrawLine(mydisplay, mywindow, gc, 320, 175, 295, 165); //from-to
	      XDrawLine(mydisplay, mywindow, gc, 320, 175, 295, 185); //from-to
	  /**kosci*/
	  XSetLineAttributes(mydisplay, gc, 3, LineSolid, CapRound, JoinRound);
	    XSetForeground(mydisplay,gc,black.pixel);
	    XDrawRectangle(mydisplay, mywindow, gc, 649, 524, 52, 52);
	    XDrawRectangle(mydisplay, mywindow, gc, 724, 524, 52, 52);
		XSetForeground(mydisplay,gc,dodger_blue.pixel);
		XFillRectangle(mydisplay,mywindow,gc,650,525,50,50);
		XSetForeground(mydisplay,gc,sea_green.pixel);
		XFillRectangle(mydisplay,mywindow,gc,725,525,50,50);
		
	  /**pola nagrod*/
	  for(i = 1; i <= LICZBA_POL; i++)
	    if(plansza_msg.pola[i].czy_jest == 1){
	      sprintf(tmp, " %d ", plansza_msg.pola[i].nagroda);
	      if(plansza_msg.pola[i].nagroda>0){
	      XSetForeground(mydisplay,gc,peru.pixel);
	      XFillRectangle(mydisplay, mywindow, gc, locxy[i].x+5,locxy[i].y+5, 25, 25);
	      XDrawArc(mydisplay, mywindow, gc, locxy[i].x-(50/2), locxy[i].y-(50/2), 50, 50, 0, 360*64);
	      }
	      else{
	      XSetForeground(mydisplay,gc,orange_red.pixel);
	      XFillRectangle(mydisplay, mywindow, gc, locxy[i].x+5,locxy[i].y+5, 25, 25);
	      XDrawArc(mydisplay, mywindow, gc, locxy[i].x-(50/2), locxy[i].y-(50/2), 50, 50, 0, 360*64);
	    }
	  XSetForeground(mydisplay,gc,black.pixel);
	  XDrawString(mydisplay, mywindow, gc, locxy[i].x+12,locxy[i].y+23, tmp, 3);
	  }

	/**pola informacji graczy*/
	  XSetForeground(mydisplay,gc,blue.pixel);
	  XDrawRectangle(mydisplay, mywindow, gc, 215, 527, 80, 60);
	  XSetForeground(mydisplay,gc,red.pixel);
	  XDrawRectangle(mydisplay, mywindow, gc, 300, 527, 80, 60);

	  if(ILOSC_GRACZY >= 3){
	    XSetForeground(mydisplay,gc,gold.pixel);
	    XDrawRectangle(mydisplay, mywindow, gc, 385, 527, 80, 60);
	  }
	  if(ILOSC_GRACZY >= 4){
	    XSetForeground(mydisplay,gc,green.pixel);
	    XDrawRectangle(mydisplay, mywindow, gc, 470, 527, 80, 60);
	  }

	/**napisy*/
	  XSetForeground(mydisplay,gc,black.pixel);
	  sprintf(czyj_ruch, "Ruch gracza: %d", plansza_msg.czyj_ruch);
	  //XDrawString(mydisplay, mywindow, gc, 25, 525, czyj_ruch, strlen(czyj_ruch));
	  XDrawString(mydisplay, mywindow, gc, 680, 590, wynik_kostki, strlen(wynik_kostki));

	  XDrawString(mydisplay, mywindow, gc, 225, 565, plansza_msg.gracze[1].label, strlen(plansza_msg.gracze[1].label));
	  XDrawString(mydisplay, mywindow, gc, 310, 565, plansza_msg.gracze[2].label, strlen(plansza_msg.gracze[2].label));
	  
	  if(ILOSC_GRACZY >= 3){
	  XDrawString(mydisplay, mywindow, gc, 395, 565, plansza_msg.gracze[3].label, strlen(plansza_msg.gracze[3].label));
	  }
	  if(ILOSC_GRACZY >= 4){
	  XDrawString(mydisplay, mywindow, gc, 480, 565, plansza_msg.gracze[4].label, strlen(plansza_msg.gracze[4].label));
	  }

	  /**oznaczamy gracza na planszy*/
	  XSetLineAttributes(mydisplay, gc, 4, LineDoubleDash, CapRound, JoinRound);

	  XSetForeground(mydisplay,gc,blue.pixel);
	  XDrawArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[1].pozycja].x-10-(30/2), 
					    locxy[plansza_msg.gracze[1].pozycja].y-10-(30/2), 30, 30, 0,360*64);
	  XFillArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[1].pozycja].x-10-(26/2), 
					    locxy[plansza_msg.gracze[1].pozycja].y-10-(26/2), 26, 26, 0,360*64);

	  XSetForeground(mydisplay,gc,red.pixel);
	  XDrawArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[2].pozycja].x+10-(30/2), 
					    locxy[plansza_msg.gracze[2].pozycja].y-10-(30/2), 30, 30, 0,360*64);
	  XFillArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[2].pozycja].x+10-(26/2), 
					    locxy[plansza_msg.gracze[2].pozycja].y-10-(26/2), 26, 26, 0,360*64);
	  
	  if(ILOSC_GRACZY >= 3){
	  XSetForeground(mydisplay,gc,gold.pixel);
	  XDrawArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[3].pozycja].x-10-(30/2), 
					    locxy[plansza_msg.gracze[3].pozycja].y+10-(30/2), 30, 30, 0,360*64);
	  XFillArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[3].pozycja].x-10-(26/2), 
					    locxy[plansza_msg.gracze[3].pozycja].y+10-(26/2), 26, 26, 0,360*64);
	  }
	  if(ILOSC_GRACZY >= 4){
	  XSetForeground(mydisplay,gc,green.pixel);
	  XDrawArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[4].pozycja].x+10-(30/2), 
					    locxy[plansza_msg.gracze[4].pozycja].y+10-(30/2), 30, 30, 0,360*64);
	  XFillArc(mydisplay, mywindow, gc, locxy[plansza_msg.gracze[4].pozycja].x+10-(26/2), 
					    locxy[plansza_msg.gracze[4].pozycja].y+10-(26/2), 26, 26, 0,360*64);
	  }
	  XSetLineAttributes(mydisplay, gc, 5, LineSolid, CapRound, JoinRound);

	  kostki();
	  XDrawString(mydisplay, mywindow, gc, 25, 560, komunikat, strlen(komunikat));
}

void kto_teraz(){
  if(plansza_msg.czyj_ruch == id){
    snprintf(komunikat, 10, "Teraz Ty!");
  }
  else {
    snprintf(komunikat, 10, "Czekaj...");
  }
/*
  if(plansza_msg.czyj_ruch == id){
    sprintf(komunikat, "Teraz Ty!");
  }
  else {
    sprintf(komunikat, "Czekaj...");
  }*/
    XDrawString(mydisplay, mywindow, gc, 25, 560, komunikat, strlen(komunikat));

}


void kostki(){
  XColor black, dummy; 
  XAllocNamedColor(mydisplay,mycolormap,"Black",&black,&dummy); 
  XSetForeground(mydisplay,gc,black.pixel);

 if(kostka_plus == 0){
    /** strzalki, start point*/
    XSetLineAttributes(mydisplay, gc, 5, LineSolid, CapRound, JoinRound);
      XDrawLine(mydisplay, mywindow, gc, 655, 550, 670, 535); //from-to
      XDrawLine(mydisplay, mywindow, gc, 655, 550, 670, 565); //from-to
      XDrawLine(mydisplay, mywindow, gc, 670, 550, 685, 535); //from-to
      XDrawLine(mydisplay, mywindow, gc, 670, 550, 685, 565); //from-to
  }
  else if(kostka_plus == 1){
      XFillArc(mydisplay, mywindow, gc, 675-(15/2), 550-(15/2), 15, 15, 0, 360*64);
  }
  else if(kostka_plus == 2){
      XFillArc(mydisplay, mywindow, gc, 660-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 690-(15/2), 565-(15/2), 15, 15, 0, 360*64);
  }
  else if(kostka_plus == 3){
      XFillArc(mydisplay, mywindow, gc, 660-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 675-(15/2), 550-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 690-(15/2), 565-(15/2), 15, 15, 0, 360*64);
  }
  else if(kostka_plus == 4){
      XFillArc(mydisplay, mywindow, gc, 660-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 690-(15/2), 565-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 690-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 660-(15/2), 565-(15/2), 15, 15, 0, 360*64);
  }
  else if(kostka_plus == 5){
      XFillArc(mydisplay, mywindow, gc, 660-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 690-(15/2), 565-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 690-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 660-(15/2), 565-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 675-(15/2), 550-(15/2), 15, 15, 0, 360*64);
  }
  else if(kostka_plus == 6){
      XFillArc(mydisplay, mywindow, gc, 660-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 690-(15/2), 565-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 690-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 660-(15/2), 565-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 690-(15/2), 550-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 660-(15/2), 550-(15/2), 15, 15, 0, 360*64);
  }

  if(kostka_minus == 0){
      XDrawLine(mydisplay, mywindow, gc, 750, 550, 735, 535); //from-to
      XDrawLine(mydisplay, mywindow, gc, 750, 550, 735, 565); //from-to
      XDrawLine(mydisplay, mywindow, gc, 765, 550, 750, 535); //from-to
      XDrawLine(mydisplay, mywindow, gc, 765, 550, 750, 565); //from-to
  }
  else if(kostka_minus == 1){
      XFillArc(mydisplay, mywindow, gc, 750-(15/2), 550-(15/2), 15, 15, 0, 360*64);
  }
  else if(kostka_minus == 2){
      XFillArc(mydisplay, mywindow, gc, 735-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 765-(15/2), 565-(15/2), 15, 15, 0, 360*64);
  }
  else if(kostka_minus == 3){
      XFillArc(mydisplay, mywindow, gc, 735-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 750-(15/2), 550-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 765-(15/2), 565-(15/2), 15, 15, 0, 360*64);
  }
  else if(kostka_minus == 4){
      XFillArc(mydisplay, mywindow, gc, 735-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 765-(15/2), 565-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 765-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 735-(15/2), 565-(15/2), 15, 15, 0, 360*64);
  }
  else if(kostka_minus == 5){
      XFillArc(mydisplay, mywindow, gc, 735-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 765-(15/2), 565-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 765-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 735-(15/2), 565-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 750-(15/2), 550-(15/2), 15, 15, 0, 360*64);
  }
  else if(kostka_minus == 6){
      XFillArc(mydisplay, mywindow, gc, 735-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 765-(15/2), 565-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 765-(15/2), 535-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 735-(15/2), 565-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 765-(15/2), 550-(15/2), 15, 15, 0, 360*64);
      XFillArc(mydisplay, mywindow, gc, 735-(15/2), 550-(15/2), 15, 15, 0, 360*64);
  }
}

void znaleziono(int nagroda)
{
    char znal[]="0";
    sprintf(znal, "%d ", nagroda);
    XColor black, teal, dummy; 
    XAllocNamedColor(mydisplay,mycolormap,"Black",&black,&dummy);
    XAllocNamedColor(mydisplay,mycolormap,"Teal",&teal,&dummy);

    XSetForeground(mydisplay,gc,teal.pixel);
    XFillArc(mydisplay, mywindow, gc, 125-(25/2), 560-(25/2), 25, 25, 0, 360*64);
    XSetForeground(mydisplay,gc,black.pixel);
    XDrawString(mydisplay, mywindow, gc, 123, 565, znal, strlen(znal));
               XFlush(mydisplay);

}

/*dodatkowe informacje w terminalu*/
void wyswietl(){
  int p;
  for(p = 1; p <= ILOSC_GRACZY; p++){
  printf("G%d ::\t typ: %d\t pozycja: %d \t punkty: %d\t czyj_ruch: %d\n, liczba_oczek: %d\n", 
	  p, game_msg.type, plansza_msg.gracze[p].pozycja, plansza_msg.gracze[p].punkty, plansza_msg.czyj_ruch, game_msg.liczba_oczek);
  }
}

void sprzatanie(int sig) {
	puts("sprzatanie..................");
	close(sid);
	exit(0);
}
