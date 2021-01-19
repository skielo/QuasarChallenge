using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoder.Logic
{
    public class ShipDecoder
    {
        private readonly Satellite[] satellites;
        public Satellite[] Satellites { get { return satellites; }  }

        public ShipDecoder(Satellite[] satellites)
        {
            this.satellites = satellites;
        }

        public (double x, double y) GetLocation(double[] distances)
        {
            return TrackPosition(satellites[0].Possition.x, satellites[0].Possition.y, distances[0],
                                 satellites[1].Possition.x, satellites[1].Possition.y, distances[1],
                                 satellites[2].Possition.x, satellites[2].Possition.y, distances[2]);
        }

        /// <summary>
        /// Calculate the point (x, y) based on the (x,y,d) of each satellite using the trilateration.
        /// </summary>
        /// <param name="x1">X possition of the first satellite</param>
        /// <param name="y1">Y possition of the first satellite</param>
        /// <param name="d1">Distance from the object to the first satellite</param>
        /// <param name="x2">X possition of the first satellite</param>
        /// <param name="y2">Y possition of the first satellite</param>
        /// <param name="d2">Distance from the object to the first satellite</param>
        /// <param name="x3">X possition of the first satellite</param>
        /// <param name="y3">Y possition of the first satellite</param>
        /// <param name="d3">Distance from the object to the first satellite</param>
        /// <returns>The relative possition of the object.</returns>
        private (double, double) TrackPosition(double x1, double y1, double d1, double x2, double y2, double d2, double x3, double y3, double d3)
        {
            double A, B, C, D, E, F, x, y;

            A = 2 * x2 - 2 * x1;
            B = 2 * y2 - 2 * y1;
            C = (Math.Pow(d1, 2) - Math.Pow(d2, 2) - Math.Pow(x1, 2) + Math.Pow(x2, 2) - Math.Pow(y1, 2) - Math.Pow(y2, 2));
            D = 2 * x3 - 2 * x2;
            E = 2 * y3 - 2 * y2;
            F = (Math.Pow(d2, 2) - Math.Pow(d3, 2) - Math.Pow(x2, 2) + Math.Pow(x3, 2) - Math.Pow(y2, 2) - Math.Pow(y3, 2));

            x = Math.Round((C * E - F * B) / (E * A - B * D),1);
            y = Math.Round((C * D - A * F) / (B * D - A * E),1);

            return (x, y);
        }

        public string GetMessage(string[][] messages)
        {
            var retval = string.Empty;
            var noOfSatellites = satellites.Length;
            var maxNoOfStrings = messages.Max(x => x.Length);
            var minNoOfStrings = messages.Min(x => x.Length);
            var existDifference = (maxNoOfStrings - minNoOfStrings > 0);
            var queue = new Queue<string>();

            //Normalize arrays
            //review first array
            messages[0] = NormalizeArray(messages[0], maxNoOfStrings, existDifference);
            messages[1] = NormalizeArray(messages[1], maxNoOfStrings, existDifference);
            messages[2] = NormalizeArray(messages[2], maxNoOfStrings, existDifference);

            for (int i = 0; i < minNoOfStrings; i++)
            {
                var tmp = MessageToAdd(messages, 0, i, Possition.START);
                if (!string.IsNullOrEmpty(tmp))
                {
                    queue.Enqueue(tmp);
                    continue;
                }
                tmp = MessageToAdd(messages, 1, i, Possition.MIDDLE);
                if (!string.IsNullOrEmpty(tmp))
                {
                    queue.Enqueue(tmp);
                    continue;
                }
                tmp = MessageToAdd(messages, 2, i, Possition.END);
                if (!string.IsNullOrEmpty(tmp))
                {
                    queue.Enqueue(tmp);
                    continue;
                }
            }

            if(queue.Count > 0)
            {
                retval = string.Join(' ', queue);
            }
            return retval;
        }

        /// <summary>
        /// This method validates there is a difference between the arrays and try to normalize them by removing the first element in case 
        /// is an empty string or the last one.
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="maxNoOfStrings"></param>
        /// <param name="existDifference"></param>
        /// <returns></returns>
        private string[] NormalizeArray(string[] messages, int maxNoOfStrings, bool existDifference)
        {
            if (existDifference && messages.Length == maxNoOfStrings && messages.First() == string.Empty)
            {
                return messages.Skip(1).ToArray();
            }
            if (existDifference && messages.Length == maxNoOfStrings && messages.Last() == string.Empty)
            {
                return messages.SkipLast(1).ToArray();
            }
            return messages;
        }

        private bool IsStringEmpty(string message)
        {
            return (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message));
        }
        
        /// <summary>
        /// This method validate the current string in the possitions of strings and validate them agains the others.
        /// If all the others are empty or equal to the one I'm validation then I added as a result.
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="start"></param>
        /// <param name="index"></param>
        /// <param name="possition"></param>
        /// <returns></returns>
        private string MessageToAdd(string[][] messages, int start, int index, Possition possition)
        {
            var retval = string.Empty;
            if (!IsStringEmpty(messages[start][index]))
            {
                switch (possition)
                {
                    case Possition.START:
                        if ((messages[start][index] == messages[start + 1][index]) ||
                            (messages[start][index] == messages[start + 2][index]) ||
                            (IsStringEmpty(messages[start + 1][index]) && IsStringEmpty(messages[start + 2][index])))
                            retval = messages[start][index];
                        break;
                    case Possition.MIDDLE:
                        if ((messages[start][index] == messages[start - 1][index]) ||
                            (messages[start][index] == messages[start + 1][index]) ||
                            (IsStringEmpty(messages[start - 1][index]) && IsStringEmpty(messages[start + 1][index])))
                            retval = messages[start][index];
                        break;
                    case Possition.END:
                        if ((messages[start][index] == messages[start - 1][index]) ||
                            (messages[start][index] == messages[start - 2][index]) ||
                            (IsStringEmpty(messages[start - 1][index]) && IsStringEmpty(messages[start - 2][index])))
                            retval = messages[start][index];
                        break;
                }

            }
            return retval;
        }
     }
}
