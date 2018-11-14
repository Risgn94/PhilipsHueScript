using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using PhilipsHueScript.Models;

namespace PhilipsHueScript
{
    public class LightningService
    {
        public List<CustomColor> ReadCSVData(string pathToDatafile, int seconds)
        {
            var returnList = new List<CustomColor>();

            using (var parser = new TextFieldParser(pathToDatafile))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                var lineCounter = 1;
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (lineCounter != 1)
                    {
                        int r = Convert.ToInt32(fields[1]);
                        int g = Convert.ToInt32(fields[2]); 
                        int b = Convert.ToInt32(fields[3]);

                        var customColor = new CustomColor
                        {
                            XyColor = ParseRGBToXY(r, g, b),
                            Brightness = Convert.ToInt32(fields[4]),
                            Saturation = Convert.ToInt32(fields[5]),
                        };

                        try
                        {
                            customColor.Seconds = Convert.ToInt32(fields[6]);
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("No \"second\" value was found. Using default value.");
                            customColor.Seconds = Convert.ToInt32(seconds);
                        }

                        returnList.Add(customColor);
                    }
                    lineCounter++;
                }
            }
            return returnList;
        }

        public XYColor ParseRGBToXY(int red, int green, int blue)
        {
            double[] normalizedToOne = new double[3];
            double cRed = red;
            double cGreen = green;
            double cBlue = blue;

            normalizedToOne[0] = (cRed / 255);
            normalizedToOne[1] = (cGreen / 255);
            normalizedToOne[2] = (cBlue / 255);

            double innerRed, innerGreen, innerBlue;

            if (normalizedToOne[0] > 0.04045)
            {
                innerRed = Math.Pow(
                    (normalizedToOne[0] + 0.055) / (1.0 + 0.055), 2.4);
            }
            else
            {
                innerRed = normalizedToOne[0] / 12.92;
            }

            // Make green more vivid
            if (normalizedToOne[1] > 0.04045)
            {
                innerGreen = Math.Pow((normalizedToOne[1] + 0.055)
                                        / (1.0 + 0.055), 2.4);
            }
            else
            {
                innerGreen = normalizedToOne[1] / 12.92;
            }

            // Make blue more vivid
            if (normalizedToOne[2] > 0.04045)
            {
                innerBlue = Math.Pow((normalizedToOne[2] + 0.055)
                                       / (1.0 + 0.055), 2.4);
            }
            else
            {
                innerBlue = normalizedToOne[2] / 12.92;
            }

            double X = (innerRed * 0.649926 + innerGreen * 0.103455 + innerBlue * 0.197109);
            double Y = (innerRed * 0.234327 + innerGreen * 0.743075 + innerBlue * 0.022598);
            double Z = (innerRed * 0.0000000 + innerGreen * 0.053077 + innerBlue * 1.035763);

            double x = X / (X + Y + Z);
            double y = Y / (X + Y + Z);

            return new XYColor
            {
                X = x,
                Y = y
            };
        }
    }
}