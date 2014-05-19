using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using Ionic.BZip2;
using Ionic.Zip;

namespace HighFreqConverter
{
    class waterBody
    {
        public string id;
        public string name;
        public string region;
        public string exactCoord;
        public string elevation;
        public string depth;
        public string depthCalibration;
        public string notes;
        public string siteName;
        public string longitude;
        public string latitude;
    }

    class metaInfo
    {
        public string name;
        public string unit;
        public qcData qc;
    }

    class qcData
    {
        public string model;
        public string sensorHeight;
        public string calibration;
        public string calibrationType;
        public string notes;
    }


    class Program
    {
        static waterBody waterMeta = new waterBody();
        static string waterType = "Lake";
        static List<metaInfo> metaData = new List<metaInfo>();

        public enum DateTimeComponentType
        {
            YYYY,
            MM,
            DD,
            hh,
            mm,
            YYYYMM,
            YYYYMMDD,
            YYYYMMDDhhmm,
            YYYYMMDDhh,
            YYYYDDMM,
            YYYYDDMMhhmm,
            YYYYDDMMhh,
            MMDD,
            DDMM,
            DDMMYYYY,
            DDMMYYYYhhmm,
            MMDDYYYY,
            MMDDYYYYhhmm,
            hhmm,
            Date,
            DateTime,
            Time
        }

        public class DateTimeComponent
        {
            public DateTimeComponentType Type;
        }

        static void Main(string[] args)
        {
            string fileName = "Lake_Rotoehu_Buoy_Apr2011-Jul2012.csv";
            string[] lines = File.ReadAllLines(fileName);
            string header = parseHeader(lines[0]);
            readInWaterMeta();
            var newFileName = "";

            newFileName = "Lake Rotoehu_April 2011-July 2012.csv";
            var writer = new StreamWriter(newFileName);
            writer.WriteLine(header);

            for (int i = 1; i < lines.Count(); i++)
            {
                writer.WriteLine(cleanData(lines[i]));
            }
            writer.Close();
            using (XmlWriter xmlWriter = XmlWriter.Create("LernzMeta.xml"))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("lernz-meta");
                xmlWriter.WriteStartElement("variables");
                foreach (metaInfo v in metaData)
                {
                    xmlWriter.WriteStartElement("variable");
                    xmlWriter.WriteElementString("name", v.name);
                    xmlWriter.WriteElementString("unit", v.unit);

                    xmlWriter.WriteStartElement("qc-information");
                    if (v.qc.model != null)
                    {

                        xmlWriter.WriteStartElement("qc-column");
                        xmlWriter.WriteElementString("parameter", "Model");

                        xmlWriter.WriteElementString("value", v.qc.model);
                        xmlWriter.WriteEndElement();// qc-column
                    }
                    if (v.qc.notes != null)
                    {

                        xmlWriter.WriteStartElement("qc-column");
                        xmlWriter.WriteElementString("parameter", "Notes");

                        xmlWriter.WriteElementString("value", v.qc.notes);
                        xmlWriter.WriteEndElement();// qc-column
                    }
                    if (v.qc.calibration != null)
                    {

                        xmlWriter.WriteStartElement("qc-column");
                        xmlWriter.WriteElementString("parameter", "Calibration");

                        xmlWriter.WriteElementString("value", v.qc.calibration);
                        xmlWriter.WriteEndElement();// qc-column
                    }
                    if (v.qc.calibrationType != null)
                    {

                        xmlWriter.WriteStartElement("qc-column");
                        xmlWriter.WriteElementString("parameter", "Calibration Type");

                        xmlWriter.WriteElementString("value", v.qc.calibrationType);
                        xmlWriter.WriteEndElement();// qc-column
                    }
                    if (v.qc.sensorHeight != null)
                    {

                        xmlWriter.WriteStartElement("qc-column");
                        xmlWriter.WriteElementString("parameter", "Sensor height (m)");

                        xmlWriter.WriteElementString("value", v.qc.sensorHeight);
                        xmlWriter.WriteEndElement();// qc-column
                    }

                    xmlWriter.WriteEndElement();// qc-information
                    xmlWriter.WriteEndElement();// variable
                }
                xmlWriter.WriteEndElement(); // end variables


                xmlWriter.WriteStartElement("waterbody");

                xmlWriter.WriteElementString("waterbody-id", waterMeta.id);
                xmlWriter.WriteElementString("waterbody-name", waterMeta.name);
                xmlWriter.WriteElementString("region", waterMeta.region);
                xmlWriter.WriteElementString("exact-coord", waterMeta.exactCoord);
                xmlWriter.WriteElementString("elevation", waterMeta.elevation);
                xmlWriter.WriteElementString("water-depth-measured", waterMeta.depth);
                xmlWriter.WriteElementString("depth-calibration", waterMeta.depthCalibration);
                xmlWriter.WriteElementString("notes", waterMeta.notes);
                xmlWriter.WriteEndElement();//waterbody


                xmlWriter.WriteEndElement();//lernz-meta
            }

