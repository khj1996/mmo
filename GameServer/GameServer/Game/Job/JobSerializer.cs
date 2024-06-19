using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Game
{
    public class JobSerializer
    {
        //시간에 따른 작업
        JobTimer _timer = new JobTimer();

        //일반 작업
        Queue<IJob> _jobQueue = new Queue<IJob>();

        //락
        object _lock = new object();

        //TODO : 기능 확인 필요
        bool _flush = false;

        #region 타이머 작업

        public IJob PushAfter(int tickAfter, Action action)
        {
            return PushAfter(tickAfter, new Job(action));
        }

        public IJob PushAfter<T1>(int tickAfter, Action<T1> action, T1 t1)
        {
            return PushAfter(tickAfter, new Job<T1>(action, t1));
        }

        public IJob PushAfter<T1, T2>(int tickAfter, Action<T1, T2> action, T1 t1, T2 t2)
        {
            return PushAfter(tickAfter, new Job<T1, T2>(action, t1, t2));
        }

        public IJob PushAfter<T1, T2, T3>(int tickAfter, Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            return PushAfter(tickAfter, new Job<T1, T2, T3>(action, t1, t2, t3));
        }

        public IJob PushAfter(int tickAfter, IJob job)
        {
            _timer.Push(job, tickAfter);
            return job;
        }

        #endregion

        #region 일반 작업

        public void Push(Action action)
        {
            Push(new Job(action));
        }

        public void Push<T1>(Action<T1> action, T1 t1)
        {
            Push(new Job<T1>(action, t1));
        }

        public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
        {
            Push(new Job<T1, T2>(action, t1, t2));
        }

        public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            Push(new Job<T1, T2, T3>(action, t1, t2, t3));
        }

        public void Push(IJob job)
        {
            lock (_lock)
            {
                _jobQueue.Enqueue(job);
            }
        }

        //업데이트마다 실행
        public void Flush()
        {
            //시간에 따른 작업
            _timer.Flush();

            while (true)
            {
                //작업 획득
                IJob job = Pop();
                //작업이 없는 경우 반환
                if (job == null)
                    return;

                //작업 실행
                job.Execute();
            }
        }

        //작업 반환
        IJob Pop()
        {
            //동시 접근 방지 락
            lock (_lock)
            {
                //작업이 없는 경우
                if (_jobQueue.Count == 0)
                {
                    _flush = false;
                    return null;
                }

                //작업 반환
                return _jobQueue.Dequeue();
            }
        }

        #endregion
    }
}