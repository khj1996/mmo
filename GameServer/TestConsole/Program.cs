using System;

public class Solution
{
    public const double BallDiameter = 2;
    

    public int solution(double[,] objectBallPosList, double[] hitVector)
    {
        int answer = -1;

        double magnitude = Math.Sqrt(hitVector[0] * hitVector[0] + hitVector[1] * hitVector[1]);
        double dirX = hitVector[0] / magnitude;
        double dirY = hitVector[1] / magnitude;


        double minDistance = double.MaxValue;

        for (int i = 0; i < objectBallPosList.GetLength(0); i++)
        {
            double posX = objectBallPosList[i, 0];
            double posY = objectBallPosList[i, 1];

            double a = dirX * dirX + dirY * dirY;
            double b = 2 * (dirX * (-posX) + dirY * (-posY));
            double c = posX * posX + posY * posY - (BallDiameter * BallDiameter);

            double rms = b * b - 4 * a * c;

            if (rms >= 0)
            {
                double t1 = (-b - Math.Sqrt(rms)) / (2 * a);
                double t2 = (-b + Math.Sqrt(rms)) / (2 * a);

                double t = Math.Min(t1 >= 0 ? t1 : double.MaxValue, t2 >= 0 ? t2 : double.MaxValue);

                if (t2 < minDistance)
                {
                    minDistance = t;
                    answer = i;
                }
            }
        }

        return answer;
    }
}