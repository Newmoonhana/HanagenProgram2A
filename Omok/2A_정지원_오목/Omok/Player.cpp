#include "Player.h"

void Player::ChangeDolToWin(Player &p1, Player &p2)
{
	if (strcmp(this->name, p1.name) == 0)	//���� �̸��� p1���� ��.
	{
		strcpy_s(p1.dol, blackDol);
		strcpy_s(p2.dol, whiteDol);
	}
	else
	{
		strcpy_s(p1.dol, whiteDol);
		strcpy_s(p2.dol, blackDol);
	}
}