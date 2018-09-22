﻿using System.IO;
using UnityEngine;
using UnityEngine.UI;
using VRM;


public class VRMRuntimeExporter : MonoBehaviour
{
    [SerializeField]
    Button m_loadButton;

    [SerializeField]
    Button m_exportButton;

    GameObject m_model;

    private void Awake()
    {
        m_loadButton.onClick.AddListener(OnLoadClicked);

        m_exportButton.onClick.AddListener(OnExportClicked);
    }

    private void Update()
    {
        m_exportButton.interactable = (m_model != null);
    }

    #region Load
    void OnLoadClicked()
    {
#if UNITY_STANDALONE_WIN
        var path = FileDialogForWindows.FileDialog("open VRM", ".vrm");
#else
        var path = Application.dataPath + "/default.vrm";
#endif
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var bytes = File.ReadAllBytes(path);
        // なんらかの方法でByte列を得た

        var context = new VRMImporterContext(UniGLTF.UnityPath.FromFullpath(path));

        // GLB形式でJSONを取得しParseします
        context.ParseGlb(bytes);


        // metaを取得(todo: thumbnailテクスチャのロード)
        var meta = context.ReadMeta();
        Debug.LogFormat("meta: title:{0}", meta.Title);

        // ParseしたJSONをシーンオブジェクトに変換していく
        LoadAsync(context);
    }

    void LoadAsync(VRMImporterContext context)
    {
        var now = Time.time;
        VRMImporter.LoadVrmAsync(context, go =>
        {
            var delta = Time.time - now;
            Debug.LogFormat("LoadVrmAsync {0:0.0} seconds", delta);
            OnLoaded(go);
        });
    }

    void OnLoaded(GameObject root)
    {
        if (m_model != null)
        {
            GameObject.Destroy(m_model.gameObject);
        }
        m_model = root;

        root.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    #endregion

    #region Export
    void OnExportClicked()
    {
//#if UNITY_STANDALONE_WIN
#if false
        var path = FileDialogForWindows.SaveDialog("save VRM", Application.dataPath + "/export.vrm");
#else
        var path = Application.dataPath + "/../export.vrm";
#endif
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var vrm = VRMExporter.Export(m_model);
        var bytes = vrm.ToGlbBytes();
        File.WriteAllBytes(path, bytes);
        Debug.LogFormat("export to {0}", path);
    }

    void OnExported(UniGLTF.glTF vrm)
    {
        Debug.LogFormat("exported");
    }
    #endregion
}
