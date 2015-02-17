// Copyright 2014 The Rector & Visitors of the University of Virginia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// TODO:  Do these probes stay on when the device is asleep?

using SensusUI.UiProperties;
using System;

namespace SensusService.Probes
{
    public abstract class ListeningProbe : Probe
    {
        private int _maxDataStoresPerSecond;

        private readonly object _locker = new object();

        [EntryIntegerUiProperty("Max Data / Second:", true, int.MaxValue)]
        public int MaxDataStoresPerSecond
        {
            get { return _maxDataStoresPerSecond; }
            set { _maxDataStoresPerSecond = value; }
        }

        protected ListeningProbe()
        {
            _maxDataStoresPerSecond = 1;
        }

        public sealed override void Start()
        {
            lock (_locker)
            {
                base.Start();

                StartListening();
            }
        }

        protected abstract void StartListening();

        public sealed override void Stop()
        {
            lock (_locker)
            {
                base.Stop();

                StopListening();
            }
        }

        protected abstract void StopListening();

        protected sealed override void StoreDatum(Datum datum)
        {
            float storesPerSecond = 1 / (float)(DateTimeOffset.UtcNow - MostRecentStoreTimestamp).TotalSeconds;
            if (storesPerSecond <= _maxDataStoresPerSecond)
                base.StoreDatum(datum);
        }
    }
}
