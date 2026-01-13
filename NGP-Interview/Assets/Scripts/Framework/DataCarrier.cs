using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DataCarrier : MonoBehaviour
{
    UnityAction<Scene, LoadSceneMode> action;
    public object arg;
    bool destroyed;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /*Para salvar algum objeto e usa-lo na acao desejada, primeiro cria-se um DataCarrier utilizando o construtor estatico,
    * passando o argumento que deseja salvar, e utiliza-se a funcao local HoldDataWithArgs, acessando a variavel do DataCarrier criado
    */
    public static DataCarrier CreateDataCarrier(object data)
    {
        GameObject obj = new GameObject("DataCarrierArgs");
        DataCarrier dataCarrier;
        obj.hideFlags = HideFlags.HideAndDontSave;
        dataCarrier = obj.AddComponent<DataCarrier>();
        dataCarrier.arg = data;
        return dataCarrier;
    }
    public void HoldDataWithArg(UnityAction<Scene, LoadSceneMode> doOnLoadScene)
    {
        action = doOnLoadScene;
        SceneManager.sceneLoaded += doOnLoadScene;
        SceneManager.sceneLoaded += DestroyHolder;
    }
    /* Caso apenas queira realizar uma função ao carregar a cena, utiliza-se a funcao estatica HoldData
    */

    public static void HoldData(UnityAction<Scene,LoadSceneMode> doOnLoadScene)
    {
        GameObject obj = new GameObject("DataCarrierStatic");
        DataCarrier dataCarrier;
        obj.hideFlags = HideFlags.HideAndDontSave;
        dataCarrier = obj.AddComponent<DataCarrier>();
        dataCarrier.action = doOnLoadScene;
        SceneManager.sceneLoaded += doOnLoadScene;
        SceneManager.sceneLoaded += dataCarrier.DestroyHolder;
    }

    private void DestroyHolder(Scene arg0, LoadSceneMode arg1)
    {
        if (destroyed) return;
        if (this != null && gameObject != null)
        {
            destroyed = true;
            if (action != null)
            {
                SceneManager.sceneLoaded -= action;
            }
            SceneManager.sceneLoaded -= DestroyHolder;
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= DestroyHolder;
    }
}
