using System;
using System.Collections.Generic;

namespace SPTrApp.ExampleApp
{
    public class AsyncSubRoutine
    {
        Delegate[] m_Routines;

        Queue<Delegate> m_RoutineQueue = new Queue<Delegate>();

        public object? Current { private set; get; }

        public AsyncSubRoutine(Delegate[] routines)
        {
            m_Routines = routines;
            Reset();
        }

        public bool MoveNext(params object[] args)
        {
            if(m_RoutineQueue.Count > 0)
            {
                var routine = m_RoutineQueue.Dequeue();

                try
                {
                    Current = routine.DynamicInvoke(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    m_RoutineQueue.Clear();
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        public void Reset()
        {
            m_RoutineQueue.Clear();

            foreach (Delegate function in m_Routines)
                m_RoutineQueue.Enqueue(function);
        }
    }
}