                     try
                    {

                        using (XmlWriter xmlWrite = XmlWriter.Create("mets.xml"))
                        {
                            xmlWrite.WriteStartDocument();
                            xmlWrite.WriteRaw("\n<mets ID=\"sort-mets_mets\" OBJID=\"sword-mets\" LABEL=\"DSpace SWORD Item\" PROFILE=\"DSpace METS SIP Profile 1.0\" xmlns=\"http://www.loc.gov/METS/\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.loc.gov/METS/ http://www.loc.gov/standards/mets/mets.xsd\">\n");
                            xmlWrite.WriteRaw("\t<metsHdr CREATEDATE=\"" + DateTime.UtcNow + "\">\n");
                            xmlWrite.WriteRaw("\t\t<agent ROLE=\"CUSTODIAN\" TYPE=\"ORGANIZATION\">\n");
                            xmlWrite.WriteRaw("\t\t\t<name>Unkown</name>\n");
                            xmlWrite.WriteRaw("\t\t</agent>\n");
                            xmlWrite.WriteRaw("\t</metsHdr>\n");
                            xmlWrite.WriteRaw("<dmdSec ID=\"sword-mets-dmd-1\" GROUPID=\"sword-mets-dmd-1_group-1\">\n");
                            xmlWrite.WriteRaw("<mdWrap LABEL=\"SWAP Metadata\" MDTYPE=\"OTHER\" OTHERMDTYPE=\"EPDCX\" MIMETYPE=\"text/xml\">\n");
                            xmlWrite.WriteStartElement("xmlData");
                            xmlWrite.WriteRaw("<epdcx:descriptionSet xmlns:epdcx=\"http://purl.org/eprint/epdcx/2006-11-16/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://purl.org/eprint/epdcx/2006-11-16/ http://www.ukoln.ac.uk/repositories/eprints-application-profile/epdcx/xsd/2006-11-16/epdcx.xsd\">\n");
                            xmlWrite.WriteRaw("<epdcx:description epdcx:resourceId=\"sword-mets-epdcx-1\">\n");
                            xmlWrite.WriteRaw("<epdcx:statement epdcx:propertyURI=\"lernz.data.category\">\n");
                            xmlWrite.WriteRaw("<epdcx:valueString>High frequency</epdcx:valueString>\n");
                            xmlWrite.WriteRaw("</epdcx:statement>\n");
                            xmlWrite.WriteRaw("<epdcx:statement epdcx:propertyURI=\"lernz.data.type\">");
                            xmlWrite.WriteRaw("<epdcx:valueString>" + waterType + "</epdcx:valueString>\n");
                            xmlWrite.WriteRaw("</epdcx:statement>\n");
                            xmlWrite.WriteRaw("<epdcx:statement epdcx:propertyURI=\"http://purl.org/dc/elements/1.1/title\">\n");
                            xmlWrite.WriteRaw("<epdcx:valueString>" + newFileName.Replace("_"," ") + "</epdcx:valueString>\n");
                            xmlWrite.WriteRaw("</epdcx:statement>\n");
                            xmlWrite.WriteRaw("<epdcx:statement epdcx:propertyURI=\"lernz.data.provenance\">");
                            xmlWrite.WriteRaw("<epdcx:valueString>SWORD submission</epdcx:valueString>");
                            xmlWrite.WriteRaw("</epdcx:statement>\n");
                            xmlWrite.WriteRaw("<epdcx:statement epdcx:propertyURI=\"http://purl.org/dc/elements/1.1/creator\">\n");
                            xmlWrite.WriteRaw("<epdcx:valueString>McBride, Chris</epdcx:valueString>\n");
                            xmlWrite.WriteRaw("</epdcx:statement>\n");
                            xmlWrite.WriteRaw("<epdcx:statement epdcx:propertyURI=\"http://purl.org/dc/terms/abstract\">\n");
                            xmlWrite.WriteRaw("<epdcx:valueString>High Frequency data from a bouy on Lake Rotoehu from April 2011 to July 2012 </epdcx:valueString>\n");
                            xmlWrite.WriteRaw("</epdcx:statement>\n");
                            xmlWrite.WriteRaw("</epdcx:description>\n");
                            xmlWrite.WriteRaw("</epdcx:descriptionSet>\n");
                            xmlWrite.WriteEndElement();
                            xmlWrite.WriteRaw("\n</mdWrap>\n");
                            xmlWrite.WriteRaw("</dmdSec>\n");
                            xmlWrite.WriteRaw("\t<fileSec>\n");
                            xmlWrite.WriteRaw("\t\t<fileGrp ID=\"sword-mets-fgrp-1\" USE=\"CONTENT\">\n");
                            xmlWrite.WriteRaw("\t\t\t<file GROUPID=\"sword-mets-fgid-0\" ID=\"sword-mets-file-0\" MIMETYPE=\"text/csv\">\n");
                            xmlWrite.WriteRaw("\t\t\t\t<FLocat LOCTYPE=\"URL\" xlink:href=\"" + newFileName + "\" />");
                            xmlWrite.WriteRaw("\t\t\t</file>\n");
                            xmlWrite.WriteRaw("\t\t\t<file GROUPID=\"sword-mets-fgid-1\" ID=\"sword-mets-file-1\" MIMETYPE=\"application/xml\">\n");
                            xmlWrite.WriteRaw("\t\t\t\t<FLocat LOCTYPE=\"URL\" xlink:href=\"LernzMeta.xml\" />\n");
                            xmlWrite.WriteRaw("\t\t\t</file>\n");
                            xmlWrite.WriteRaw("\t\t</fileGrp>\n");
                            xmlWrite.WriteRaw("\t</fileSec>\n");
                            xmlWrite.WriteRaw("\t<structMap ID=\"sword-mets-struct-1\" LABEL=\"structure\" TYPE=\"LOGICAL\">\n");
                            xmlWrite.WriteRaw("\t\t<div ID=\"sword-mets-div-1\" DMDID=\"sword-mets-dmd-1\" TYPE=\"SWORD Object\">\n");
                            xmlWrite.WriteRaw("\t\t\t<div ID=\"sword-mets-div-2\" TYPE=\"File\">\n");
                            xmlWrite.WriteRaw("\t\t\t\t<fptr FILEID=\"sword-mets-file-0\" />\n");
                            xmlWrite.WriteRaw("\t\t\t\t<fptr FILEID=\"sword-mets-file-1\" />\n");
                            xmlWrite.WriteRaw("\t\t\t</div>\n");
                            xmlWrite.WriteRaw("\t\t</div>\n");
                            xmlWrite.WriteRaw("\t</structMap>\n");
                            xmlWrite.WriteRaw("</mets>\n");

                            xmlWrite.WriteEndDocument();
                        }
                     }
                     catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.AddFile("mets.xml");
                        zip.AddFile(newFileName);
                        zip.AddFile("LernzMeta.xml");
                        zip.Save(newFileName + ".zip");
                    }

