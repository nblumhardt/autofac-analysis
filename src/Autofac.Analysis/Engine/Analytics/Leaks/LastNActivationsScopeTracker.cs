using System;
using System.Collections.Specialized;

namespace Autofac.Analysis.Engine.Analytics.Leaks
{
    class LastNActivationsScopeTracker
    {
        readonly int _maxTrackedScopes;
        readonly OrderedDictionary /* string, int */ _activationCounts = new OrderedDictionary();

        public LastNActivationsScopeTracker(int maxTrackedScopes)
        {
            _maxTrackedScopes = maxTrackedScopes;
        }

        public int RecordActivation(string lifetimeScopeId)
        {
            if (lifetimeScopeId == null) throw new ArgumentNullException(nameof(lifetimeScopeId));

            if (_activationCounts.Contains(lifetimeScopeId))
            {
                var newCount = 1 + (int)_activationCounts[lifetimeScopeId];
                _activationCounts[lifetimeScopeId] = newCount;
                return newCount;
            }

            _activationCounts.Insert(0, lifetimeScopeId, 1);
            if (_activationCounts.Count > _maxTrackedScopes)
                _activationCounts.RemoveAt(_maxTrackedScopes);

            return 1;
        }

        public bool HasWarningBeenIssued { get; set; }
    }
}
