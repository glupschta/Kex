﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kex.Interfaces;

namespace Kex
{
    public class ItemSelection : IEnumerable<IItem>
    {
        public FileActionType FileAction { get; set; }
        public IEnumerable<IItem> Selection { get; set; }

        public int ItemCount
        {
            get
            {
                return Selection.Count();
            }
        }

        public string ItemSize
        {
            get
            {
                return Formatter.FormatLength(Selection.Sum(it => it.Length));
            }
        }

        public IEnumerator<IItem> GetEnumerator()
        {
            return Selection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
