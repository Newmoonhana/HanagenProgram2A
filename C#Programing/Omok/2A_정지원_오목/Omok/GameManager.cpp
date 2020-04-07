#include "GameManager.h"
#include "map.h"
#include "Player.h"

makeMap thisMap;
Player player01;
Player player02;
Control playControl;

void GameM::Awake(GameM &GM)
{
	thisMap.GM = &GM;
	thisMap.playControl = &playControl;
	GM.turn = 0;

	//�ػ� ����.
	char command[256] = { '\0', };
	sprintf_s(command, "mode con: lines=%d cols=%d", (sizeof(thisMap.map[0]) / sizeof(int) + 2) + 10, (sizeof(thisMap.map) / sizeof(thisMap.map[0]) + 2) * 2);
	system(command);
	playControl.mapCur = { (sizeof(thisMap.map[0]) / sizeof(int)) / 2, (sizeof(thisMap.map[0]) / sizeof(int)) / 2 };
	
	//�÷��̾� ����.
	sprintf_s(player01.name, "Player 01");
	sprintf_s(player02.name, "Player 02");
	player01.ChangeDolToWin(player01, player02);
	
	//�� ����.
	thisMap.ResetMap();
	thisMap.drawMap(player01, player02);

	GameRun(GM);
}

#include <conio.h>
void GameM::GameRun(GameM &GM)
{
	InputKey(GM);
	if (WinLose())
	{
		beforeWinner = turn % 2 + 1;
		thisMap.MapWarp(playControl.mapCur);
		cout << ((beforeWinner == 1) ? player01 : player02).dol;
		COORD p1cur = { 0, thisMap.mapRow + 9 };
		SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), p1cur);
		cout << ' ' << ((beforeWinner == 1) ? player01 : player02).dol << ' ' << ((beforeWinner == 1) ? player01 : player02).name << " Win!" << endl;
		bool loop = true;
		cout << "Retry? : zŰ�� ��������.";
		while (loop)
		{
			int getkey = _getch();
			if (getkey == ENTER)
				loop = false;
		}
		((beforeWinner == 1) ? player01 : player02).winNum++;
		((beforeWinner == 1) ? player01 : player02).ChangeDolToWin(player01, player02);

		Retry(GM);
	}
	else
	{
		turn++;
		thisMap.drawMap(player01, player02);
		GameRun(GM);
	}
}

void GameM::Retry(GameM &GM)
{
	beforeWinner = GM.turn % 2 == 1 ? 0 : 1;
	playControl.mapCur = { (sizeof(thisMap.map[0]) / sizeof(int)) / 2, (sizeof(thisMap.map[0]) / sizeof(int)) / 2 };
	//�� ����.
	thisMap.ResetMap();
	thisMap.drawMap(player01, player02);

	GameRun(GM);
}

void GameM::InputKey(GameM &GM)
{
	bool loop = true;
	while (loop)
	{
		int getkey = _getch();

		switch (getkey)
		{
		case LEFT:
		case RIGHT:
		case UP:
		case DOWN:
			playControl.GotoXY(getkey, thisMap, player01, player02);
			break;
		case ENTER:
			loop = playControl.Enter(thisMap, turn);
			break;
		case RESET:
			loop = false;
			Retry(GM);
			break;
		}
		thisMap.drawPlayerStat(player01, player02);
	}
}

bool GameM::WinLose()
{
	int sum[5] = { 0 };	//0:�߾�, 1:�»����, 2:����, 3:���Ͽ��, 4:�¿�.
	sum[0] = 1;
	//�ֺ� ������ ���� �� ����.
	//�»�.
	for (int i = 1; i <= 4; i++)
		if (playControl.mapCur.X - i >= 0)
			if (playControl.mapCur.Y - i >= 0)
			{
				if (thisMap.map[playControl.mapCur.X - i][playControl.mapCur.Y - i] == thisMap.map[playControl.mapCur.X][playControl.mapCur.Y])
					sum[1]++;
				else
					break;
			}
	//����.
	for (int i = 1; i <= 4; i++)
		if (playControl.mapCur.X + i >= 0)
			if (playControl.mapCur.Y + i >= 0)
			{
				if (thisMap.map[playControl.mapCur.X + i][playControl.mapCur.Y + i] == thisMap.map[playControl.mapCur.X][playControl.mapCur.Y])
					sum[1]++;
				else
					break;
			}
	if (sum[1] + sum[0] >= 5)
		return true;

	//��.
	for (int i = 1; i <= 4; i++)
		if (playControl.mapCur.Y - i >= 0)
		{
			if (thisMap.map[playControl.mapCur.X][playControl.mapCur.Y - i] == thisMap.map[playControl.mapCur.X][playControl.mapCur.Y])
				sum[2]++;
			else
				break;
		}
	//��.
	for (int i = 1; i <= 4; i++)
		if (playControl.mapCur.Y + i >= 0)
		{
			if (thisMap.map[playControl.mapCur.X][playControl.mapCur.Y + i] == thisMap.map[playControl.mapCur.X][playControl.mapCur.Y])
				sum[2]++;
			else
				break;
		}
	if (sum[2] + sum[0] >= 5)
		return true;

	//���.
	for (int i = 1; i <= 4; i++)
		if (playControl.mapCur.X + i >= 0)
			if (playControl.mapCur.Y - i >= 0)
			{
				if (thisMap.map[playControl.mapCur.X + i][playControl.mapCur.Y - i] == thisMap.map[playControl.mapCur.X][playControl.mapCur.Y])
					sum[3]++;
				else
					break;
			}
	//����.
	for (int i = 1; i <= 4; i++)
		if (playControl.mapCur.X - i >= 0)
			if (playControl.mapCur.Y + i >= 0)
			{
				if (thisMap.map[playControl.mapCur.X - i][playControl.mapCur.Y + i] == thisMap.map[playControl.mapCur.X][playControl.mapCur.Y])
					sum[3]++;
				else
					break;
			}
	if (sum[3] + sum[0] >= 5)
		return true;

	//��.
	for (int i = 1; i <= 4; i++)
		if (playControl.mapCur.X - i >= 0)
		{
			if (thisMap.map[playControl.mapCur.X - i][playControl.mapCur.Y] == thisMap.map[playControl.mapCur.X][playControl.mapCur.Y])
				sum[4]++;
			else
				break;
		}
	//��.
	for (int i = 1; i <= 4; i++)
		if (playControl.mapCur.X + i >= 0)
		{
			if (thisMap.map[playControl.mapCur.X + i][playControl.mapCur.Y] == thisMap.map[playControl.mapCur.X][playControl.mapCur.Y])
				sum[4]++;
			else
				break;
		}
	if (sum[4] + sum[0] >= 5)
		return true;

	return false;
}