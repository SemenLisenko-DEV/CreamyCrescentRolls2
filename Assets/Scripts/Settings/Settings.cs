using System;

namespace CreamyCrescentRolls.RoomGame.Settings
{
    [Serializable]
    public class Setting
    {
        public string name;

        public float floatValue;
        public int intValue;
        public string stringValue;
        public bool boolValue;

        public bool isNew = true;
    }
}
