using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic.CompilerServices;

namespace ConsoleApp1
{
    /*
     * 2020_2A_중간고사_정지원_19032079
     * 테트리스를 구현했으며 여러 기능들을 많이 구현하다 버그를 일으키기보단 테트리스 초창기의 기본 규칙을 참고해 베이스 컨셉으로 만들었다.
     * 벽, 블럭 근처에서의 회전을 막는 등 예외사항에도 최대한 충실했으며(기본 베이스로 컨셉을 잡은 이유부터 버그가 거의 없게 하려고) 적어도 테스트 플레이에선 버그가 없었다.
     * 게임 오버 후 리트라이는 구현을 패스해서 게임 오버 시 콘솔 종료 글이 나와서 화면이 가려지지만 위로 스크롤 시 게임 오버 시 GameOver문구와 점수가 나온다.
     * 여러 cs파일로 분할해 하나의 코드처럼 쓰는 법을(헤더처럼) 몰라서 그냥 한 cs 파일에 여러 class를 넣어버린지라 이 부분에서 상당히 스파게티 코드가 되버렸다...
     * 점수 획득 방식은 줄을 없애는것 뿐, 한번에 많이 없앨 수록 기본 점수(100점) * 없앤 줄의 제곱 점수를 획득(즉 최대 4줄(I자 블럭 이용)이 한번에 최다 획득 점수).
     */
    class KeyCode   //사용할 키보드 키 단축 int.
    {
        public const int left = 75;
        public const int right = 77;
        public const int up = 72;
        public const int down = 80;
        public const int x = 88;
        public const int z = 90;
        public const int space = 32;
    }

    class Program   //Main 포함 클래스(전체적으로 돌아가는 함수 위주의 작성, 자주 쓰이는 변수를 저장해놓는다는 점에선 게임매니저 역할?)
    {
        public static Random r = new Random();  //랜덤 값 획득 용 랜덤 변수.

        public static Render render = new Render(); //출력 클래스.
        public static Map map = null;   //전체 맵(이중배열) 관련 클래스.
        public static Block []block = new Block[6]; //플레이어가 사용할 블록(0번이 사용중인 블록, next block를 표시하기 위해 5개로 설정했지만 구현 시간 상 패스.
        public static Block tempblock = new Block(-1);  //교체하며 사용할 수 있는 더미 블록.

        public static int key;  //입력한 키 값 저장소.
        public static bool gameover = false;    //게임오버 여부(FPS 클래스에서 판단).
        public static Score myScore = new Score();  //점수 클래스
        public static bool tempchange = false;  //교체 블럭 사용 시 해당 블럭을 사용 전까진 교체 불가 처리용 bool.
        static void Main(string[] args)
        {
            map = new Map();    //초기 Map 생성으로 인한 변수 생성 및 함수 순서 충돌 문제 방지로 Map은 Main 실행 후 new로 동적 할당.
            Awake();
            System.FPS();
        }

        static void Awake()
        {
            Console.CursorVisible = false;
            //사용 블럭 생성.
            for (int i = 0; i < 6; i++)
            {
                block[i] = new Block(r.Next(0, 7));
            }
            map.NextGo();
        }

        public static void Update()
        {
            key = Keyboard.InputKey();  //키값 할당.
            map.changeDir();    //회전 입력.
            map.moveXY();   //이동 입력.
            tempblock.SaveTempBlock();  //교체 블럭 입력.
        }

        public static void GameOver()   //게임 오버 처리.
        {
            Console.SetWindowPosition(0, 26);
            string outstring = "GameOver!\nMy Score: " + myScore.myScore;
            Console.WriteLine(outstring);
            Console.CursorVisible = true;
            Environment.Exit(0);
        }
    }

    class Map   //전체 맵(이중배열) 관련 클래스.
    {
        public static int mapX = 10, mapY = 20; //스테이지 크기
        public int [,]map = new int[mapX, mapY + 4];   //세로 축 상단 여백 공간으로 인해 index 4 = 0줄.
        Point playerXY; //공간 내 플레이어 좌표
        Point startPoint = new Point(mapX / 2, 0);  //플레이어 블록 생성 지점

        public Map()
        {
            for (int j = 0; j < mapY; j++)
                for (int i = 0; i < mapX; i++)
                    map[i, j] = 0;
        }

