using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagement.Helper
{
    public static class FileNameHelper
    {
        /// <summary>
        /// 新しい名前を生成する。リストから新しい名前と一致する名前に連番を付けて返す
        /// </summary>
        /// <param name="list">検索する文字列リスト</param>
        /// <param name="newName">新しい名前</param>
        /// <param name="grueChar">新しい名前と番号の間の文字列。デフォルトは"_"</param>
        /// <returns>番号つきの新しい名前</returns>
        public static string CreateNumberAppendToNewname(IEnumerable<string> list, string newName, string grueChar = "_")
        {
            var sufixVal = 0;
            var sufixList = list.
                    Where(t => t is not (null or "") &&
                        System.Text.RegularExpressions.Regex.IsMatch(t, $"^{newName}_(\\d)+$|^{newName}$"));
            if (sufixList.Count() > 0)
            {
                sufixVal = sufixList.Select(t =>
                {//サフィックスの生成
                 //if (!System.Text.RegularExpressions.Regex.IsMatch(t.InfoDepartments.Name, $"^{newName}_(\\d)+$|^{newName}$")) return 0;
                    var numbers = System.Text.RegularExpressions.Regex.Matches(t, $"_(\\d)+$");

                    if (numbers.Count() > 0 && int.TryParse(numbers.Last().Value[1..], out var sufix))
                    {
                        return sufix;
                    }
                    return 0;
                }).
                    Max() + 1;
            }
            var result = newName + (sufixVal > 0 ? $"{grueChar}{sufixVal}" : "");
            return result;
        }
    }
}
