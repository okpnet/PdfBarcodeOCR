using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Helper
{
    public static class FileNameHelper
    {
        /// <summary>
        /// 新しい名前を生成する。リストから新しい名前と一致する名前に連番を付けて返す
        /// </summary>
        /// <param name="list">検索する文字列リスト</param>
        /// <param name="nm">新しい名前</param>
        /// <param name="gc">新しい名前と番号の間の文字列。デフォルトは"_"</param>
        /// <returns>番号つきの新しい名前</returns>
        public static string CreateNumberAppendToNewname(IEnumerable<string> list, string newName, string grueStr = "_")
        {
            var nm = System.Text.RegularExpressions.Regex.Escape(newName);
            var gc = System.Text.RegularExpressions.Regex.Escape(grueStr);

            var sufixVal = 0;
            var sufixList = list.
                    Where(t => t is not (null or "") &&
                        System.Text.RegularExpressions.Regex.IsMatch(t, $"^{nm}{gc}(\\d)+$|^{nm}$"));
            if (sufixList.Count() > 0)
            {
                sufixVal = sufixList.Select(t =>
                {//サフィックスの生成
                 //if (!System.Text.RegularExpressions.Regex.IsMatch(t.InfoDepartments.Name, $"^{newName}_(\\d)+$|^{newName}$")) return 0;
                    var numbers = System.Text.RegularExpressions.Regex.Matches(t, $"{gc}(\\d)+$");

                    if (numbers.Count() > 0 && int.TryParse(numbers.Last().Value[1..], out var sufix))
                    {
                        return sufix;
                    }
                    return 0;
                }).Max() + 1;
            }
            var result = newName + (sufixVal > 0 ? $"{grueStr}{sufixVal}" : "");
            return result;
        }
        /// <summary>
        /// 新しい名前を生成する。リストから新しい名前と一致する名前に連番を付けて返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">配列</param>
        /// <param name="getName">インスタンスから文字列を取り出す</param>
        /// <param name="setName">インスタンスへ文字列をセットする</param>
        /// <param name="grueStr">文字列と連番の接続文字列</param>
        public static void CreateNumberAppendToNewnames<T>(this IEnumerable<T> values, Func<T, string> getName, Action<T, string> setName, string grueStr = "_")
        {
            var groups = values.GroupBy(getName).Where(t => t.Count() > 1);

            foreach (var group in groups)
            {
                var continued = false;

                foreach (var item in group)
                {
                    if (!continued | ReferenceEquals(item, group.FirstOrDefault()))
                    {
                        continued = true;
                        continue;
                    }
                    var newName = CreateNumberAppendToNewname(group.Select(getName), getName(item), grueStr);
                    setName(item, newName);
                }
            }
        }

        public static IEnumerable<string> CreateNumberAppendToNewnames(this IEnumerable<string> values, string grueStr = "_")
        {
            var groups = values.GroupBy(t => t);
            foreach (var group in groups)
            {
                var continued = false;
                foreach (var item in group)
                {
                    if (!continued)
                    {
                        continued = true;
                        yield return item;
                        continue;
                    }
                    var newName = CreateNumberAppendToNewname(group, item, grueStr);
                    yield return newName;
                }
            }
        }
    }
}