        public Point playerxy   //playerXY Point get set(왜 따로 만들었지 뭔가 이유가 있었는데)(아무튼 playerXY랑 역할은 같음).
        {
            get { return playerXY; }
            set { playerXY.x = value.x; playerXY.y = value.y; }
        }

        public void ReturnStartPoint()  //플레이어 좌표 초기값 화(다음 블럭, 교체 블럭 시 사용).
        {
            playerXY = new Point(startPoint.x, startPoint.y);
        }
        public void NextGo()    //다음 블럭
        {
            ReturnStartPoint();
            for (int i = 0; i < 5; i++)
                Program.block[i] = Program.block[i + 1];
            Program.block[5] = new Block(Program.r.Next(0, 7));
            MoveCol();
            if (!Program.map.thisiscol())
                Program.gameover = true;
            Program.tempchange = false;
        }
        int[] blockLRUD(int dir, int leftD = 4, int rightD = -1, int upD = 4, int downD = -1)   //블럭의 상하좌우 최소, 최대값 찾기 함수.
        {
            for (int j = 0; j < 4; j++)
                for (int i = 0; i < 4; i++)
                {
                    if (Program.block[0].Get_Block(dir, j, i) == 1)
                    {
                        if (leftD > i)
                            leftD = i;
                        if (rightD < i)
                            rightD = i;
                        if (upD > j)
                            upD = j;
                        downD = j;
                    }
                }
            int[] temp = { leftD, rightD, upD, downD };

            return temp;
        }
        public void MoveCol()   //가장자리 벗어난 경우 범위 조정.
        {
            int[] LRUD = blockLRUD(Program.block[0].Dir);
            Point pXY = playerXY;
            if (pXY.x - (2 - LRUD[0]) < 0)
                pXY.x = (2 - LRUD[0]);
            else if (pXY.x + LRUD[1] - 2 > mapX - 1)
                pXY.x = mapX - 1 - (LRUD[1] - 2);
            if (pXY.y - (3 - LRUD[2]) < 0)
                pXY.y = (3 - LRUD[2]);
            else if (pXY.y - (3 - LRUD[3]) > mapY - 1)
                pXY.y = mapY - 1 - (3 - LRUD[3]);

            playerXY = pXY;
        }
        bool MoveCheck(int dir) //블럭 이동 가능 여부 확인
        {
            int[] LRUD = blockLRUD(Program.block[0].Dir);
            switch (dir)
            {
                case KeyCode.left:
                    if (playerXY.x - (2 - LRUD[0]) > 0)
                    {
                        bool left = true;
                        for (int j = 0; j < 4; j++)
                        {
                            if (playerXY.y - (3 - LRUD[2]) + j >= 0 && playerXY.y - (3 - j) <= mapY - 1)
                                if (Program.block[0].Get_Block(Program.block[0].Dir, j, LRUD[0]) == 1)
                                    if (Program.map.map[playerXY.x - (2 - LRUD[0]) - 1, playerXY.y - (3 - j) + 4] == 1)
                                        left = false;
                        }
                        
                        return left;
                    }
                    break;
                case KeyCode.right:
                    if (playerXY.x + LRUD[1] - 2 < mapX - 1)
                    {
                        bool right = true;
                        for (int j = 0; j < 4; j++)
                        {
                            if (playerXY.y - (3 - LRUD[2]) + j >= 0 && playerXY.y - (3 - j) <= mapY - 1)
                                if (Program.block[0].Get_Block(Program.block[0].Dir, j, LRUD[1]) == 1)
                                    if (Program.map.map[playerXY.x + LRUD[1] - 2 + 1, playerXY.y - (3 - j) + 4] == 1)
                                        right = false;
                        }

                        return right;
                    }
                    break;
                case KeyCode.down:
                        bool down = true;
                        if (playerxy.y - (3 - LRUD[3]) >= mapY - 1)
                        {
                            down = false;
                        }
                        else
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (playerXY.x - 2 + i >= 0 && playerXY.x - 2 + i <= mapX - 1)
                                    if (Program.block[0].Get_Block(Program.block[0].Dir, LRUD[3], i) == 1)
                                        if (Program.map.map[playerXY.x - 2 + i, playerXY.y - (3 - LRUD[3]) + 5] == 1)
                                            down = false;
                            }
                        }
                            
