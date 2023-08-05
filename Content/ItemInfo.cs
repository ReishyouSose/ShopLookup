using System.Drawing;
using Terraria.Localization;

namespace ShopLookup.Content
{
    public class ItemInfo : BaseUIElement
    {
        public List<string> infos;
        public ItemInfo(string info, IEnumerable<Condition> cds, float width)
        {
            infos = new()
            {
                info
            };
            int i = 1;
            foreach (Condition c in cds)
            {
                string cdesc = c.Description.Value;
                int count = cds.Count();
                if (i == 1 && count > 1)
                {
                    //height = font.MeasureString(c.Description.Value).Y;
                }
            }
        }
        private static string Decription(string info, IEnumerable<Condition> cds, float width, out float height)
        {
            string conditions = "";
            var font = FontAssets.MouseText.Value;
            height = 0;
            int count = cds.Count();
            if (count > 0)
            {
                int i = 1;
                foreach (Condition c in cds)
                {
                    string cdesc = c.Description.Value;
                    conditions += cdesc;
                    /*if (c.Description.Key == cdesc)
                    {

                    }*/
                    if (i == 1 && count > 1)
                    {
                        height = font.MeasureString(c.Description.Value).Y;
                    }
                    if (i < count) conditions += "\n";
                    i++;
                }
                float oldH = font.MeasureString(conditions).Y;
                conditions = font.CreateWrappedText(conditions, width);
                float newH = font.MeasureString(conditions).Y;
                if (height > 0)
                {
                    height = newH - height;
                }
                else
                {
                    if (newH > oldH) height = newH - oldH;
                }

                return info + "\n" + conditions;
            }
            //else return info + "\n" + Language.GetTextValue(LocalKey + "NoCondition");
            return info + "\n" + conditions;
        }
    }
}
