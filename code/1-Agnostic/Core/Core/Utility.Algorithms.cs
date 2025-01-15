namespace SA.Agnostic {
    using AssociationTargetSet = System.Collections.Generic.HashSet<object>;
    using AssociationDictionary = System.Collections.Generic.Dictionary<object, object>;
    using AssociationTargetDictionary = System.Collections.Generic.Dictionary<object, System.Collections.Generic.HashSet<object>>;

    public static partial class Utility {

        public static class AlgorithmSet {
        } //class AlgorithmSet

        public class AssociatonDictionary {
            public void Add(object reference, object target) {
                if (!associationDictionary.ContainsKey(reference))
                    associationDictionary.Add(reference, target);
                if (!associationTargetDictionary.TryGetValue(target, out AssociationTargetSet targetSet)) {
                    AssociationTargetSet newTargetSet = new();
                    newTargetSet.Add(reference);
                    associationTargetDictionary.Add(target, newTargetSet);
                } else
                    targetSet.Add(reference);
            } //Add
            public void RemoveReference(object reference) {
                if (!associationDictionary.TryGetValue(reference, out object target)) return;
                if (!associationTargetDictionary.TryGetValue(target, out AssociationTargetSet targetSet)) return;
                targetSet.Remove(reference);
                if (targetSet.Count < 1)
                    associationTargetDictionary.Remove(target);
                associationDictionary.Remove(reference);
            } //RemoveReference
            public void RemoveTarget(object target) {
                if (!associationTargetDictionary.TryGetValue(target, out AssociationTargetSet targetSet)) return;
                foreach (var reference in targetSet)
                    RemoveReference(reference);
            } //RemoveTarget
            public void RemoveAny(object referenceOrTarget) {
                RemoveReference(referenceOrTarget);
                RemoveTarget(referenceOrTarget);
            } //RemoveAny
            public object FindTarget(object reference) {
                if (associationDictionary.TryGetValue(reference, out object target))
                    return target;
                else
                    return null;
            } //FindTarget
            public object[] FindReferences(object target) {
                if (associationTargetDictionary.TryGetValue(target, out AssociationTargetSet targetSet)) {
                    object[] result = new object[targetSet.Count];
                    if (targetSet.Count > 0)
                        targetSet.CopyTo(result);
                    return result;
                } else
                    return System.Array.Empty<object>();
            } //FindReferences
            public void ForEachReference(System.Action<object, object> action) {
                foreach (var pair in associationDictionary)
                    action(pair.Key, pair.Value);
            } //ForEachReference
            public void Clear() {
                associationDictionary.Clear();
                associationTargetDictionary.Clear();
            } //Clear
            readonly AssociationDictionary associationDictionary = new();
            readonly AssociationTargetDictionary associationTargetDictionary = new();
        } //class AssociatonDictionary

    } //class Utility

}
