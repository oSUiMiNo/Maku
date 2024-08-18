using System.Collections.Generic;
using UnityEngine;

// キー打った時のイベント
public class InputEventHandler : SingletonCompo<InputEventHandler>
{
    #region Key ==================================
    public static event System.Action On_A;
    public static event System.Action On_B;
    public static event System.Action On_C;
    public static event System.Action On_D;
    public static event System.Action On_E;
    public static event System.Action On_F;
    public static event System.Action On_G;
    public static event System.Action On_H;
    public static event System.Action On_I;
    public static event System.Action On_J;
    public static event System.Action On_K;
    public static event System.Action On_L;
    public static event System.Action On_M;
    public static event System.Action On_N;
    public static event System.Action On_O;
    public static event System.Action On_P;
    public static event System.Action On_Q;
    public static event System.Action On_R;
    public static event System.Action On_S;
    public static event System.Action On_T;
    public static event System.Action On_U;
    public static event System.Action On_V;
    public static event System.Action On_W;
    public static event System.Action On_X;
    public static event System.Action On_Y;
    public static event System.Action On_Z;

    public static event System.Action On_0;
    public static event System.Action On_1;
    public static event System.Action On_2;
    public static event System.Action On_3;
    public static event System.Action On_4;
    public static event System.Action On_5;
    public static event System.Action On_6;
    public static event System.Action On_7;
    public static event System.Action On_8;
    public static event System.Action On_9;

    public static event System.Action On_F1;
    public static event System.Action On_F2;
    public static event System.Action On_F3;
    public static event System.Action On_F4;
    public static event System.Action On_F5;
    public static event System.Action On_F6;
    public static event System.Action On_F7;
    public static event System.Action On_F8;
    public static event System.Action On_F9;
    public static event System.Action On_F10;
    public static event System.Action On_F11;
    public static event System.Action On_F12;

    public static event System.Action On_BracketL;
    public static event System.Action On_BracketR;
    public static event System.Action On_Slash;
    public static event System.Action On_BackSlash;
    public static event System.Action On_At;
    public static event System.Action On_Period;
    public static event System.Action On_Comma;
    public static event System.Action On_Colon;
    public static event System.Action On_Semicolon;

    public static event System.Action On_BackSpace;
    public static event System.Action On_Delete;
    public static event System.Action On_Tab;
    public static event System.Action On_Enter;
    public static event System.Action On_Esc;
    public static event System.Action On_Space;
    public static event System.Action On_Up;
    public static event System.Action On_Down;
    public static event System.Action On_Right;
    public static event System.Action On_Left;
    public static event System.Action On_Ctrl;
    public static event System.Action On_Alt;
    #endregion

    #region KeyDown ==================================
    public static event System.Action OnDown_A;
    public static event System.Action OnDown_B;
    public static event System.Action OnDown_C;
    public static event System.Action OnDown_D;
    public static event System.Action OnDown_E;
    public static event System.Action OnDown_F;
    public static event System.Action OnDown_G;
    public static event System.Action OnDown_H;
    public static event System.Action OnDown_I;
    public static event System.Action OnDown_J;
    public static event System.Action OnDown_K;
    public static event System.Action OnDown_L;
    public static event System.Action OnDown_M;
    public static event System.Action OnDown_N;
    public static event System.Action OnDown_O;
    public static event System.Action OnDown_P;
    public static event System.Action OnDown_Q;
    public static event System.Action OnDown_R;
    public static event System.Action OnDown_S;
    public static event System.Action OnDown_T;
    public static event System.Action OnDown_U;
    public static event System.Action OnDown_V;
    public static event System.Action OnDown_W;
    public static event System.Action OnDown_X;
    public static event System.Action OnDown_Y;
    public static event System.Action OnDown_Z;
    public static event System.Action OnDown_0;
    public static event System.Action OnDown_1;
    public static event System.Action OnDown_2;
    public static event System.Action OnDown_3;
    public static event System.Action OnDown_4;
    public static event System.Action OnDown_5;
    public static event System.Action OnDown_6;
    public static event System.Action OnDown_7;
    public static event System.Action OnDown_8;
    public static event System.Action OnDown_9;

