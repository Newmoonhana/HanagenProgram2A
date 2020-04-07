#pragma once
#define _CRT_SECURE_NO_WARNINGS
#include <iostream>
using namespace std;
#include <Windows.h>
#include <string>;

#define LEFT 75
#define RIGHT 77
#define UP 72
#define DOWN 80
#define ENTER 'z'
#define RESET 'x'

#define blackDol "¡Û"
#define whiteDol "¡Ü"

class GameM
{
public:
	int beforeWinner = 1;
	int turn = 0;
	void Awake(GameM &GM);
	void GameRun(GameM &GM);
	void InputKey(GameM &GM);
	bool WinLose();
	void Retry(GameM &GM);
};