using UnityEngine;
using UnityEngine.EventSystems;

public class ManualInputFix : MonoBehaviour
{
    void Update()
    {
        //Detectar explícitamente la tecla 'E'
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

            if (selectedObj != null)
            {
                //Forzar a ejecutar la acción de "Submit" (Clic) en ese objeto
                ExecuteEvents.Execute(selectedObj, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);


                Debug.Log("ManualInputFix: Se presionó E y se forzó el clic.");
            }
        }
    }
}