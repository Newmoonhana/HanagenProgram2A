#include "map.h"

void makeMap::ResetMap()
{
	mapCol = sizeof(map[0]) / sizeof(int) + 1;
	mapRow = sizeof(map) / sizeof(map[0]) + 1;

	for (int i = 0; i < mapRow - 1; i++)
		for (int j = 0; j < mapCol - 1; j++)
			map[j][i] = 0;
}

void makeMap::drawMap(Player p1, Player p2)
{
	drawCanvas(p1, p2);
	drawPlayerStat(p1, p2);
}

void makeMap::drawCanvas(Player p1, Player p2)
{
	system("cls");
	for (int i = 0; i <= mapRow; i++)
	{
		for (int j = 0; j <= mapCol; j++)
		{
			if (i == 0 || j == 0 || i == mapRow || j == mapCol)
			{
				if (!(i == 0 && j == 0) && !(i == mapRow && j == mapCol))
					cout << " ";
			}
			else
			{
				switch (map[j - 1][i - 1])
				{
				case 1:
					cout << p1.dol;
					break;
				case 2:
					cout << p2.dol;
					break;
				default:
					Canvas(i, j);
					break;
				}
			}
		}
		cout << endl;
	}
		
	cout << endl;

}

void makeMap::Canvas(int i, int j)
{
	if (i == 1 && j == 1)
		cout << "┌";
	else if (i == 1 && j == mapCol - 1)
		cout << "┐";
	else if (i == mapRow - 1 && j == 1)
		cout << "└";
	else if (i == mapRow - 1 && j == mapCol - 1)
		cout << "┘";
	else if (i == 1)
		cout << "┬";
	else if (i == mapRow - 1)
		cout << "┴";
	else if (j == 1)
		cout << "├";
	else if (j == mapCol - 1)
		cout << "┤";
	else
		cout << "┼";
}

void makeMap::drawPlayerStat(Player p1, Player p2)
{
	MapWarp(playControl->mapCur);
	switch (map[playControl->mapCur.X][playControl->mapCur.Y])
	{
	case 0:
		cout << "◈";
		break;
	case 1:
	case 2:
		cout << "⊙";
		break;
	}

	COORD p1cur = { 0, mapRow };
	SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), p1cur);
	cout << "  조작법 : 방향키, 확인 : z키, 리셋 : x키" << endl;
	//차례 표시
	if (GM->turn % 2 == (GM->beforeWinner == 1 ? 0 : 1))	//흑돌 차례 시.
	{
		if (strcmp(p1.dol, blackDol) == 0)
			cout << ' ' << p1.dol << ' ' << p1.name;
		else
			cout << ' ' << p2.dol << ' ' << p2.name;
	}
	else
	{
		if (strcmp(p1.dol, whiteDol) == 0)
			cout << ' ' << p1.dol << ' ' << p1.name;
		else
			cout << ' ' << p2.dol << ' ' << p2.name;
	}
	
	cout << " 차례 : (" << (char)(playControl->mapCur.X + 1 + 64) << ", " << mapRow - playControl->mapCur.Y - 1 << ")  " << endl << endl;	//바둑판 좌표 표시 기준.
	
	//스테이더스 표시.
	char ch[256] = "  이름 : ";
	cout << ch << p1.name << endl;
	RightStringNull(ch, p2.name);
	strcpy(ch, "  Win : ");
	cout << ch << p1.winNum << endl;
	char p2ch[256];
	sprintf(p2ch , "%d", p2.winNum);
	RightStringNull(ch, p2ch);
	strcpy(ch, "  돌 : ");
	cout << ch << p1.dol << endl;
	RightStringNull(ch, p2.dol);
}

void makeMap::RightStringNull(char ch[], char in[])
{
	char sum[256];
	strcpy(sum, ch);
	strcat(sum, in);
	for (int i = 0; i < mapCol * 2 - strlen(sum); i++)
		cout << ' ';
	cout << sum << endl;
}

void makeMap::MapWarp(COORD mapCur)
{
	COORD mapcur = mapCur;
	mapcur.X = mapcur.X * 2 + 1;
	mapcur.Y += 1;
	SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), mapcur);
}

void Control::GotoXY(int key, makeMap map, Player p1, Player p2)
{
	map.MapWarp(mapCur);
	switch (map.map[mapCur.X][mapCur.Y])
	{
	case 0:
		map.Canvas(mapCur.Y + 1, mapCur.X + 1);
		break;
	case 1:
		cout << p1.dol;
		break;
	case 2:
		cout << p2.dol;
		break;
	}
	switch (key)
	{
	case LEFT:
		this->mapCur.X -= 1;
		break;
	case RIGHT:
		this->mapCur.X += 1;
		break;
	case UP:
		this->mapCur.Y -= 1;
		break;
	case DOWN:
		this->mapCur.Y += 1;
		break;
	}

	if (this->mapCur.X < 0)
		this->mapCur.X = 0;
	if (this->mapCur.Y < 0)
		this->mapCur.Y = 0;
	if (this->mapCur.X > map.mapCol - 2)
		this->mapCur.X = map.mapCol - 2;
	if (this->mapCur.Y > map.mapRow - 2)
		this->mapCur.Y = map.mapRow - 2;
}

bool Control::Enter(makeMap &map, int turn)
{
	if (map.map[this->mapCur.X][this->mapCur.Y] == 0)
	{
		map.map[this->mapCur.X][this->mapCur.Y] = turn % 2 + 1;
		return false;
	}
	
	return true;
}