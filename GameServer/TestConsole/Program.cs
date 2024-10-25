using System;
using System.Collections.Generic;
using System.Linq;

public class Solution
{
    public string[] solution(string[,] plans)
    {
        List<string> answer = new List<string>();
        var data = new List<(string name, int start, int cost)>();

        for (int i = 0; i < plans.GetLength(0); i++)
        {
            var name = plans[i, 0];
            var splitTime = plans[i, 1].Split(":");
            int start = Convert.ToInt32(splitTime[0]) * 60 + Convert.ToInt32(splitTime[1]);
            int cost = Convert.ToInt32(plans[i, 2]);

            data.Add((name, start, cost));
        }

        data = data.OrderBy(p => p.start).ToList();

        Stack<(string name, int start, int cost)> stack = new Stack<(string name, int start, int cost)>();

        int currentTime = 0;

        foreach (var current in data)
        {
            //현재 진행중인 과제가 있을경우
            while (stack.Count > 0)
            {
                //진행중인 과제
                var process = stack.Peek();

                //현재 진행중인 과제가 새로운 과제보다 먼저 끝날경우
                if (currentTime + process.cost <= current.start)
                {
                    answer.Add(stack.Pop().name);
                    currentTime += process.cost;
                }
                else
                {
                    //소모시간 제외
                    var renew = stack.Pop();
                    renew.cost -= current.start - currentTime;
                    stack.Push(renew);
                    break;
                }
            }

            stack.Push(current);
            currentTime = current.start;
        }

        while (stack.Count > 0)
        {
            answer.Add(stack.Pop().name);
        }

        return answer.ToArray();
    }
}