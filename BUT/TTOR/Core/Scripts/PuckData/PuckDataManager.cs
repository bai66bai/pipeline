using BUT.TTOR.Core.Utils;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace BUT.TTOR.Core
{
    public static class PuckDataManager
    {
        public static PuckData[] _puckData;

        private static bool _isDataLoaded = false;


        public static PuckData[] GetPuckData()
        {
            if (!_isDataLoaded) { LoadData(); }
            return _puckData;
        }

        public static void SaveData(PuckData puckData)
        {
            string dirPath = "C:\\PuckData/";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            BinaryFormatter formatter = new BinaryFormatter();
            string path = dirPath + "Puck_" + puckData.name + ".PuckData";

            //New Card File
            FileStream stream = new FileStream(path, FileMode.Create);
            //Insert data into file
            formatter.Serialize(stream, new SerializablePuckData(puckData));
            stream.Close();

            //LoadData();
            TTOR_Logger.Log("<color=lime><b>Calibration data saved for puck: </b></color>" + puckData.name + $"in {Application.persistentDataPath}/Data/", TTOR_Logger.LogMask.CalibrationEvents);

            puckData.IsCalibrated = true;
        }

        private static void LoadData()
        {
            _puckData = Resources.LoadAll<PuckData>("");

            for (int i = 0; i < _puckData.Length; i++)
            {
                string path = "C:\\PuckData/" + "Puck_" + _puckData[i].name + ".PuckData";
                //Debug.Log(path);
                if (File.Exists(path))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    FileStream stream = new FileStream(path, FileMode.Open);
                    SerializablePuckData loadedData = formatter.Deserialize(stream) as SerializablePuckData;
                    _puckData[i].SetData(loadedData);

                    stream.Close();
                }
            }
        }

        public static void DeleteData()
        {
            string path = "C:\\PuckData/";
            if (Directory.Exists(path))
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                directory.Delete(true);
                Directory.CreateDirectory(path);

                _puckData = Resources.LoadAll<PuckData>("");
                for (int i = 0; i < _puckData.Length; i++)
                {
                    _puckData[i].ResetValues();
                }

                TTOR_Logger.Log("<color=lime><b>All calibration data has been deleted</b></color>", TTOR_Logger.LogMask.CalibrationEvents);
            }
            else
            {
                TTOR_Logger.LogWarning("<color=orange><b>Could not delete calibration data, directory missing:</b></color> " + path);
            }
        }

        public static void DeleteData(string puckId)
        {
            string path = "C:\\PuckData/" + "Puck_" + puckId + ".PuckData";
            if (File.Exists(path))
            {
                File.Delete(path);
                //Debug.Log("PuckData "+ puckId + " Deleted");
                TTOR_Logger.Log("<color=lime><b>Calibration data removed for puck: </b></color>" + puckId, TTOR_Logger.LogMask.CalibrationEvents);
            }
            else
            {
                //Debug.LogWarning("Could not delete PuckData" + puckId + ", no data exists or file missing: " + path);
                TTOR_Logger.LogWarning("<color=orange><b>Could not delete calibration data for puck: </b></color>" + puckId + ", data already removed or file missing: " + path + "");
            }

            PuckData pd = _puckData.ToList().Find(pd => pd.name == puckId);
            if (pd == null)
            {
                TTOR_Logger.LogWarning("<color=orange><b>Could not delete calibration data for puck: </b></color>" + puckId + " _puckData does not contain a puck with Id: " + puckId);
            }
            else
            {
                pd.ResetValues();
            }
        }

        public static PuckData GetPuckDataByName(string name)
        {

            PuckData pd = _puckData.ToList().Find(pd => pd.name == name);
            if (pd == null)
            {
                TTOR_Logger.LogWarning("<color=orange><b>Could not GetPuckDataByName: </b></color> No ScriptableObject found with name: " + name);
                return null;
            }

            return pd;
        }
    }
}
