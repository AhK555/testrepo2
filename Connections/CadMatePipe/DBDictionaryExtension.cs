using Gssoft.Gscad.DatabaseServices;
using Gssoft.Gscad.Runtime;

namespace NSVLibUtils
{
    /// <summary>
    /// Provides extension methods for working with DBDictionary and Xrecord.
    /// </summary>
    public static class DBDictionaryExtension
    {
        /// <summary>
        /// Gets the named object dictionary.
        /// </summary>
        /// <param name="db">Instance to which the method applies.</param>
        /// <param name="tr">Active transaction.</param>
        /// <param name="mode">Open mode to obtain in.</param>
        /// <returns>The named object dictionary.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="db"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="tr"/> is null.</exception>
        public static DBDictionary GetNOD(
            this Database db,
            Transaction tr,
            OpenMode mode = OpenMode.ForRead)
        {
            Assert.IsNotNull(db, nameof(db));
            Assert.IsNotNull(tr, nameof(tr));

            return (DBDictionary)tr.GetObject(db.NamedObjectsDictionaryId, mode);
        }

        /// <summary>
        /// Tries to get the named dictionary.
        /// </summary>
        /// <param name="parent">Instance to which the method applies.</param>
        /// <param name="key">Name of the dictionary.</param>
        /// <param name="tr">Active transaction.</param>
        /// <param name="dictionary">Output dictionary.</param>
        /// <param name="mode">Open mode to obtain in.</param>
        /// <param name="openErased">Value indicating whether to obtain erased objects.</param>
        /// <returns><c>true</c>, if the operations succeeded; <c>false</c>, otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="parent"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name ="key"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="tr"/> is null.</exception>
        public static bool TryGetNamedDictionary(
            this DBDictionary parent,
            string key,
            Transaction tr,
            out DBDictionary dictionary,
            OpenMode mode = OpenMode.ForRead,
            bool openErased = false)
        {
            Assert.IsNotNull(parent, nameof(parent));
            Assert.IsNotNullOrWhiteSpace(key, nameof(key));
            Assert.IsNotNull(tr, nameof(tr));

            dictionary = default;
            if (parent.Contains(key))
            {
                var id = parent.GetAt(key);
                if (id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(DBDictionary))))
                {
                    dictionary = (DBDictionary)tr.GetObject(id, mode, openErased);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to get the object extension dictionary.
        /// </summary>
        /// <param name="dbObject">Instance to which the method applies.</param>
        /// <param name="tr">Active transaction.</param>
        /// <param name="dictionary">Output dictionary.</param>
        /// <param name="mode">Open mode to obtain in.</param>
        /// <param name="openErased">Value indicating whether to obtain erased objects.</param>
        /// <returns><c>true</c>, if the operation succeeded; <c>false</c>, otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="dbObject"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="tr"/> is null.</exception>
        public static bool TryGetExtensionDictionary(
            this DBObject dbObject,
            Transaction tr,
            out DBDictionary dictionary,
            OpenMode mode = OpenMode.ForRead,
            bool openErased = false)
        {
            Assert.IsNotNull(dbObject, nameof(dbObject));
            Assert.IsNotNull(tr, nameof(tr));

            dictionary = default;
            var id = dbObject.ExtensionDictionary;
            if (id.IsNull)
                return false;
            dictionary = (DBDictionary)tr.GetObject(id, mode, openErased);
            return true;
        }

        /// <summary>
        /// Gets or creates the named dictionary.
        /// </summary>
        /// <param name="parent">Instance to which the method applies.</param>
        /// <param name="name">Name of the dictionary.</param>
        /// <param name="tr">Active transaction.</param>
        /// <returns>The found or newly created dictionary.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="parent"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name ="name"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="tr"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Throw if the <paramref name="name"/> is not a DBDictionary.</exception>
        public static DBDictionary GetOrCreateNamedDictionary(
            this DBDictionary parent,
            string name,
            Transaction tr)
        {
            Assert.IsNotNull(parent, nameof(parent));
            Assert.IsNotNullOrWhiteSpace(name, nameof(name));
            Assert.IsNotNull(tr, nameof(tr));

            if (parent.Contains(name))
            {
                var id = parent.GetAt(name);
                if (!id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(DBDictionary))))
                    throw new System.ArgumentException("Not a DBDictionary", nameof(name));
                return (DBDictionary)tr.GetObject(id, OpenMode.ForRead);
            }
            else
            {
                var dictionary = new DBDictionary();
                parent.OpenForWrite(tr);
                parent.SetAt(name, dictionary);
                tr.AddNewlyCreatedDBObject(dictionary, true);
                return dictionary;
            }
        }