                    File.Delete(newFileName);
                    File.Delete("LernzMeta.xml");
                    metaData = new List<metaInfo>();
            Console.WriteLine(header);
            Console.ReadLine();
        }

        private static string cleanData(string p)
        {
            bool first = true;
            string finalData = "";

            string[] lineData = p.Split('\t');

            foreach (string s in lineData)
            {
                if(first == true)
                {
                finalData += s;
                finalData += "\t";
                                finalData += waterMeta.latitude;
            finalData += "\t";
            finalData += waterMeta.longitude;
            finalData += "\t";
            finalData += waterMeta.siteName;
            finalData += "\t";
            first = false;
                }
                else
                {
                                    finalData += s;
                finalData += "\t";
                }
            }
            return finalData;
        }
        


        

private static string parseHeader(string p)
{
    string toReturn = "Date and Time";
            toReturn += "\t";
            toReturn += "Latitude";
            toReturn += "\t";
            toReturn += "Longitude";
            toReturn += "\t";
            toReturn += "Site ID/Site name";
            toReturn += "\t";
    string[] headers = p.Split(','); 

    foreach (var header in headers)
    {
        DateTimeComponentType dateTimeComponent;
        var cleansedHeader = header.Replace(" ", "").Replace("-", "").Replace("/", "").Replace(":", "");
        var isDateTimeComponent = Enum.TryParse(cleansedHeader, out dateTimeComponent);
        if (isDateTimeComponent)
        {

        }
        else
        {

            if (header.Contains("_") && header.Substring(2, 5).Contains('_'))
            {

                string[] hdrSplit = header.Split('_');
                if (!String.IsNullOrWhiteSpace(hdrSplit[0]) || !String.IsNullOrWhiteSpace(hdrSplit[1]))
                {
                    String[] hdrSplitTwo = hdrSplit[1].Split('(');
                    string sen;
                    if (!String.IsNullOrWhiteSpace(hdrSplitTwo[1]))
                    {
                        hdrSplitTwo[1] = hdrSplitTwo[1].TrimEnd(')'); // If it gets to here has unit and a header name of some sort
                        if (!String.IsNullOrWhiteSpace(hdrSplitTwo[1]))
                        {

                            if (!string.IsNullOrWhiteSpace(hdrSplitTwo[0]))
                            {
                                sen = hdrSplitTwo[0][0].ToString();

                                if (!String.IsNullOrEmpty(sen))
                                {

                                    hdrSplitTwo[0] = hdrSplitTwo[0].TrimStart(sen[0]);

                                    toReturn += hdrSplit[0] + "_" + hdrSplit[1].Remove(hdrSplit[1].IndexOf('('));
                                    toReturn += "\t";
                                    metaInfo temp = new metaInfo();
                                    temp.name = hdrSplit[0] + "_" + hdrSplit[1].Remove(hdrSplit[1].IndexOf('('));
                                    temp.unit = hdrSplitTwo[1];
                                    temp.qc = new qcData();
                                    temp.qc.sensorHeight = hdrSplitTwo[0];
                                    metaData.Add(temp);


                                }
                            }
                            else
                            {
                              
                            }

                        }
                        else
                        {
                            
                        }
                    }
                }


                else
                {
                  
                }


            }
        }
    }
       return toReturn;
}
        
        
        static void readInWaterMeta()
        {
            var reader = new StreamReader("waterData.txt");
            string temp;
            reader.ReadLine();
            reader.ReadLine();
            temp = reader.ReadLine();
            if (temp.Length <= 14)
            {
                waterMeta.id = "";
            }
            else
            {
                waterMeta.id = temp.Substring(14);
            }
            waterMeta.name = reader.ReadLine().Substring(16);
            waterMeta.region = reader.ReadLine().Substring(8);
            waterMeta.exactCoord = reader.ReadLine().Substring(13);
            waterMeta.elevation = reader.ReadLine().Substring(10);
            waterMeta.depth = reader.ReadLine().Substring(22);
            waterMeta.depthCalibration = reader.ReadLine().Substring(18);
            temp = reader.ReadLine();
            if (temp.Length <= 7)
            {
                waterMeta.notes = "";
            }
            else
            {
                waterMeta.notes = temp.Substring(7);
            }
            waterMeta.siteName = reader.ReadLine().Substring(11);
            waterMeta.longitude = reader.ReadLine().Substring(11);
            waterMeta.latitude = reader.ReadLine().Substring(11);
            waterType = reader.ReadLine().Substring(11);
        }
    }
}
