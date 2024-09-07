using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPTrEngine
{
    public interface ISPTrLoop
    {
        public void Awake();
        public void OnInitialized();
        public void OnEnable();
        public void OnDisable();
        public void Start();
        public void FixedTick();
        public void Tick();
        public void AfterTick();
    }
}
