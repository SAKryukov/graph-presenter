namespace SA.Agnostic {
    using System;
    using System.Collections.Generic;

    public static partial class Utility {

        public static class Collection {

            public static bool IsList(Type type) =>
                type != null && type.IsAssignableTo(typeof(System.Collections.IList));

            public static void AddToGenericCollection(object collection, object child) {
                var addMethod = collection.GetType().GetMethod(AddName);
                addMethod.Invoke(collection, new object[] { child });
            } //AddToGenericCollection

            public static void InsertIntoGenericCollection(object collection, int index, object child) {
                var insertMethod = collection.GetType().GetMethod(InsertName);
                insertMethod?.Invoke(collection, new object[] { index, child });
                if (insertMethod != null) return;
                var addMethod = collection.GetType().GetMethod(AddName);
                addMethod.Invoke(collection, new object[] { child });
            } //InsertIntoGenericCollection

            public static void RemoveFromGenericCollection(object collection, object child) {
                var removeMethod = collection.GetType().GetMethod(RemoveName);
                removeMethod.Invoke(collection, new object[] { child });
            } //RemoveFromGenericCollection

            #region implementation

            static string AddName {
                get {
                    if (addName == null)
                        addName = nameof(List<byte>.Add);
                    return addName;
                }
            } //AddName
            static string RemoveName {
                get {
                    if (removeName == null)
                        removeName = nameof(List<byte>.Remove);
                    return removeName;
                }
            } //RemoveName
            static string InsertName {
                get {
                    if (insertName == null)
                        insertName = nameof(List<byte>.Insert);
                    return insertName;
                }
            } //InsertName
            static string addName, removeName, insertName;

            #endregion implementation

        } //class Collection

    } //class Utility

}
