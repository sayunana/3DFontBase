using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ThreeDFont
{
    [System.Serializable]
    public class CharacterData
    {
        public char Character;

        [SerializeField]
        private List<FontPosition> p = new List<FontPosition>();
        public List<FontPosition> FontPositions { get { return p; } set { p = value; } }
        
        [SerializeField]
        private List<FontPosition> np = new List<FontPosition>();
        public List<FontPosition> NormalizedFontPositions { get { return np;} set { np = value;} }    

        [SerializeField]
        private List<Stroke> s = new List<Stroke>();
        public List<Stroke> Strokes { get { return s; } set { s = value; } }

        public CharacterData(char c) 
        {
            Character = c;
        }

        public CharacterData()
        {
        }

        public CharacterData(CharacterData characterData)
        {
            Character = characterData.Character;
            FontPositions = characterData.FontPositions;
        }

        public void Normalize()
        {
            // 初期化
            NormalizedFontPositions.Clear();
            
            float minX = FontPositions.Where(x => x.IsWrite).Min(x => x.WritePosition.x);
            float minY = FontPositions.Where(x => x.IsWrite).Min(x => x.WritePosition.y);
            float maxX = FontPositions.Where(x => x.IsWrite).Max(x => x.WritePosition.x);
            float maxY = FontPositions.Where(x => x.IsWrite).Max(x => x.WritePosition.y);
            float minZ = FontPositions.Where(x => x.IsWrite).Min(x => x.WritePosition.z);
            float maxZ = FontPositions.Where(x => x.IsWrite).Max(x => x.WritePosition.z);

            foreach (var fontPosition in FontPositions)
            {
                NormalizedFontPositions.Add(new FontPosition()
                {
                    WritePosition = new UnityEngine.Vector3(
                        Round(fontPosition.WritePosition.x, minX,maxX),
                        Round(fontPosition.WritePosition.y, minY,maxY),
                        Round(fontPosition.WritePosition.z, minZ,maxZ)),
                    IsWrite = fontPosition.IsWrite
                });
            }
        }

        private float Round(float value,float min,float max)
        {
            var f = (value - min) / (max - min);
            if ( float.IsInfinity( f ) || float.IsNaN( f ))
            {
                return 0;
            }
            float multiplier = Mathf.Pow(10.0f, 5);
            return Mathf.Round(f * multiplier) / multiplier;
        }
        public void LastWriteDataRemove()
        {
            var baseFontPosition = new List<FontPosition>(FontPositions);
            do
            {
                var firstFontPosition = baseFontPosition.FirstOrDefault();
                if (firstFontPosition is null)
                {
                    break;
                }
                var res = baseFontPosition.TakeWhile(x => x.IsWrite == firstFontPosition.IsWrite).ToList();
                Strokes.Add(new Stroke(res));
                baseFontPosition.RemoveRange(0,res.Count);
            } while (true);

            Debug.Log(Strokes.Last().Positions.Count +
                      Strokes.ElementAt(Strokes.Count - 2 - 1).Positions.Count);
            FontPositions.Reverse();
            FontPositions.RemoveRange(0,
                Strokes.Last().Positions.Count +
                Strokes.ElementAt(Strokes.Count - 2 - 1).Positions.Count);
            FontPositions.Reverse();
        }
    }

    [System.Serializable]
    public class FontPosition
    {
        [SerializeField]
        private Vector3 p; // WritePosition の短縮名
        [SerializeField]
        private bool w; // IsWrite の短縮名
        public Vector3 WritePosition { get { return p; } set { p = value; } }
        public bool IsWrite { get { return w; } set { w = value; } }
    }
    [System.Serializable]
    public class Stroke
    {
        [SerializeField]
        private List<FontPosition> d = new List<FontPosition>();
        public List<FontPosition> Positions { get { return d; } set { d = value; } }
        public Stroke(List<FontPosition> positions)
        {
            Positions = positions;
        }
    }
}