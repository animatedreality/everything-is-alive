using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Audio
{
    public class InstGroup : MonoBehaviour
    {
        public List<Instrument> instruments = new List<Instrument>();

        public string id;

        public InstGroup()
        {
            this.id = Util.AutoID();
        }

        public void AddInstrument(Instrument instrument)
        {
            instruments.Add(instrument);
        }

        public void RemoveInstrument(Instrument instrument)
        {
            instruments.Remove(instrument);
        }

        public void Schedule(int globalBeatIndex, double nextEventTime)
        {
            foreach (Instrument instrument in instruments)
            {
                instrument.Schedule(globalBeatIndex, nextEventTime);
            }
        }

        public void Play()
        {
            foreach (Instrument instrument in instruments)
            {
                instrument.Play();
            }
        }

        public void Stop()
        {
            foreach (Instrument instrument in instruments)
            {
                instrument.Stop();
            }
        }

    }
}