using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace LoginServer.Game
{
    //우선순위 작업(시간 설정 작업)
    //상속은 CompareTo 사용을 위한 상속
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        //실행 타이밍 설정
        public int execTick;

        //타이머 종료시 실행 작업
        public IJob job;

        //우선순위
        public int CompareTo(JobTimerElem other)
        {
            return other.execTick - execTick;
        }
    }

    public class JobTimer
    {
        //우선순위큐
        PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();

        //락
        object _lock = new object();

        //작업 추가
        public void Push(IJob job, int tickAfter = 0)
        {
            //작업 
            JobTimerElem jobElement;
            //실행 타이밍 설정
            jobElement.execTick = System.Environment.TickCount + tickAfter;
            //실행 작업
            jobElement.job = job;

            //락
            lock (_lock)
            {
                _pq.Push(jobElement);
            }
        }

        //작업 실행
        public void Flush()
        {
            //실행해야하는 작업이 있는경우 계속해서 실행
            while (true)
            {
                //현재 틱
                int now = System.Environment.TickCount;

                JobTimerElem jobElement;

                lock (_lock)
                {
                    //실행해야하는 작업이 없는 경우
                    if (_pq.Count == 0)
                        break;

                    //다음 작업 데이터 확인
                    jobElement = _pq.Peek();
                    //다음 작업이 시간이 안된경우 탈출
                    if (jobElement.execTick > now)
                        break;

                    //다음 작업이 실행 타이밍인 경우 팝
                    _pq.Pop();
                }

                //다음 작업 실행
                jobElement.job.Execute();
            }
        }
    }
}