using System;
using System.Threading;

namespace PongClassic
{
    class Program
    {
        static int screenWidth = 50;
        static int screenHeight = 20;
        static char[,] screenBuffer = new char[screenHeight, screenWidth];
        static char[,] previousScreenBuffer = new char[screenHeight, screenWidth];

        static int paddleHeight = 4;
        static int paddleWidth = 1;
        static int leftPaddleX = 1;
        static int leftPaddleY = 8;
        static int rightPaddleX = screenWidth - 2;
        static int rightPaddleY = 8;

        static int ballX = screenWidth / 2;
        static int ballY = screenHeight / 2;
        static int ballDirX = 1;
        static int ballDirY = 1;

        static int leftPlayerScore = 0;
        static int rightPlayerScore = 0;

        static void Main(string[] args)
        {
            ShowWelcomeMessage();
            Console.ReadKey();

            while (true)
            {
                RunGame();

                Console.Clear();
                Console.WriteLine("Game Over!");
                Console.WriteLine($"Final Score - Left Player: {leftPlayerScore} | Right Player: {rightPlayerScore}");
                Console.WriteLine("Press R to restart or any other key to exit...");

                var key = Console.ReadKey();
                if (key.Key != ConsoleKey.R)
                {
                    break;
                }

                ResetGame();
            }
        }

        static void ShowWelcomeMessage()
        {

            string welcomeMessage = @"
__        __   _                            _           ____   ___   _   _  ____      
\ \      / /__| | ___ ___  _ __ ___   ___  | |_ ___    |  _ \ / _ \ | \ | |/ ___|
 \ \ /\ / / _ \ |/ __/ _ \| '_ ` _ \ / _ \ | __/ _ \   | |_) | | | ||  \| | |  _ 
  \ V  V /  __/ | (_| (_) | | | | | |  __/ | || (_) |  |  __/| |_| || . ` | |_| |
   \_/\_/ \___|_|\___\___/|_| |_| |_|\___|  \__\___( ) |_|    \___/ |_| \_|\____|
                                                   |/                  
           ";

            Console.Clear();
            Console.WriteLine(welcomeMessage);
            Console.WriteLine("Press any key to play...");
            DrawGameBoard();
        }

        static void DrawGameBoard()
        {
            Console.SetCursorPosition(0, 7);
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    Console.Write(screenBuffer[y, x]);
                }
                Console.WriteLine();
            }
        }

        static void RunGame()
        {
            bool gameOver = false;

            while (!gameOver)
            {
                HandleInput();
                UpdateBall();
                RenderScreen();

                Thread.Sleep(80);

                // Check for game over condition
                if (leftPlayerScore >= 10 || rightPlayerScore >= 10)
                {
                    gameOver = true;
                }
            }
        }

        static void ResetGame()
        {
            leftPlayerScore = 0;
            rightPlayerScore = 0;
            leftPaddleY = 8;
            rightPaddleY = 8;
            ballX = screenWidth / 2;
            ballY = screenHeight / 2;
            ballDirX = 1;
            ballDirY = 1;
            ResetScreenBuffers();
            Console.Clear();
        }

        static void HandleInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    //player 1
                    case ConsoleKey.W:
                        if (leftPaddleY > 1) leftPaddleY--;
                        break;
                    case ConsoleKey.S:
                        if (leftPaddleY < screenHeight - paddleHeight - 1) leftPaddleY++;
                        break;

                    //player 2
                    case ConsoleKey.UpArrow:
                        if (rightPaddleY > 1) rightPaddleY--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (rightPaddleY < screenHeight - paddleHeight - 1) rightPaddleY++;
                        break;
                }
            }
        }

        static void UpdateBall()
        {
            // Update ball position
            ballX += ballDirX;
            ballY += ballDirY;

            // Check for collision with top and bottom walls
            if (ballY <= 1 || ballY >= screenHeight - 2)
            {
                ballDirY = -ballDirY; // Reverse Y direction
            }

            // Check for collision with paddles
            if (ballX == leftPaddleX + paddleWidth && ballY >= leftPaddleY && ballY < leftPaddleY + paddleHeight)
            {
                ballDirX = -ballDirX; // Reverse X direction
            }
            if (ballX == rightPaddleX - paddleWidth && ballY >= rightPaddleY && ballY < rightPaddleY + paddleHeight)
            {
                ballDirX = -ballDirX; // Reverse X direction
            }

            // Check for game over and update score
            if (ballX <= 0)
            {
                rightPlayerScore++;
                ResetBall();
            }
            else if (ballX >= screenWidth - 1)
            {
                leftPlayerScore++;
                ResetBall();
            }
        }

        static void ResetBall()
        {
            ballX = screenWidth / 2;
            ballY = screenHeight / 2;
            ballDirX = -ballDirX; // Send the ball in the opposite direction
        }

        static void ResetScreenBuffers()
        {
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    screenBuffer[y, x] = ' ';
                    previousScreenBuffer[y, x] = ' ';
                }
            }
        }

        static void RenderScreen()
        {
            // Clear the screen buffer
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    screenBuffer[y, x] = ' ';
                }
            }

            // Draw paddles
            for (int i = 0; i < paddleHeight; i++)
            {
                if (leftPaddleY + i < screenHeight && leftPaddleY + i >= 0)
                    screenBuffer[leftPaddleY + i, leftPaddleX] = '|';

                if (rightPaddleY + i < screenHeight && rightPaddleY + i >= 0)
                    screenBuffer[rightPaddleY + i, rightPaddleX] = '|';
            }

            // Draw borders
            for (int x = 0; x < screenWidth; x++)
            {
                screenBuffer[0, x] = '-';
                screenBuffer[screenHeight - 1, x] = '-';
            }

            for (int y = 0; y < screenHeight; y++)
            {
                screenBuffer[y, 0] = '|';
                screenBuffer[y, screenWidth - 1] = '|';
            }

            // Draw ball
            if (ballY < screenHeight && ballY >= 0 && ballX < screenWidth && ballX >= 0)
                screenBuffer[ballY, ballX] = 'O';

            // Render only the parts of the screen that have changed
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; x++)
                {
                    if (screenBuffer[y, x] != previousScreenBuffer[y, x])
                    {
                        Console.SetCursorPosition(x, y + 8);
                        Console.Write(screenBuffer[y, x]);
                        previousScreenBuffer[y, x] = screenBuffer[y, x];
                    }
                }
            }

            // Display scores
            Console.SetCursorPosition(0, screenHeight + 8);
            Console.WriteLine($"Left Player: {leftPlayerScore} | Right Player: {rightPlayerScore}");
        }
    }
}
