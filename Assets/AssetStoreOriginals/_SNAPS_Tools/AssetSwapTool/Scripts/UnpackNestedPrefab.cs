﻿using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace AssetStoreOriginals._SNAPS_Tools.AssetSwapTool.Scripts
{
    public class UnpackNestedPrefab : EditorWindow
    {

        public const string GenSnapsPrototypePath = "GenSnapsPrototype";
        public const string GenSnapsHDPath = "GenSnapsHD";

        
        public const string PrefabRoot = "Assets/AssetStoreOriginals/_SNAPS_PrototypingAssets";


        private static bool IsSnapsPrototypePrefab(GameObject targetGo)
        {

            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(targetGo);

            if (prefabType != PrefabAssetType.Regular && prefabType != PrefabAssetType.Variant)
                return false;


            Regex reg = new Regex(@"_snaps[0-9][0-9][0-9].prefab$");

            var PrefabPath = SwapTool.GetOriginalPrefabPath(targetGo).ToLower();



            if (PrefabPath.Contains(PrefabRoot.ToLower()) || PrefabPath.Contains(GenSnapsPrototypePath.ToLower()) )
            {
                if (reg.IsMatch(PrefabPath))
                    return true;
            }

            return false;
        }

        private static bool IsSnapsHDPrefab(GameObject targetGo)
        {

            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(targetGo);

            if (prefabType != PrefabAssetType.Regular && prefabType != PrefabAssetType.Variant)
                return false;

            if (IsSnapsPrototypePrefab(targetGo) == false)
            {

                Regex reg = new Regex(@"_snaps[0-9][0-9][0-9].prefab$");

                var PrefabPath = SwapTool.GetOriginalPrefabPath(targetGo).ToLower();

                var SnapsPrototypePath = SwapTool.PrefabPath.Replace(Application.dataPath, string.Empty).ToLower();

                if (reg.IsMatch(PrefabPath) && !PrefabPath.Contains(SnapsPrototypePath))
                    return true;
            }

            return false;
        }

        public static string CreateGenSnapsHDFolder()
        {
            var createdPath = string.Format("Assets/{0}", GenSnapsHDPath);

            if (!AssetDatabase.IsValidFolder(createdPath))
            {
                createdPath = AssetDatabase.GUIDToAssetPath( AssetDatabase.CreateFolder("Assets", GenSnapsHDPath) );
            }

            return createdPath;
        }

        public static string CreateGenSnapsPrototypeFolder()
        {
            var createdPath = string.Format("{0}/{1}", PrefabRoot, GenSnapsPrototypePath);

            if (!AssetDatabase.IsValidFolder(createdPath))
            {
                createdPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(PrefabRoot, GenSnapsPrototypePath));
            }

            return createdPath;
        }


        private static bool SetUnpackPrefab(GameObject target)
        {
            if (target == null)
                return false;

            if (IsSnapsHDPrefab(target))
                return false;

            var PrefabPath = SwapTool.PrefabPath;

            var ObjInfo = SwapTool.GetObjectMatchingTable(PrefabPath);

            var targetPrefabPath = Path.GetFileNameWithoutExtension( SwapTool.GetOriginalPrefabPath(target).ToLower() );

            if (ObjInfo.ContainsKey(targetPrefabPath))
                return false;


            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(target);

            if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
            {
                if (target.transform.childCount == 0 && target.GetComponent<MeshRenderer>() == null)
                    return false;

                PrefabUtility.UnpackPrefabInstance(target, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            }

            return true;

        }


        public static void UnpackSelectedPrefab(GameObject currentObject)
        {
            var NestedGameObject = new Stack<GameObject>();

            NestedGameObject.Clear();


            if (SetUnpackPrefab(currentObject) == false)
                return;

            for (var i = 0; i < currentObject.transform.childCount; i++)
            {
                GameObject childGameObject = currentObject.transform.GetChild(i).gameObject;

                NestedGameObject.Push(currentObject.transform.GetChild(i).gameObject);
            }

            while (NestedGameObject.Count != 0)
            {
                GameObject gObj = NestedGameObject.Pop();

                if (SetUnpackPrefab(gObj) == false)
                    continue;

                for (var i = 0; i < gObj.transform.childCount; i++)
                {
                    NestedGameObject.Push(gObj.transform.GetChild(i).gameObject);
                }
            }

        }


    }
}