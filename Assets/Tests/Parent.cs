using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPQ
{
    public class Parent
    {
        public void Write()
        {
            Debug.Log(this.text());
        }
        public virtual string text() => "Parent text";
    }
}
