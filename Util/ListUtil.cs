using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Maku
{
    public static class ListUtil
    {
        public static List<int> Mode(this List<int> numbers)
        {
            var groupedNumbers = numbers.GroupBy(n => n)
                                        .Select(g => new { Number = g.Key, Count = g.Count() })
                                        .OrderByDescending(g => g.Count);

            int maxCount = groupedNumbers.First().Count;

            List<int> modes = groupedNumbers.Where(g => g.Count == maxCount)
                                      .Select(g => g.Number).ToList();

            Debug.Log("ç≈ïpíl: " + string.Join(", ", modes));

            return modes;
        }
    }
}