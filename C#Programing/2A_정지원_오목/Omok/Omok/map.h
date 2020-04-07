#include "GameManager.h"
#include "Player.h"

class makeMap;

class Control
{
public:
	COORD mapCur;
	void GotoXY(int key, makeMap map, Player p1, Player p2);
	bool Enter(makeMap &map, int turn);
};

class makeMap
{
	void RightStringNull(char ch[], char in[]);	//우측작성 여백 생성 함수.
public:
	GameM *GM;
	Control *playControl;
	int map[19][19] = { 0 };	//0: 빈공간, 1: 흑돌, 2: 백돌.
	int mapCol, mapRow;

	void ResetMap();
	void drawMap(Player p1, Player p2);
	void drawCanvas(Player p1, Player p2);
	void Canvas(int i, int j);
	void drawPlayerStat(Player p1, Player p2);
	void MapWarp(COORD mapCur);
};