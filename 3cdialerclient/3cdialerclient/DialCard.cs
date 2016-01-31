using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _cdialerclient
{
    class DialCard
    {
        internal static List<DialCardItem> Create(Call call)
        {
            List<DialCardItem> items = new List<DialCardItem>();

            items.Add(new DialCardItem() { label = "Name:", val = call.name == "" ? "None" : call.name });
            items.Add(new DialCardItem() { label = "Telephone 1:", val = call.tel1 == "" ? "None" : call.tel1 });
            items.Add(new DialCardItem() { label = "Telephone 2:", val = call.tel2 == "" ? "None" : call.tel2 });
            items.Add(new DialCardItem() { label = "Language:", val = call.lang == "" ? "None" : call.lang});
            items.Add(new DialCardItem() { label = "Country:", val = call.country == "" ? "None" : call.country });
            items.Add(new DialCardItem() { label = "Custom1:", val = call.custom1 == "" ? "None" : call.custom1 });
            items.Add(new DialCardItem() { label = "Custom2:", val = call.custom2 == "" ? "None" : call.custom2 });
            items.Add(new DialCardItem() { label = "Custom3:", val = call.custom3 == "" ? "None" : call.custom3 });
            items.Add(new DialCardItem() { label = "Custom4:", val = call.custom4 == "" ? "None" : call.custom4 });
            items.Add(new DialCardItem() { label = "Custom5:", val = call.custom5 == "" ? "None" : call.custom5 });
            items.Add(new DialCardItem() { label = "Custom6:", val = call.custom6 == "" ? "None" : call.custom6 });
            items.Add(new DialCardItem() { label = "Custom7:", val = call.custom7 == "" ? "None" : call.custom7 });

            return items;
        }
        internal static List<DialCardItem> CreateBlank()
        {
            Call call = new Call() { 
                name = "No calls loaded from server.",
                tel1 = "upload new list.",
                tel2 = "",
                lang = "",
                custom1 = "",
                country = "",
                custom2 = "",
                custom3 = "",
                custom4 = "",
                custom5 = "",
                custom6 = "",
                custom7 = ""
            };
            List<DialCardItem> items = new List<DialCardItem>();

            items.Add(new DialCardItem() { label = "Name:", val = call.name == "" ? "None" : call.name });
            items.Add(new DialCardItem() { label = "Telephone 1:", val = call.tel1 == "" ? "None" : call.tel1 });
            items.Add(new DialCardItem() { label = "Telephone 2:", val = call.tel2 == "" ? "None" : call.tel2 });
            items.Add(new DialCardItem() { label = "Language:", val = call.lang == "" ? "None" : call.lang });
            items.Add(new DialCardItem() { label = "Country:", val = call.country == "" ? "None" : call.country });
            items.Add(new DialCardItem() { label = "Custom1:", val = call.custom1 == "" ? "None" : call.custom1 });
            items.Add(new DialCardItem() { label = "Custom2:", val = call.custom2 == "" ? "None" : call.custom2 });
            items.Add(new DialCardItem() { label = "Custom3:", val = call.custom3 == "" ? "None" : call.custom3 });
            items.Add(new DialCardItem() { label = "Custom4:", val = call.custom4 == "" ? "None" : call.custom4 });
            items.Add(new DialCardItem() { label = "Custom5:", val = call.custom5 == "" ? "None" : call.custom5 });
            items.Add(new DialCardItem() { label = "Custom6:", val = call.custom6 == "" ? "None" : call.custom6 });
            items.Add(new DialCardItem() { label = "Custom7:", val = call.custom7 == "" ? "None" : call.custom7 });

            return items;
        }
    }
    class DialCardItem
    {
        public string label {get; set;}
        public string val {get; set;}
    }
}
