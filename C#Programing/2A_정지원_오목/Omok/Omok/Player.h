#pragma once
#include "GameManager.h"

class Player
{
public:
	char name[256] = "Player 00";
	int winNum = 0;	//승리 횟수.
	char dol[4] = "?";	//흑돌 백돌 종류(○/●).
	void ChangeDolToWin(Player &p1, Player &p2);	//승자 플레이어로 함수 실행.
};