    public static event System.Action OnDown_F1;
    public static event System.Action OnDown_F2;
    public static event System.Action OnDown_F3;
    public static event System.Action OnDown_F4;
    public static event System.Action OnDown_F5;
    public static event System.Action OnDown_F6;
    public static event System.Action OnDown_F7;
    public static event System.Action OnDown_F8;
    public static event System.Action OnDown_F9;
    public static event System.Action OnDown_F10;
    public static event System.Action OnDown_F11;
    public static event System.Action OnDown_F12;

    public static event System.Action OnDown_BracketL;
    public static event System.Action OnDown_BracketR;
    public static event System.Action OnDown_Slash;
    public static event System.Action OnDown_BackSlash;
    public static event System.Action OnDown_At;
    public static event System.Action OnDown_Period;
    public static event System.Action OnDown_Comma;
    public static event System.Action OnDown_Colon;
    public static event System.Action OnDown_Semicolon;

    public static event System.Action OnDown_BackSpace;
    public static event System.Action OnDown_Delete;
    public static event System.Action OnDown_Tab;
    public static event System.Action OnDown_Enter;
    public static event System.Action OnDown_Esc;
    public static event System.Action OnDown_Space;
    public static event System.Action OnDown_Up;
    public static event System.Action OnDown_Down;
    public static event System.Action OnDown_Right;
    public static event System.Action OnDown_Left;
    public static event System.Action OnDown_Ctrl;
    public static event System.Action OnDown_Alt;
    #endregion

    #region KeyUp ==================================
    public static event System.Action OnUp_A;
    public static event System.Action OnUp_B;
    public static event System.Action OnUp_C;
    public static event System.Action OnUp_D;
    public static event System.Action OnUp_E;
    public static event System.Action OnUp_F;
    public static event System.Action OnUp_G;
    public static event System.Action OnUp_H;
    public static event System.Action OnUp_I;
    public static event System.Action OnUp_J;
    public static event System.Action OnUp_K;
    public static event System.Action OnUp_L;
    public static event System.Action OnUp_M;
    public static event System.Action OnUp_N;
    public static event System.Action OnUp_O;
    public static event System.Action OnUp_P;
    public static event System.Action OnUp_Q;
    public static event System.Action OnUp_R;
    public static event System.Action OnUp_S;
    public static event System.Action OnUp_T;
    public static event System.Action OnUp_U;
    public static event System.Action OnUp_V;
    public static event System.Action OnUp_W;
    public static event System.Action OnUp_X;
    public static event System.Action OnUp_Y;
    public static event System.Action OnUp_Z;

    public static event System.Action OnUp_0;
    public static event System.Action OnUp_1;
    public static event System.Action OnUp_2;
    public static event System.Action OnUp_3;
    public static event System.Action OnUp_4;
    public static event System.Action OnUp_5;
    public static event System.Action OnUp_6;
    public static event System.Action OnUp_7;
    public static event System.Action OnUp_8;
    public static event System.Action OnUp_9;

    public static event System.Action OnUp_F1;
    public static event System.Action OnUp_F2;
    public static event System.Action OnUp_F3;
    public static event System.Action OnUp_F4;
    public static event System.Action OnUp_F5;
    public static event System.Action OnUp_F6;
    public static event System.Action OnUp_F7;
    public static event System.Action OnUp_F8;
    public static event System.Action OnUp_F9;
    public static event System.Action OnUp_F10;
    public static event System.Action OnUp_F11;
    public static event System.Action OnUp_F12;

    public static event System.Action OnUp_BracketL;
    public static event System.Action OnUp_BracketR;
    public static event System.Action OnUp_Slash;
    public static event System.Action OnUp_BackSlash;
    public static event System.Action OnUp_At;
    public static event System.Action OnUp_Period;
    public static event System.Action OnUp_Comma;
    public static event System.Action OnUp_Colon;
    public static event System.Action OnUp_Semicolon;