                        return down;
            }
            return false;
        }
        public void DropIt()    //블럭 한칸 아래 이동
        {
            if (MoveCheck(KeyCode.down))
            {
                playerXY.y += 1;
            }
            else    //이동 실패(제일 밑바닥 || 아래 블럭에 막힘) 시
            {
                SetPlayerBlockOnMap();
                FullLineDelete();
                NextGo();
            }
            
            MoveCol();
        }
        void FullLineDelete()   //가득 찬 줄 제거 및 점수 추가 함수
        {
            int line = 0;
            for (int j = 0; j < mapY + 4; j++)
            {
                bool linefull = true;
                for (int i = 0; i < mapX; i++)
                {
                    if (Program.map.map[i, j] == 0)
                    {
                        linefull = false;
                        break;
                    }
                    else if (i == mapX - 1)
                    {
                        for (int l = j; l >= 0; l--)
                            for (int k = 0; k < mapX; k++)
                            {
                                if (l - 1 >= 0)
                                    Program.map.map[k, l] = Program.map.map[k, l - 1];
                                else
                                    Program.map.map[k, l] = 0;
                            }
                    }
                }
                if (linefull)
                    line++;
            }
            Program.myScore.myScore += Program.myScore.lineScore * line * line; //한번에 많은 라인 제거할 수록 점수 추가.
        }
        void SetPlayerBlockOnMap()  //플레이어 블록 스테이지에 안착 시 좌표값 등록.
        {
            for (int j = Program.map.playerxy.y - 3; j <= Program.map.playerxy.y; j++)
                for (int i = Program.map.playerxy.x - 2; i <= Program.map.playerxy.x + 1; i++)
                    if (i >= 0 && i < mapX)
                        if (j >= 0 && j < mapY)
                        {
                            if (Program.map.map[i, j + 4] == 0)
                                Program.map.map[i, j + 4] = Program.block[0].Get_Block(Program.block[0].Dir, j - (Program.map.playerxy.y - 3), i - (Program.map.playerxy.x - 2));
                        }
        }

        public void moveXY()    //플레이어 블록 이동.
        {
            if (Program.key != -1)
            {
                switch (Program.key)
                {
                    case KeyCode.left:
                        if (MoveCheck(KeyCode.left))
                            playerXY.x -= 1;
                        break;
                    case KeyCode.right:
                        if (MoveCheck(KeyCode.right))
                            playerXY.x += 1;
                        break;
                    case KeyCode.up:    //블럭 바로 드랍.
                        while(true)
                        {
                            bool moveCheck = MoveCheck(KeyCode.down);
                            DropIt();
                            if (!moveCheck)
                                return;
                        }
                    case KeyCode.down:
                        if (MoveCheck(KeyCode.down))
                            playerXY.y += 1;
                        break;
                }
                MoveCol();
            }
        }
        public void changeDir() //각도 변경.
        {
            if (Program.key == KeyCode.z || Program.key == KeyCode.x)
            {
                int dirtemp = Program.block[0].Dir;
                switch (Program.key)
                {
                    case KeyCode.z:
                        Program.block[0].Dir -= 1;
                        break;
                    case KeyCode.x:
                        Program.block[0].Dir += 1;
                        break;
                }

                if (Program.block[0].Dir < 0)
                    Program.block[0].Dir = 3;
                else if (Program.block[0].Dir >= 4)
                    Program.block[0].Dir = 0;

                if (thisiscol())
                    MoveCol();
                else    //충돌 오류 발생 시 다시 원상복귀(회전 위치에 다른 블럭이 있는 경우)
                    Program.block[0].Dir = dirtemp;
            }
        }
        public bool thisiscol() //충돌 여부 체크 함수(회전 시 등에 사용)
        {
            bool col = true;
            for (int j = 0; j < 4; j++)
                for (int i = 0; i < 4; i++)
                    if (playerXY.x - 2 + i >= 0 && playerXY.x - 2 + i <= mapX - 1)
                        if (playerXY.y - 3 + j >= 0 && playerXY.y - 3 + j <= mapY - 1)
                            if (Program.map.map[playerXY.x - 2 + i, playerXY.y - 3 + j + 4] == 1 && Program.block[0].Get_Block(Program.block[0].Dir, j, i) == 1)
                                col = false;
            return col;
        }
    }

    class Keyboard  //키 입력 클래스.
    {
        public static int InputKey()
        {
            ConsoleKeyInfo key = new ConsoleKeyInfo();// default(ConsoleKeyInfo);
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        return KeyCode.up;
                    case ConsoleKey.DownArrow:
                        return KeyCode.down;
                    case ConsoleKey.LeftArrow:
                        return KeyCode.left;
                    case ConsoleKey.RightArrow:
                        return KeyCode.right;
                    case ConsoleKey.Z:
                        return KeyCode.z;
                    case ConsoleKey.X:
                        return KeyCode.x;
                    case ConsoleKey.Spacebar:
                        return KeyCode.space;
                }
            }
            return -1;  //미입력 = -1
        }
    }

    class Block //테트리스 블럭 클래스.
    {
        int whatBlock;  //블럭 종류
        int[][][] block;    //회전방향/가로/세로.
        int dir = 0;    //회전방향.
        public Block(int what)
        {
            whatBlock = what;
            SetBlock(whatBlock);
        }
        public int Dir  //각도 변수(direction)(0: 0도 1: 90도 2: 180도 3: 270도).
        {
            get { return dir; }
            set { dir = value; }
        }
        void SetBlock(int what)
        {
            whatBlock = what;
            //테트리스 블럭 4방향/x/y로 삼중배열 사용.
            switch (what)
            {
                case 0: //I블럭.
                    block = new int[4][][]
                    {
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 1, 1, 1, 1 }
                },
                new int[4][]
                {
                new int[4]{ 0, 1, 0, 0 },
                new int[4]{ 0, 1, 0, 0 },
                new int[4]{ 0, 1, 0, 0 },
                new int[4]{ 0, 1, 0, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 1, 1, 1, 1 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 1, 0 },
                new int[4]{ 0, 0, 1, 0 },
                new int[4]{ 0, 0, 1, 0 },
                new int[4]{ 0, 0, 1, 0 }
                }
                    };
                    break;
                case 1://O블럭.
                    block = new int[4][][]{
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 0, 0, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 0, 0, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 0, 0, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 0, 0, 0 }
                }
            };
                    break;
                case 2://Z블럭.
                    block = new int[4][][]{
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 1, 1, 0, 0 },
                new int[4]{ 0, 1, 1, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 1, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 1, 0, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 1, 1, 0, 0 },
                new int[4]{ 0, 1, 1, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 1, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 1, 0, 0 }
                }
                    };
                    break;
                case 3://S블럭.
                    block = new int[4][][]{
                        new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 1, 1 },
                new int[4]{ 0, 1, 1, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 0, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 0, 1, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 1, 1 },
                new int[4]{ 0, 1, 1, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 0, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 0, 1, 0 }
                }
            };
                    break;
                case 4://J블럭.
                    block = new int[4][][]{
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 0, 0 },
                new int[4]{ 0, 1, 1, 1 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 1, 0, 0 },
                new int[4]{ 0, 1, 0, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 1, 1, 1, 0 },
                new int[4]{ 0, 0, 1, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 1, 0 },
                new int[4]{ 0, 0, 1, 0 },
                new int[4]{ 0, 1, 1, 0 }
                }
            };
                    break;
                case 5://L블럭.
                    block = new int[4][][]{
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 1, 0 },
                new int[4]{ 1, 1, 1, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 0, 0 },
                new int[4]{ 0, 1, 0, 0 },
                new int[4]{ 0, 1, 1, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 1, 1 },
                new int[4]{ 0, 1, 0, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 0, 1, 0 },
                new int[4]{ 0, 0, 1, 0 }
                }
            };
                    break;
                case 6://T블럭.
                    block = new int[4][][]{
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 0, 0 },
                new int[4]{ 1, 1, 1, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 0, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 1, 0, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 1, 1, 1 },
                new int[4]{ 0, 0, 1, 0 }
                },
                new int[4][]
                {
                new int[4]{ 0, 0, 0, 0 },
                new int[4]{ 0, 0, 1, 0 },
                new int[4]{ 0, 1, 1, 0 },
                new int[4]{ 0, 0, 1, 0 }
                }
            };
                    break;
            }
        }

        public int Get_Index()
        {
            return whatBlock;
        }
        public int Get_Block(int index, int indexY, int indexX)
        {
            return block[index][indexY][indexX];
        }
        public void Set_Block(int value)
        {
            whatBlock = value;
            SetBlock(value);
        }

        public void SaveTempBlock()  //교체 블럭 사용 시 해당 블럭을 사용 전까진 교체 불가
        {
            if (!Program.tempchange)
            {
                if (Program.key != -1)
                {
                    if (Program.key == KeyCode.space)
                    {
                        //교체블럭 저장.
                        int tempWhatBlock = whatBlock;
                        whatBlock = Program.block[0].whatBlock;
                        this.SetBlock(whatBlock);

                        if (tempWhatBlock != -1)
                        {
                            //0번 블럭(사용 블럭) 교체 후 좌표값 초기화.
                            Program.block[0].SetBlock(tempWhatBlock);
                            Program.map.ReturnStartPoint();
                            Program.map.MoveCol();
                            if (!Program.map.thisiscol())
                                Program.gameover = true;
                        }
                        else    //교체블럭 비존재(null).
                        {
                            Program.map.NextGo();
                        }
                        Program.tempchange = true;
                    }
                }
            }
        }
    }

    class Render //출력 클래스.
    {
        public void Rendering()
        {
            Console.SetCursorPosition(0, 0);
            string outWindow = "┌";
            for (int i = 0; i < Map.mapX; i++)
                outWindow += "──";
            outWindow += "┐";
            for (int j = 0; j < Map.mapY; j++)
            {
                outWindow += "\n│";
                for (int i = 0; i < Map.mapX; i++)
                {
                    if (i >= Program.map.playerxy.x - 2 && i <= Program.map.playerxy.x + 1 && j >= Program.map.playerxy.y - 3 && j <= Program.map.playerxy.y)
                    {
                        if (Program.map.map[i, j + 4] == 0 && Program.block[0].Get_Block(Program.block[0].Dir, j - (Program.map.playerxy.y - 3), i - (Program.map.playerxy.x - 2)) == 1)
                            outWindow += "□";
                        else if (Program.map.map[i, j + 4] == 1)
                            outWindow += "■";
                        else
                            outWindow += "  ";

                    }
                    else if (Program.map.map[i, j + 4] == 1)
                    {
                        outWindow += "■";
                    }
                    else
                        outWindow += "  ";
                }
                outWindow += "│";
                if (j >= 0 && j <= 4)
                {
                    if (j == 0)
                        outWindow += "\t<temp block>";
                    else if (Program.tempblock.Get_Index() != -1)
                    {
                        outWindow += "\t  ";
                        for (int i = 0; i < 4; i++)
                        {
                            if (Program.tempblock.Get_Block(0, j - 1, i) == 1)
                                outWindow +=  "■";
                            else
                                outWindow += "  ";
                        }
                    }
                }
            }
            outWindow += "\n└";
            for (int i = 0; i < Map.mapX; i++)
                outWindow += "──";
            outWindow += "┘";
            outWindow += "\nscore:  ";
            outWindow += Program.myScore.myScore;
            outWindow += "\n게임 방법: \n←↓→: 이동  ↑: 블럭 드랍  z,x: 회전  space: 더미블록 저장";

            Console.Write(outWindow);
        }
    }

    class System    //외부적에 가까운 시스템 관련 클래스(였으나 결국 함수 한개...)
    {
        public static void FPS()
        {
            int currentTick, previousTick, tempTick;    //다음 틱, 현재 틱, 저장 틱.
            int DelayTick = (1000 / 60); //60 fps.
            tempTick = Environment.TickCount;
            currentTick = tempTick + DelayTick;

            int DelayTickSec = 1000; //1초.
            int SecTick = tempTick + DelayTickSec;
            while (true)
            {
                previousTick = Environment.TickCount;
                // 60 프레임으로 호출
                if (previousTick >= currentTick)
                {
                    Program.Update();
                    currentTick = previousTick + DelayTick;
                }

                // 1초마다 호출
                if (SecTick <= previousTick)
                {
                    Program.map.DropIt();
                    SecTick = previousTick + DelayTickSec;
                }
                Program.render.Rendering(); //출력.
                if (Program.gameover)   //게임오버 처리 시 탈출.
                    break;
            }
            Program.GameOver(); //모든 함수 종료 후 게임오버 처리.
        }
    }

    class Point //포인트 클래스(없어서 만듬)
    {
        public int x, y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }
        public static Point operator -(Point a, Point b)
        {
            int x, y;
            x = (a.x > b.x ? a.x - b.x : b.x - a.x);
            y = (a.y > b.y ? a.y -b.y : b.y - a.y);
            return new Point(x, y);
        }
    }

    class Score  //점수 클래스(점수 전용 함수 생성에 대비해 만든 클래스지만 lineScore 말고는 의미가 없었는듯)
    {
        public int myScore
        {
            get;
            set;
        } = 0;
        public int lineScore
        {
            get;
            set;
        } = 100;
    }
}
