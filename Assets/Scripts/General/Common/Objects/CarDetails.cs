using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Objects
{
    [Serializable]
    public class CarDetails:MonoBehaviour
    {
        [SerializeField]
        List<GameObject> _bodyParts;

        public List<GameObject> BodyParts => _bodyParts;
    }
}