    public static event System.Action OnUp_BackSpace;
    public static event System.Action OnUp_Delete;
    public static event System.Action OnUp_Tab;
    public static event System.Action OnUp_Enter;
    public static event System.Action OnUp_Esc;
    public static event System.Action OnUp_Space;
    public static event System.Action OnUp_Up;
    public static event System.Action OnUp_Down;
    public static event System.Action OnUp_Right;
    public static event System.Action OnUp_Left;
    public static event System.Action OnUp_Ctrl;
    public static event System.Action OnUp_Alt;
    #endregion

    #region マウス ===============================================
    // ホイール
    bool onWheel = false;
    List<float> recentWheelDeltas = new List<float>();
    List<float> recentFps = new List<float>();
    [SerializeField] float stopWheelSecond = 0.3f; // 実際にスクロールをやめてから何秒間スクロールしなければスクロールが止まったと判定されるかの時間
    public static event System.Action On_Wheel;
    public static event System.Action OnStart_Wheel;
    public static event System.Action OnStop_Wheel;
    public static event System.Action OnUp_Wheel;
    public static event System.Action OnDown_Wheel;

    // 左右ボタン
    public static event System.Action On_MouseLeft;
    public static event System.Action OnDown_MouseLeft;
    public static event System.Action OnUp_MouseLeft;
    public static event System.Action On_MouseRight;
    public static event System.Action OnDown_MouseRight;
    public static event System.Action OnUp_MouseRight;
    public static event System.Action On_MouseMiddle;
    public static event System.Action OnDown_MouseMiddle;
    public static event System.Action OnUp_MouseMiddle;

    // ダブルクリック
    int doubleClickCount = 0;
    float doubleClickInterval = 0;
    public static event System.Action On_MouseDouble;
    public static event System.Action OnDown_MouseDouble;
    public static event System.Action OnUp_MouseDouble;
    #endregion ==================================================


    protected sealed override void Update()
    {
        Key();
        KeyDown();
        KeyUp();
        Wheel();
        Click();
        DoubleClick();
    }


    void Wheel()
    {
        // 直近10フレームくらいの平均FPSを求めておく
        float averageFps = 0;
        recentFps.Add(1f / Time.deltaTime);
        if (recentFps.Count >= 10) recentFps.RemoveAt(0);
        recentFps.ForEach(a => averageFps += a);
        averageFps /= recentFps.Count;
        int stopFps = (int)(averageFps / (1f / stopWheelSecond));


        if (Input.mouseScrollDelta.y != 0)
        {
            if (!onWheel) OnStart_Wheel?.Invoke();
            On_Wheel?.Invoke();
            onWheel = true;
        }
        if (Input.mouseScrollDelta.y == 1) OnUp_Wheel?.Invoke();
        if (Input.mouseScrollDelta.y == -1) OnDown_Wheel?.Invoke();
        if (onWheel)
        {
            recentWheelDeltas.Add(Input.mouseScrollDelta.y);
            if (recentWheelDeltas.Count >= stopFps) recentWheelDeltas.RemoveAt(0);
            if (Input.mouseScrollDelta.y == 0)
            {
                if (recentWheelDeltas.Contains(1) || recentWheelDeltas.Contains(-1)) return;
                OnStop_Wheel?.Invoke();
                recentWheelDeltas.Clear();
                onWheel = false;
            }
        }
    }

    void Click()
    {
        if (Input.GetMouseButton(0)) On_MouseLeft?.Invoke();
        if (Input.GetMouseButtonDown(0)) OnDown_MouseLeft?.Invoke();
        if (Input.GetMouseButtonUp(0)) OnUp_MouseLeft?.Invoke();
        if (Input.GetMouseButton(1)) On_MouseRight?.Invoke();
        if (Input.GetMouseButtonDown(1)) OnDown_MouseRight?.Invoke();
        if (Input.GetMouseButtonUp(1)) OnUp_MouseRight?.Invoke();
        if (Input.GetMouseButton(2)) On_MouseMiddle?.Invoke();
        if (Input.GetMouseButtonDown(2)) OnDown_MouseMiddle?.Invoke();
        if (Input.GetMouseButtonUp(2)) OnUp_MouseMiddle?.Invoke();
    }


