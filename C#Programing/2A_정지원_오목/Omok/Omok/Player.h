#pragma once
#include "GameManager.h"

class Player
{
public:
	char name[256] = "Player 00";
	int winNum = 0;	//�¸� Ƚ��.
	char dol[4] = "?";	//�浹 �鵹 ����(��/��).
	void ChangeDolToWin(Player &p1, Player &p2);	//���� �÷��̾�� �Լ� ����.
};