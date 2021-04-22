using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class CollectionExtensions {
        /* from:https://stackoverflow.com/questions/56692/random-weighted-choice

    Dictionary<string, float> foo = new Dictionary<string, float>();
        foo.Add("Item 25% 1", 0.5f);
        foo.Add("Item 25% 2", 0.5f);
        foo.Add("Item 50%", 1f);

        for(int i = 0; i < 10; i++)
            Console.WriteLine(this, "Item Chosen {0}", foo.RandomElementByWeight(e => e.Value));
    */

        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector) {
            T[] result = RandomElementsByWeight(sequence, weightSelector, 1);
            if (result == null) {
                return default;
            }
            return result[0];
        }

        public static T[] RandomElementsByWeight<T>(this IEnumerable<T> sequence, Func<T,float> weightSelector, int count) {
            T[] result = new T[count];
            float totalWeight = sequence.Sum(weightSelector);

            for (int i = 0; i<count; i++) {
                float itemWeightIndex = UnityEngine.Random.Range(0f, 1f) * totalWeight;
                float currentWeightIndex = 0;

                foreach (T item in sequence) {
                    currentWeightIndex += weightSelector(item);

                    if (currentWeightIndex >= itemWeightIndex) {
                        result[i] = item;
                        break;
                    }
                }
            }


            return result;
        }

        public static T RandomElement<T> (this List<T> list) {
            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }

        public static List<Vector3> Shuffle(this List<Vector3> list, int shuffleMultiplier = 1) {

            for (int count = 0; count<shuffleMultiplier; count++) {
                for (int i = 0; i<list.Count-1; i++) {
                
                    list.Swap(i, Random.Range(i+1, list.Count));
                }
            }
            return list;
        }

        public static List<Vector3> Swap(this List<Vector3> list, int a, int b) {
            Vector3 temp = list[a];
            list[a] = list[b];
            list[b] = temp;

            return list;
        }

    }
}