    void DoubleClick()
    {
        if (doubleClickCount == 0)
            if (Input.GetMouseButtonDown(0))
            {
                doubleClickCount++;
                return;
            }

        if (doubleClickCount == 1)
            if (Input.GetMouseButtonDown(0))
            {
                doubleClickCount++;
            }

        if (doubleClickCount < 1) return;
        if (doubleClickCount > 2)
        {
            doubleClickCount = 0;
            doubleClickInterval = 0;
            return;
        }

        doubleClickInterval += Time.deltaTime;

        if (doubleClickInterval > 1f)
        {
            doubleClickCount = 0;
            doubleClickInterval = 0;
            return;
        }

        if (doubleClickCount != 2) return;
        doubleClickCount = 0;
        doubleClickInterval = 0;
        On_MouseDouble?.Invoke();
    }

    void Key()
    {
        if (Input.GetKey(KeyCode.A)) On_A?.Invoke();
        if (Input.GetKey(KeyCode.B)) On_B?.Invoke();
        if (Input.GetKey(KeyCode.C)) On_C?.Invoke();
        if (Input.GetKey(KeyCode.D)) On_D?.Invoke();
        if (Input.GetKey(KeyCode.E)) On_E?.Invoke();
        if (Input.GetKey(KeyCode.F)) On_F?.Invoke();
        if (Input.GetKey(KeyCode.G)) On_G?.Invoke();
        if (Input.GetKey(KeyCode.H)) On_H?.Invoke();
        if (Input.GetKey(KeyCode.I)) On_I?.Invoke();
        if (Input.GetKey(KeyCode.J)) On_J?.Invoke();
        if (Input.GetKey(KeyCode.K)) On_K?.Invoke();
        if (Input.GetKey(KeyCode.L)) On_L?.Invoke();
        if (Input.GetKey(KeyCode.M)) On_M?.Invoke();
        if (Input.GetKey(KeyCode.N)) On_N?.Invoke();
        if (Input.GetKey(KeyCode.O)) On_O?.Invoke();
        if (Input.GetKey(KeyCode.P)) On_P?.Invoke();
        if (Input.GetKey(KeyCode.Q)) On_Q?.Invoke();
        if (Input.GetKey(KeyCode.R)) On_R?.Invoke();
        if (Input.GetKey(KeyCode.S)) On_S?.Invoke();
        if (Input.GetKey(KeyCode.T)) On_T?.Invoke();
        if (Input.GetKey(KeyCode.U)) On_U?.Invoke();
        if (Input.GetKey(KeyCode.V)) On_V?.Invoke();
        if (Input.GetKey(KeyCode.W)) On_W?.Invoke();
        if (Input.GetKey(KeyCode.X)) On_X?.Invoke();
        if (Input.GetKey(KeyCode.Y)) On_Y?.Invoke();
        if (Input.GetKey(KeyCode.Z)) On_Z?.Invoke();

        if (Input.GetKey(KeyCode.Alpha0)) On_0?.Invoke();
        if (Input.GetKey(KeyCode.Alpha1)) On_1?.Invoke();
        if (Input.GetKey(KeyCode.Alpha2)) On_2?.Invoke();
        if (Input.GetKey(KeyCode.Alpha3)) On_3?.Invoke();
        if (Input.GetKey(KeyCode.Alpha4)) On_4?.Invoke();
        if (Input.GetKey(KeyCode.Alpha5)) On_5?.Invoke();
        if (Input.GetKey(KeyCode.Alpha6)) On_6?.Invoke();
        if (Input.GetKey(KeyCode.Alpha7)) On_7?.Invoke();
        if (Input.GetKey(KeyCode.Alpha8)) On_8?.Invoke();
        if (Input.GetKey(KeyCode.Alpha9)) On_9?.Invoke();
        if (Input.GetKey(KeyCode.F1)) On_F1?.Invoke();
        if (Input.GetKey(KeyCode.F2)) On_F2?.Invoke();
        if (Input.GetKey(KeyCode.F3)) On_F3?.Invoke();
        if (Input.GetKey(KeyCode.F4)) On_F4?.Invoke();
        if (Input.GetKey(KeyCode.F5)) On_F5?.Invoke();
        if (Input.GetKey(KeyCode.F6)) On_F6?.Invoke();
        if (Input.GetKey(KeyCode.F7)) On_F7?.Invoke();
        if (Input.GetKey(KeyCode.F8)) On_F8?.Invoke();
        if (Input.GetKey(KeyCode.F9)) On_F9?.Invoke();
        if (Input.GetKey(KeyCode.F10)) On_F10?.Invoke();
        if (Input.GetKey(KeyCode.F11)) On_F11?.Invoke();
        if (Input.GetKey(KeyCode.F12)) On_F12?.Invoke();

        if (Input.GetKey(KeyCode.LeftBracket)) On_BracketL?.Invoke();
        if (Input.GetKey(KeyCode.RightBracket)) On_BracketR?.Invoke();
        if (Input.GetKey(KeyCode.Slash)) On_Slash?.Invoke();
        if (Input.GetKey(KeyCode.Backslash)) On_BackSlash?.Invoke();
        if (Input.GetKey(KeyCode.At)) On_At?.Invoke();
        if (Input.GetKey(KeyCode.Period)) On_Period?.Invoke();
        if (Input.GetKey(KeyCode.Comma)) On_Comma?.Invoke();
        if (Input.GetKey(KeyCode.Colon)) On_Colon?.Invoke();
        if (Input.GetKey(KeyCode.Semicolon)) On_Semicolon?.Invoke();

        if (Input.GetKey(KeyCode.Backspace)) On_BackSpace?.Invoke();
        if (Input.GetKey(KeyCode.Delete)) On_Delete?.Invoke();
        if (Input.GetKey(KeyCode.Tab)) On_Tab?.Invoke();
        if (Input.GetKey(KeyCode.Return)) On_Enter?.Invoke();
        if (Input.GetKey(KeyCode.Escape)) On_Esc?.Invoke();
        if (Input.GetKey(KeyCode.Space)) On_Space?.Invoke();
        if (Input.GetKey(KeyCode.UpArrow)) On_Up?.Invoke();
        if (Input.GetKey(KeyCode.DownArrow)) On_Down?.Invoke();
        if (Input.GetKey(KeyCode.RightArrow)) On_Right?.Invoke();
        if (Input.GetKey(KeyCode.LeftArrow)) On_Left?.Invoke();
        if (Input.GetKey(KeyCode.RightControl) ||
            Input.GetKey(KeyCode.LeftControl)) On_Ctrl?.Invoke();
        if (Input.GetKey(KeyCode.RightAlt) ||
            Input.GetKey(KeyCode.LeftAlt)) On_Alt?.Invoke();
    }

