﻿using System;
using System.Collections.Generic;
using System.Text;
using Axis.Jupiter.Contracts;
using Axis.Luna.Extensions;

namespace Axis.Jupiter
{
    public class StoreProviderMap
    {
        private readonly Dictionary<string, Entry> _entries = new Dictionary<string, Entry>();

        public Entry Default { get; }

        public StoreProviderMap(params Entry[] entries)
        {
            (entries ?? new Entry[0])
                .ForAll(AddEntry);
        }

        public StoreProviderMap(Entry defaultEntry, params Entry[] entries)
        : this(entries)
        {
            Default = defaultEntry ?? throw new ArgumentException("Invalid Default Entry specified: null");
        }

        public Entry StoreEntry(string storeName) => _entries[storeName];


        private void AddEntry(Entry entry)
        {
            if (entry == null)
                throw new ArgumentException("Invalid Entry specified: null");

            if (_entries.ContainsKey(entry.StoreName))
                throw new Exception("Duplicate Store Name detected");

            entry.Validate();

            _entries.Add(entry.StoreName, entry);
        }


        public sealed class Entry
        {
            public string StoreName { get; set; }
            public Type StoreQueryType { get; set; }
            public Type StoreCommandType { get; set; }

            public void Validate()
            {
                #region Store Query

                if(StoreQueryType == null)
                    throw new Exception("Invalid Store Query Type");

                if(!StoreQueryType.Implements(typeof(IStoreQuery)))
                    throw new Exception($"Invalid Store Query Type: does not implement/extend {typeof(IStoreQuery).FullName}");

                #endregion

                #region Store Command

                if (StoreCommandType == null)
                    throw new Exception("Invalid Store Command Type");

                if (!StoreCommandType.Implements(typeof(IStoreCommand)))
                    throw new Exception($"Invalid Store Command Type: does not implement/extend {typeof(IStoreCommand).FullName}");

                #endregion
            }
        }
    }
}