        /// <summary>
        /// Gets or creates the extension dictionary.
        /// </summary>
        /// <param name="dbObject">Instance to which the method applies.</param>
        /// <param name="tr">Active transaction.</param>
        /// <param name="mode">Open mode to obtain in.</param>
        /// <returns>The extension dictionary.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="dbObject"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="tr"/> is null.</exception>
        public static DBDictionary GetOrCreateExtensionDictionary(
            this DBObject dbObject,
            Transaction tr,
            OpenMode mode = OpenMode.ForRead)
        {
            Assert.IsNotNull(dbObject, nameof(dbObject));
            Assert.IsNotNull(tr, nameof(tr));

            if (dbObject.ExtensionDictionary.IsNull)
            {
                dbObject.OpenForWrite(tr);
                dbObject.CreateExtensionDictionary();
            }
            return (DBDictionary)tr.GetObject(dbObject.ExtensionDictionary, mode);
        }

        /// <summary>
        /// Tries to get the xrecord data.
        /// </summary>
        /// <param name="dictionary">Instance to which the method applies.</param>
        /// <param name="key">Key of the xrecord.</param>
        /// <param name="tr">Active transaction</param>
        /// <param name="data">Output data.</param>
        /// <returns><c>true</c>, if the operation succeeded; <c>false</c>, otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="dictionary"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name ="key"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="tr"/> is null.</exception>
        public static bool TryGetXrecordData(
            this DBDictionary dictionary,
            string key,
            Transaction tr,
            out ResultBuffer data)
        {
            Assert.IsNotNull(dictionary, nameof(dictionary));
            Assert.IsNotNullOrWhiteSpace(key, nameof(key));
            Assert.IsNotNull(tr, nameof(tr));
            data = default;
            if (dictionary.Contains(key))
            {
                var id = dictionary.GetAt(key);
                if (id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(Xrecord))))
                {
                    var xrecord = (Xrecord)tr.GetObject(id, OpenMode.ForRead);
                    data = xrecord.Data;
                    return data != null;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets the xrecord data.
        /// </summary>
        /// <param name="dictionary">Instance to which the method applies.</param>
        /// <param name="key">Key of the xrecord, the xrecord is created if it does not already exist.</param>
        /// <param name="tr">Active transaction.</param>
        /// <param name="data">Data</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="dictionary"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name ="key"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="data"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="tr"/> is null.</exception>
        public static void SetXrecordData(this DBDictionary dictionary, string key, Transaction tr, ResultBuffer data)
        {
            Assert.IsNotNull(dictionary, nameof(dictionary));
            Assert.IsNotNullOrWhiteSpace(key, nameof(key));
            Assert.IsNotNull(data, nameof(data));
            Assert.IsNotNull(tr, nameof(tr));
            Xrecord xrecord;
            if (dictionary.Contains(key))
            {
                var id = dictionary.GetAt(key);
                if (!id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(Xrecord))))
                    throw new System.ArgumentException("Not an Xrecord'", nameof(key));
                {
                    xrecord = (Xrecord)tr.GetObject(id, OpenMode.ForWrite);
                }
            }
            else
            {
                xrecord = new Xrecord();
                dictionary.OpenForWrite(tr);
                dictionary.SetAt(key, xrecord);
                tr.AddNewlyCreatedDBObject(xrecord, true);
            }
            xrecord.Data = data;
        }

        /// <summary>
        /// Sets the xrecord data.
        /// </summary>
        /// <param name="dictionary">Instance to which the method applies.</param>
        /// <param name="key">Key of the xrecord, the xrecord is created if it does not already exist.</param>
        /// <param name="tr">Active transaction.</param>
        /// <param name="data">Data</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="dictionary"/> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name ="key"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="data"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="tr"/> is null.</exception>
        public static void SetXrecordData(this DBDictionary dictionary, string key, Transaction tr, params TypedValue[] data) =>
            dictionary.SetXrecordData(key, tr, new ResultBuffer(data));

        /// <summary>
        /// Opens the object for write.
        /// </summary>
        /// <param name="dbObject">Instance to which the method applies.</param>
        /// <param name="tr">Active transaction.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="dbObject"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="tr"/> is null.</exception>
        public static void OpenForWrite(this DBObject dbObject, Transaction tr)
        {
            Assert.IsNotNull(dbObject, nameof(dbObject));
            Assert.IsNotNull(tr, nameof(tr));
            if (!dbObject.IsWriteEnabled)
                tr.GetObject(dbObject.ObjectId, OpenMode.ForWrite);
        }

        static class Assert
        {
            public static void IsNotNull<T>(T obj, string paramName) where T : class
            {
                if (obj == null)
                    throw new System.ArgumentNullException(paramName);
            }

            public static void IsNotNullOrWhiteSpace(string str, string paramName)
            {
                if (string.IsNullOrWhiteSpace(str))
                    throw new System.ArgumentException("eNullOrWhiteSpace", paramName);
            }
        }
    }
}