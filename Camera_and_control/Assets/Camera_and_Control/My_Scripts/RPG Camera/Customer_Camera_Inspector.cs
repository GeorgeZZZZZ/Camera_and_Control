using UnityEngine;
using UnityEditor;

//  vr: 0.1.0

[CustomEditor(typeof(Camera_Controller))]
public class Customer_Camera_Inspector : Editor {
    Camera_Controller CC;
    bool foldRPGMode, foldRTSMode;
    Mouse_Control_Cam_Types_In_RPG_Mode MCCTinRPG;
    Camera_Follow_Player_Behavior CFPB;
    Camera_Movement_Types_In_RTS_Mode CMTinRTS;
    Mouse_Control_Cam_Types_In_RTS_Mode MCCTinRTS;

    private void OnEnable()
    {
        CC = (Camera_Controller) target;
    }

    

    public override void OnInspectorGUI()
    {

        //base.OnInspectorGUI();    //  If this code is uncomment then all original values will appear in inspector

        //  General Settings---------------------------
        GUILayout.Space(10);
        EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
        CC.Player_Obj = (GameObject)EditorGUILayout.ObjectField("Player to be follow", CC.Player_Obj, typeof(GameObject),true);
        CC.X_Rote_Cent = (GameObject)EditorGUILayout.ObjectField("X axis rotation center", CC.X_Rote_Cent, typeof(GameObject), true);
        CC.Cam_Obj = (GameObject)EditorGUILayout.ObjectField("Camera", CC.Cam_Obj, typeof(GameObject), true);

        CC.Height_Offset = EditorGUILayout.FloatField("Height_Offset", CC.Height_Offset);
        CC.Look_Sensitivity = EditorGUILayout.FloatField("Look_Sensitivity", CC.Look_Sensitivity);
        CC.Look_SmoothDamp = EditorGUILayout.FloatField("Look_SmoothDamp", CC.Look_SmoothDamp);
        CC.Mouse_Scroll_Sensitivity = EditorGUILayout.FloatField("Mouse_Scroll_Sensitivity", CC.Mouse_Scroll_Sensitivity);
        CC.Mouse_Scroll_SmoothDamp = EditorGUILayout.FloatField("Mouse_Scroll_SmoothDamp", CC.Mouse_Scroll_SmoothDamp);
        CC.Max_X_Rotation_Angle = EditorGUILayout.FloatField("Max_X_Rotation_Angle", CC.Max_X_Rotation_Angle);
        CC.Min_X_Rotation_Angle = EditorGUILayout.FloatField("Min_X_Rotation_Angle", CC.Min_X_Rotation_Angle);

        if (CC.Player_Obj != null) 
        {
            //  RPG Mode settings---------------------------
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Settings in RPG Mode: ", EditorStyles.boldLabel);

            foldRPGMode = EditorGUILayout.Foldout(foldRPGMode, "");
            if (foldRPGMode)
            {
                CC.Player_Follow_SmoothDamp = EditorGUILayout.FloatField("Player_Follow_SmoothDamp", CC.Player_Follow_SmoothDamp);
                CC.Max_Cam_Distance = EditorGUILayout.FloatField("Max_Cam_Distance", CC.Max_Cam_Distance);
                CC.Min_Cam_Distance = EditorGUILayout.FloatField("Min_Cam_Distance", CC.Min_Cam_Distance);
                CC.RPG_Min_X_Rotation_Angle = EditorGUILayout.FloatField("RPG_Min_X_Rotation_Angle", CC.RPG_Min_X_Rotation_Angle);
                CC.Distance_Change_Sensitivity = EditorGUILayout.FloatField("Distance_Change_Sensitivity", CC.Distance_Change_Sensitivity);
                CC.Distance_Change_SmoothDamp = EditorGUILayout.FloatField("Distance_Change_SmoothDamp", CC.Distance_Change_SmoothDamp);
                CC.Angle_Change_Sensitivity = EditorGUILayout.FloatField("Angle_Change_Sensitivity", CC.Angle_Change_Sensitivity);
                MCCTinRPG = (Mouse_Control_Cam_Types_In_RPG_Mode)EditorGUILayout.EnumPopup("Mouse control camera types", MCCTinRPG);
                switch (MCCTinRPG)
                {
                    case Mouse_Control_Cam_Types_In_RPG_Mode.RPG_Mid_Mous_Rote_Cam:
                        CC.RPG_Mid_Mous_Rote_Cam = true;
                        CC.RPG_Edge_Rote_Cam = false;
                        CC.RPG_Dir_Rote_Cam = false;
                        break;
                    case Mouse_Control_Cam_Types_In_RPG_Mode.RPG_Edge_Rote_Cam:
                        CC.RPG_Mid_Mous_Rote_Cam = false;
                        CC.RPG_Edge_Rote_Cam = true;
                        CC.RPG_Dir_Rote_Cam = false;
                        CC.Edge_Boundary = EditorGUILayout.IntField("Mouse rotation Edge boundary", CC.Edge_Boundary);
                        break;
                    case Mouse_Control_Cam_Types_In_RPG_Mode.RPG_Dir_Rote_Cam:
                        CC.RPG_Mid_Mous_Rote_Cam = false;
                        CC.RPG_Edge_Rote_Cam = false;
                        CC.RPG_Dir_Rote_Cam = true;
                        break;
                }
                CFPB = (Camera_Follow_Player_Behavior)EditorGUILayout.EnumPopup("Camera follow player behavior", CFPB);
                switch (CFPB)
                {
                    case Camera_Follow_Player_Behavior.RPG_Classic_Cam_Follow:
                        CC.RPG_Classic_Cam_Follow = true;
                        CC.RPG_Complet_Cam_Follow = false;
                        break;
                    case Camera_Follow_Player_Behavior.RPG_Complet_Cam_Follow:
                        CC.RPG_Classic_Cam_Follow = false;
                        CC.RPG_Complet_Cam_Follow = true;
                        break;
                }
            }
        }
        //  RTS Mode settings---------------------------
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Settings in RTS Mode: ", EditorStyles.boldLabel);

        foldRTSMode = EditorGUILayout.Foldout(foldRTSMode, "");
        if (foldRTSMode)
        {
            CC.RTS_Plan_Fir_View_Flag = EditorGUILayout.Toggle("RTS_Plan_Fir_View_Flag", CC.RTS_Plan_Fir_View_Flag);
            if (CC.RTS_Plan_Fir_View_Flag)
            {
                CC.RTS_Fir_Cam_Distance = EditorGUILayout.FloatField("RTS_Fir_Cam_Distance", CC.RTS_Fir_Cam_Distance);
                CC.RTS_Plan_Sec_View_Flag = EditorGUILayout.Toggle("RTS_Plan_Sec_View_Flag", CC.RTS_Plan_Sec_View_Flag);
                if (CC.RTS_Plan_Sec_View_Flag)
                {
                    CC.RTS_Sec_Cam_Distance = EditorGUILayout.FloatField("RTS_Sec_Cam_Distance", CC.RTS_Sec_Cam_Distance);
                }
            }

            MCCTinRTS = (Mouse_Control_Cam_Types_In_RTS_Mode)EditorGUILayout.EnumPopup("Mouse control camera types", MCCTinRTS);
            switch (MCCTinRTS)
            {
                case Mouse_Control_Cam_Types_In_RTS_Mode.RTS_Complet_Cam_Follow:
                    CC.RTS_Complet_Cam_Follow = true;
                    CC.RTS_Mid_Mous_Rote_Cam = false;
                    break;
                case Mouse_Control_Cam_Types_In_RTS_Mode.RTS_Mid_Mous_Rote_Cam:
                    CC.RTS_Complet_Cam_Follow = true;
                    CC.RTS_Mid_Mous_Rote_Cam = false;
                    break;
            }

            CC.Cam_Move_Speed = EditorGUILayout.FloatField("Cam_Move_Speed", CC.Cam_Move_Speed);
            CMTinRTS = (Camera_Movement_Types_In_RTS_Mode)EditorGUILayout.EnumPopup("Camera movement types", CMTinRTS);
            switch (CMTinRTS)
            {
                case Camera_Movement_Types_In_RTS_Mode.Move_Camera_towards_cam_Facing:
                    CC.Move_Camera_towards_cam_Facing = true;
                    CC.Move_Camera_Along_World_Axis = false;
                    break;
                case Camera_Movement_Types_In_RTS_Mode.Move_Camera_Along_World_Axis:
                    CC.Move_Camera_towards_cam_Facing = false;
                    CC.Move_Camera_Along_World_Axis = true;
                    break;
            }
            CC.Move_Camera_at_Edge = EditorGUILayout.Toggle("Move_Camera_at_Edge", CC.Move_Camera_at_Edge);

        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Debug: ", EditorStyles.boldLabel);
        CC.Move_Debug = EditorGUILayout.Toggle("Move_Debug", CC.Move_Debug);

    }

}

public enum Mouse_Control_Cam_Types_In_RPG_Mode
{
    RPG_Dir_Rote_Cam,
    RPG_Mid_Mous_Rote_Cam,
    RPG_Edge_Rote_Cam
}

public enum Camera_Follow_Player_Behavior
{
    RPG_Classic_Cam_Follow,
    RPG_Complet_Cam_Follow
}

public enum Camera_Movement_Types_In_RTS_Mode
{
    Move_Camera_towards_cam_Facing,
    Move_Camera_Along_World_Axis
}

public enum Mouse_Control_Cam_Types_In_RTS_Mode
{
    RTS_Complet_Cam_Follow,
    RTS_Mid_Mous_Rote_Cam
}
