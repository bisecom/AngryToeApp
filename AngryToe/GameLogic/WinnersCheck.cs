using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngryToe.GameLogic
{
    public static class WinnersCheck
    {
        static WinnersCheck() { }

        //Logic for 5 cells field
        //--------------------------------------------------------------------------
        public static int DglRightToLeft5(int[,] matrix, int N)
        {
            int i = 0, j = 0;
            List<int> player1;
            List<int> player2;
            // Loop to print each diagonal
            for (int cnt = 0; cnt < 2 * N - 1; cnt++)
            {
                player1 = new List<int>();
                player2 = new List<int>();
                if (cnt < N)
                {
                    i = cnt;
                    j = 0;
                }
                else
                {
                    i = N - 1;
                    j = (cnt + 1) % N;
                }
                while (i >= 0 && j < N)
                {

                    player1.Add(matrix[i, j]);
                    if (player1.Count == 3 && player1[0] == 1 && player1[1] == 1 && player1[2] == 1 ||
                      player1.Count == 4 && player1[0] == 1 && player1[1] == 1 && player1[2] == 1 ||
                      player1.Count == 4 && player1[1] == 1 && player1[2] == 1 && player1[3] == 1 ||
                      player1.Count == 5 && player1[0] == 1 && player1[1] == 1 && player1[2] == 1 ||
                      player1.Count == 5 && player1[4] == 1 && player1[3] == 1 && player1[2] == 1 ||
                      player1.Count == 5 && player1[1] == 1 && player1[2] == 1 && player1[3] == 1)
                    {
                        return 1; 
                    }
                    if (player1.Count == 3 && player1[0] == 2 && player1[1] == 2 && player1[2] == 2 ||
                          player1.Count == 4 && player1[0] == 2 && player1[1] == 2 && player1[2] == 2 ||
                          player1.Count == 4 && player1[1] == 2 && player1[2] == 2 && player1[3] == 2 ||
                          player1.Count == 5 && player1[0] == 2 && player1[1] == 2 && player1[2] == 2 ||
                          player1.Count == 5 && player1[4] == 2 && player1[3] == 2 && player1[2] == 2 ||
                          player1.Count == 5 && player1[1] == 2 && player1[2] == 2 && player1[3] == 2)
                    {
                        //Console.WriteLine("Player 2 Wins Diag !!"); break;
                        return 2;
                    }

                    i--;
                    j++;
                }
            }
            return 0;
        }

        public static int DglLeftToRight5(int[,] matrix)
        {
            if (matrix[0, 2] == 1 && matrix[1, 3] == 1 && matrix[2, 4] == 1 ||
                matrix[0, 1] == 1 && matrix[1, 2] == 1 && matrix[2, 3] == 1 ||
                   matrix[1, 2] == 1 && matrix[2, 3] == 1 && matrix[3, 4] == 1 ||
                matrix[0, 0] == 1 && matrix[1, 1] == 1 && matrix[2, 2] == 1 ||
                matrix[1, 1] == 1 && matrix[2, 2] == 1 && matrix[3, 3] == 1 ||
                matrix[2, 2] == 1 && matrix[3, 3] == 1 && matrix[4, 4] == 1 ||
                matrix[1, 0] == 1 && matrix[2, 1] == 1 && matrix[3, 2] == 1 ||
                matrix[2, 1] == 1 && matrix[3, 2] == 1 && matrix[4, 3] == 1 ||
                matrix[2, 1] == 1 && matrix[3, 2] == 1 && matrix[4, 3] == 1 ||
                matrix[2, 0] == 1 && matrix[3, 1] == 1 && matrix[4, 2] == 1
               )
            {
                //Console.WriteLine("Player 1 Wins Diag !!");
                return 1;
            }

            if (matrix[0, 2] == 2 && matrix[1, 3] == 2 && matrix[2, 4] == 2 ||
                matrix[0, 1] == 2 && matrix[1, 2] == 2 && matrix[2, 3] == 2 ||
                   matrix[1, 2] == 2 && matrix[2, 3] == 2 && matrix[3, 4] == 2 ||
                matrix[0, 0] == 2 && matrix[1, 1] == 2 && matrix[2, 2] == 2 ||
                matrix[1, 1] == 2 && matrix[2, 2] == 2 && matrix[3, 3] == 2 ||
                matrix[2, 2] == 2 && matrix[3, 3] == 2 && matrix[4, 4] == 2 ||
                matrix[1, 0] == 2 && matrix[2, 1] == 2 && matrix[3, 2] == 2 ||
                matrix[2, 1] == 2 && matrix[3, 2] == 2 && matrix[4, 3] == 2 ||
                matrix[2, 1] == 2 && matrix[3, 2] == 2 && matrix[4, 3] == 2 ||
                matrix[2, 0] == 2 && matrix[3, 1] == 2 && matrix[4, 2] == 2
               )
            {
                //Console.WriteLine("Player 2 Wins Diag !!");
                return 2;
            }
            return 0;
        }

        public static int Check_all_rows5(int[,] matrix)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (matrix[i, j] == 1 && j - 1 >= 0 && matrix[i, j - 1] == 1 && j - 2 >= 0 && matrix[i, j - 2] == 1)
                    {
                        //Console.WriteLine("Player 1 Wins Rows !!");
                        //break;
                        return 1;
                    }

                    if (matrix[i, j] == 2 && j - 1 >= 0 && matrix[i, j - 1] == 2 && j - 2 >= 0 && matrix[i, j - 2] == 2)
                    {
                        //Console.WriteLine("Player 2 Wins Rows !!");
                        //break;
                        return 2;
                    }
                }
            }


            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (matrix[i, j] == 1 && i - 1 >= 0 && matrix[i - 1, j] == 1 && i - 2 >= 0 && matrix[i - 2, j] == 1)
                    {
                        //Console.WriteLine("Player 1 Wins Columns !!");
                        //break;
                        return 1;
                    }

                    if (matrix[i, j] == 2 && i - 1 >= 0 && matrix[i - 1, j] == 2 && i - 2 >= 0 && matrix[i - 2, j] == 2)
                    {
                        //Console.WriteLine("Player 2 Wins Columns !!");
                        //break;
                        return 2;
                    }
                }
            }
            return 0;
        }

        //Logic for 3 cells field
        //--------------------------------------------------------------------------
        public static int Check_all_rows(int[,] matrix)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (matrix[i, j] == 1 && j - 1 >= 0 && matrix[i, j - 1] == 1 && j - 2 >= 0 && matrix[i, j - 2] == 1)
                    {
                        //Console.WriteLine("Player 1 Wins Rows !!");
                        //break;
                        return 1;
                    }

                    if (matrix[i, j] == 2 && j - 1 >= 0 && matrix[i, j - 1] == 2 && j - 2 >= 0 && matrix[i, j - 2] == 2)
                    {
                        //Console.WriteLine("Player 2 Wins Rows !!");
                        //break;
                        return 2;
                    }
                }
            }


            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (matrix[i, j] == 1 && i - 1 >= 0 && matrix[i - 1, j] == 1 && i - 2 >= 0 && matrix[i - 2, j] == 1)
                    {
                        //Console.WriteLine("Player 1 Wins Columns !!");
                        //break;
                        return 1;
                    }

                    if (matrix[i, j] == 2 && i - 1 >= 0 && matrix[i - 1, j] == 2 && i - 2 >= 0 && matrix[i - 2, j] == 2)
                    {
                        //Console.WriteLine("Player 2 Wins Columns !!");
                        //break;
                        return 2;
                    }
                }
            }
            return 0;
        }

        public static int Check_all_diagonals3(int[,] matrix)
        {
            int countforP11 = 0;
            int countforP12 = 0;
            int countforP21 = 0;
            int countforP22 = 0;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == j && matrix[j, i] == 1)
                        countforP11++;

                    if (i + j == 2 && matrix[j, i] == 1)
                        countforP12++;

                    if (i == j && matrix[j, i] == 2)
                        countforP21++;

                    if (i + j == 2 && matrix[j, i] == 2)
                        countforP22++;

                    if (countforP11 == 3)
                    {
                        //Console.WriteLine("Player 1 Wins Diagon!!");
                        //break;
                        return 1;
                    }
                    if (countforP12 == 3)
                    {
                        //Console.WriteLine("Player 1 Wins Diagon!!");
                        //break;
                        return 1;
                    }
                    if (countforP21 == 3)
                    {
                        //Console.WriteLine("Player 2 Wins Diagon!!");
                        //break;
                        return 2;
                    }
                    if (countforP22 == 3)
                    {
                        //Console.WriteLine("Player 2 Wins Diagon!!");
                        //break;
                        return 2;
                    }
                }
            }
            return 0;
        }

        

    }
}
