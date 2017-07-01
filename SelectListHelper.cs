using System.Collections.Generic;
using System.Web.Mvc;

namespace NT.Web.App.Utilities
{
    public static class FieldDictSelectListHelper
    {
        /// <summary>
        /// 將dictionary轉為SelectList供Html.DrowDonwList使用
        /// </summary>
        /// <param name="source">dictionary的key是option的value, value是option的text</param>
        /// <param name="defaultSelected">預設為selected的value</param>
        /// <returns></returns>
        public static SelectList ToSelectList(this IDictionary<string, string> source, string defaultSelected)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem item = null;

            foreach (var dict in source)
            {
                item = new SelectListItem();
                item.Value = dict.Key.ToString();
                item.Text = dict.Value;
                list.Add(item);
            }

            SelectList selectList = new SelectList(
                items: list,
                dataTextField: "Text",
                dataValueField: "Value",
                selectedValue: defaultSelected
                );

            return selectList;
        }

    }
}
