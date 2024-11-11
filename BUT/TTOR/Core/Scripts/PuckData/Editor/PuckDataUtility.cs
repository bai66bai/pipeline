using UnityEditor;

#if UNITY_EDITOR
namespace BUT.TTOR.Core
{
    public static class PuckDataUtility
    {
        [MenuItem("Tools/TTOR/Delete Calibrated PuckData")]
        private static void DeleteSavedPuckData()
        {
            if(EditorUtility.DisplayDialog("Delete Calibrated PuckData?",
                "Are you sure you want to delete all calibrated PuckData?", "Yes", "No"))
            {
                PuckDataManager.DeleteData();
            }
        }
    }
}
#endif