    void KeyDown()
    {
        if (Input.GetKeyDown(KeyCode.A)) OnDown_A?.Invoke();
        if (Input.GetKeyDown(KeyCode.B)) OnDown_B?.Invoke();
        if (Input.GetKeyDown(KeyCode.C)) OnDown_C?.Invoke();
        if (Input.GetKeyDown(KeyCode.D)) OnDown_D?.Invoke();
        if (Input.GetKeyDown(KeyCode.E)) OnDown_E?.Invoke();
        if (Input.GetKeyDown(KeyCode.F)) OnDown_F?.Invoke();
        if (Input.GetKeyDown(KeyCode.G)) OnDown_G?.Invoke();
        if (Input.GetKeyDown(KeyCode.H)) OnDown_H?.Invoke();
        if (Input.GetKeyDown(KeyCode.I)) OnDown_I?.Invoke();
        if (Input.GetKeyDown(KeyCode.J)) OnDown_J?.Invoke();
        if (Input.GetKeyDown(KeyCode.K)) OnDown_K?.Invoke();
        if (Input.GetKeyDown(KeyCode.L)) OnDown_L?.Invoke();
        if (Input.GetKeyDown(KeyCode.M)) OnDown_M?.Invoke();
        if (Input.GetKeyDown(KeyCode.N)) OnDown_N?.Invoke();
        if (Input.GetKeyDown(KeyCode.O)) OnDown_O?.Invoke();
        if (Input.GetKeyDown(KeyCode.P)) OnDown_P?.Invoke();
        if (Input.GetKeyDown(KeyCode.Q)) OnDown_Q?.Invoke();
        if (Input.GetKeyDown(KeyCode.R)) OnDown_R?.Invoke();
        if (Input.GetKeyDown(KeyCode.S)) OnDown_S?.Invoke();
        if (Input.GetKeyDown(KeyCode.T)) OnDown_T?.Invoke();
        if (Input.GetKeyDown(KeyCode.U)) OnDown_U?.Invoke();
        if (Input.GetKeyDown(KeyCode.V)) OnDown_V?.Invoke();
        if (Input.GetKeyDown(KeyCode.W)) OnDown_W?.Invoke();
        if (Input.GetKeyDown(KeyCode.X)) OnDown_X?.Invoke();
        if (Input.GetKeyDown(KeyCode.Y)) OnDown_Y?.Invoke();
        if (Input.GetKeyDown(KeyCode.Z)) OnDown_Z?.Invoke();

        if (Input.GetKeyDown(KeyCode.Alpha0)) OnDown_0?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha1)) OnDown_1?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha2)) OnDown_2?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha3)) OnDown_3?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha4)) OnDown_4?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha5)) OnDown_5?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha6)) OnDown_6?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha7)) OnDown_7?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha8)) OnDown_8?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha9)) OnDown_9?.Invoke();
        if (Input.GetKeyDown(KeyCode.F1)) OnDown_F1?.Invoke();
        if (Input.GetKeyDown(KeyCode.F2)) OnDown_F2?.Invoke();
        if (Input.GetKeyDown(KeyCode.F3)) OnDown_F3?.Invoke();
        if (Input.GetKeyDown(KeyCode.F4)) OnDown_F4?.Invoke();
        if (Input.GetKeyDown(KeyCode.F5)) OnDown_F5?.Invoke();
        if (Input.GetKeyDown(KeyCode.F6)) OnDown_F6?.Invoke();
        if (Input.GetKeyDown(KeyCode.F7)) OnDown_F7?.Invoke();
        if (Input.GetKeyDown(KeyCode.F8)) OnDown_F8?.Invoke();
        if (Input.GetKeyDown(KeyCode.F9)) OnDown_F9?.Invoke();
        if (Input.GetKeyDown(KeyCode.F10)) OnDown_F10?.Invoke();
        if (Input.GetKeyDown(KeyCode.F11)) OnDown_F11?.Invoke();
        if (Input.GetKeyDown(KeyCode.F12)) OnDown_F12?.Invoke();

        if (Input.GetKeyDown(KeyCode.LeftBracket)) OnDown_BracketL?.Invoke();
        if (Input.GetKeyDown(KeyCode.RightBracket)) OnDown_BracketR?.Invoke();
        if (Input.GetKeyDown(KeyCode.Slash)) OnDown_Slash?.Invoke();
        if (Input.GetKeyDown(KeyCode.Backslash)) OnDown_BackSlash?.Invoke();
        if (Input.GetKeyDown(KeyCode.At)) OnDown_At?.Invoke();
        if (Input.GetKeyDown(KeyCode.Period)) OnDown_Period?.Invoke();
        if (Input.GetKeyDown(KeyCode.Comma)) OnDown_Comma?.Invoke();
        if (Input.GetKeyDown(KeyCode.Colon)) OnDown_Colon?.Invoke();
        if (Input.GetKeyDown(KeyCode.Semicolon)) OnDown_Semicolon?.Invoke();

        if (Input.GetKeyDown(KeyCode.Backspace)) OnDown_BackSpace?.Invoke();
        if (Input.GetKeyDown(KeyCode.Delete)) OnDown_Delete?.Invoke();
        if (Input.GetKeyDown(KeyCode.Tab)) OnDown_Tab?.Invoke();
        if (Input.GetKeyDown(KeyCode.Return)) OnDown_Enter?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape)) OnDown_Esc?.Invoke();
        if (Input.GetKeyDown(KeyCode.Space)) OnDown_Space?.Invoke();
        if (Input.GetKeyDown(KeyCode.UpArrow)) OnDown_Up?.Invoke();
        if (Input.GetKeyDown(KeyCode.DownArrow)) OnDown_Down?.Invoke();
        if (Input.GetKeyDown(KeyCode.RightArrow)) OnDown_Right?.Invoke();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) OnDown_Left?.Invoke();
        if (Input.GetKeyDown(KeyCode.RightControl) ||
            Input.GetKeyDown(KeyCode.LeftControl)) OnDown_Ctrl?.Invoke();
        if (Input.GetKeyDown(KeyCode.RightAlt) ||
            Input.GetKeyDown(KeyCode.LeftAlt)) OnDown_Alt?.Invoke();
    }

    void KeyUp()
    {
        if (Input.GetKeyUp(KeyCode.A)) OnUp_A?.Invoke();
        if (Input.GetKeyUp(KeyCode.B)) OnUp_B?.Invoke();
        if (Input.GetKeyUp(KeyCode.C)) OnUp_C?.Invoke();
        if (Input.GetKeyUp(KeyCode.D)) OnUp_D?.Invoke();
        if (Input.GetKeyUp(KeyCode.E)) OnUp_E?.Invoke();
        if (Input.GetKeyUp(KeyCode.F)) OnUp_F?.Invoke();
        if (Input.GetKeyUp(KeyCode.G)) OnUp_G?.Invoke();
        if (Input.GetKeyUp(KeyCode.H)) OnUp_H?.Invoke();
        if (Input.GetKeyUp(KeyCode.I)) OnUp_I?.Invoke();
        if (Input.GetKeyUp(KeyCode.J)) OnUp_J?.Invoke();
        if (Input.GetKeyUp(KeyCode.K)) OnUp_K?.Invoke();
        if (Input.GetKeyUp(KeyCode.L)) OnUp_L?.Invoke();
        if (Input.GetKeyUp(KeyCode.M)) OnUp_M?.Invoke();
        if (Input.GetKeyUp(KeyCode.N)) OnUp_N?.Invoke();
        if (Input.GetKeyUp(KeyCode.O)) OnUp_O?.Invoke();
        if (Input.GetKeyUp(KeyCode.P)) OnUp_P?.Invoke();
        if (Input.GetKeyUp(KeyCode.Q)) OnUp_Q?.Invoke();
        if (Input.GetKeyUp(KeyCode.R)) OnUp_R?.Invoke();
        if (Input.GetKeyUp(KeyCode.S)) OnUp_S?.Invoke();
        if (Input.GetKeyUp(KeyCode.T)) OnUp_T?.Invoke();
        if (Input.GetKeyUp(KeyCode.U)) OnUp_U?.Invoke();
        if (Input.GetKeyUp(KeyCode.V)) OnUp_V?.Invoke();
        if (Input.GetKeyUp(KeyCode.W)) OnUp_W?.Invoke();
        if (Input.GetKeyUp(KeyCode.X)) OnUp_X?.Invoke();
        if (Input.GetKeyUp(KeyCode.Y)) OnUp_Y?.Invoke();
        if (Input.GetKeyUp(KeyCode.Z)) OnUp_Z?.Invoke();

        if (Input.GetKeyUp(KeyCode.Alpha0)) OnUp_0?.Invoke();
        if (Input.GetKeyUp(KeyCode.Alpha1)) OnUp_1?.Invoke();
        if (Input.GetKeyUp(KeyCode.Alpha2)) OnUp_2?.Invoke();
        if (Input.GetKeyUp(KeyCode.Alpha3)) OnUp_3?.Invoke();
        if (Input.GetKeyUp(KeyCode.Alpha4)) OnUp_4?.Invoke();
        if (Input.GetKeyUp(KeyCode.Alpha5)) OnUp_5?.Invoke();
        if (Input.GetKeyUp(KeyCode.Alpha6)) OnUp_6?.Invoke();
        if (Input.GetKeyUp(KeyCode.Alpha7)) OnUp_7?.Invoke();
        if (Input.GetKeyUp(KeyCode.Alpha8)) OnUp_8?.Invoke();
        if (Input.GetKeyUp(KeyCode.Alpha9)) OnUp_9?.Invoke();
        if (Input.GetKeyUp(KeyCode.F1)) OnUp_F1?.Invoke();
        if (Input.GetKeyUp(KeyCode.F2)) OnUp_F2?.Invoke();
        if (Input.GetKeyUp(KeyCode.F3)) OnUp_F3?.Invoke();
        if (Input.GetKeyUp(KeyCode.F4)) OnUp_F4?.Invoke();
        if (Input.GetKeyUp(KeyCode.F5)) OnUp_F5?.Invoke();
        if (Input.GetKeyUp(KeyCode.F6)) OnUp_F6?.Invoke();
        if (Input.GetKeyUp(KeyCode.F7)) OnUp_F7?.Invoke();
        if (Input.GetKeyUp(KeyCode.F8)) OnUp_F8?.Invoke();
        if (Input.GetKeyUp(KeyCode.F9)) OnUp_F9?.Invoke();
        if (Input.GetKeyUp(KeyCode.F10)) OnUp_F10?.Invoke();
        if (Input.GetKeyUp(KeyCode.F11)) OnUp_F11?.Invoke();
        if (Input.GetKeyUp(KeyCode.F12)) OnUp_F12?.Invoke();

        if (Input.GetKeyUp(KeyCode.LeftBracket)) OnUp_BracketL?.Invoke();
        if (Input.GetKeyUp(KeyCode.RightBracket)) OnUp_BracketR?.Invoke();
        if (Input.GetKeyUp(KeyCode.Slash)) OnUp_Slash?.Invoke();
        if (Input.GetKeyUp(KeyCode.Backslash)) OnUp_BackSlash?.Invoke();
        if (Input.GetKeyUp(KeyCode.At)) OnUp_At?.Invoke();
        if (Input.GetKeyUp(KeyCode.Period)) OnUp_Period?.Invoke();
        if (Input.GetKeyUp(KeyCode.Comma)) OnUp_Comma?.Invoke();
        if (Input.GetKeyUp(KeyCode.Colon)) OnUp_Colon?.Invoke();
        if (Input.GetKeyUp(KeyCode.Semicolon)) OnUp_Semicolon?.Invoke();

        if (Input.GetKeyUp(KeyCode.Backspace)) OnUp_BackSpace?.Invoke();
        if (Input.GetKeyUp(KeyCode.Delete)) OnUp_Delete?.Invoke();
        if (Input.GetKeyUp(KeyCode.Tab)) OnUp_Tab?.Invoke();
        if (Input.GetKeyUp(KeyCode.Return)) OnUp_Enter?.Invoke();
        if (Input.GetKeyUp(KeyCode.Escape)) OnUp_Esc?.Invoke();
        if (Input.GetKeyUp(KeyCode.Space)) OnUp_Space?.Invoke();
        if (Input.GetKeyUp(KeyCode.UpArrow)) OnUp_Up?.Invoke();
        if (Input.GetKeyUp(KeyCode.DownArrow)) OnUp_Down?.Invoke();
        if (Input.GetKeyUp(KeyCode.RightArrow)) OnUp_Right?.Invoke();
        if (Input.GetKeyUp(KeyCode.LeftArrow)) OnUp_Left?.Invoke();
        if (Input.GetKeyUp(KeyCode.RightControl) ||
            Input.GetKeyUp(KeyCode.LeftControl)) OnUp_Ctrl?.Invoke();
        if (Input.GetKeyUp(KeyCode.RightAlt) ||
            Input.GetKeyUp(KeyCode.LeftAlt)) OnUp_Alt?.Invoke();
    }
}
