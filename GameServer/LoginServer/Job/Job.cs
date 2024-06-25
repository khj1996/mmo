using System;
using System.Collections.Generic;
using System.Text;

namespace LoginServer.Game
{
    //작업의 추상클래스
    public abstract class IJob
    {
        public abstract void Execute();
        public bool Cancel { get; set; } = false;
    }

    //매개변수가 없는 작업
    public class Job : IJob
    {
        Action _action;

        public Job(Action action)
        {
            _action = action;
        }

        public override void Execute()
        {
            if (Cancel == false)
                _action.Invoke();
        }
    }

    //매개변수가 1개인 작업
    public class Job<T1> : IJob
    {
        Action<T1> _action;
        T1 _t1;

        public Job(Action<T1> action, T1 t1)
        {
            _action = action;
            _t1 = t1;
        }

        public override void Execute()
        {
            if (Cancel == false)
                _action.Invoke(_t1);
        }
    }

    //매개변수가 2개인 작업
    public class Job<T1, T2> : IJob
    {
        Action<T1, T2> _action;
        T1 _t1;
        T2 _t2;

        public Job(Action<T1, T2> action, T1 t1, T2 t2)
        {
            _action = action;
            _t1 = t1;
            _t2 = t2;
        }

        public override void Execute()
        {
            if (Cancel == false)
                _action.Invoke(_t1, _t2);
        }
    }

    //매개변수가 3개인 작업
    public class Job<T1, T2, T3> : IJob
    {
        Action<T1, T2, T3> _action;
        T1 _t1;
        T2 _t2;
        T3 _t3;

        public Job(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            _action = action;
            _t1 = t1;
            _t2 = t2;
            _t3 = t3;
        }

        public override void Execute()
        {
            if (Cancel == false)
                _action.Invoke(_t1, _t2, _t3);
        }
    }
}