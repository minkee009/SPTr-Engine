using System;
using System.Collections.Generic;

namespace SPTrApp.ExampleApp
{
    public class AsyncSubRoutine
    {
        Delegate[] m_Routines;

        int m_index = -1;

        public object? Current { private set; get; }

        public AsyncSubRoutine(Delegate[] routines)
        {
            m_Routines = routines;
            Reset();
        }

        public bool MoveNext(params object[] args)
        {
            if(m_index < m_Routines.Length)
            {
                var routine = m_Routines[m_index++];

                try
                {
                    Current = routine.DynamicInvoke(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    m_index = -1;
                    return false;
                }
                return true;
            }
            else
                return false;
        }

        public void Reset()
        {
            m_index = 0;
        }
    }
}
