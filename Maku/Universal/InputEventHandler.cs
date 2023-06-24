using UnityEngine;

// キー打った時のイベント
public class InputEventHandler : SingletonCompo<InputEventHandler>
{
    public static event System.Action OnKeyDown_A;
    public static event System.Action OnKeyDown_B;
    public static event System.Action OnKeyDown_C;
    public static event System.Action OnKeyDown_D;
    public static event System.Action OnKeyDown_E;
    public static event System.Action OnKeyDown_F;
    public static event System.Action OnKeyDown_G;
    public static event System.Action OnKeyDown_H;
    public static event System.Action OnKeyDown_I;
    public static event System.Action OnKeyDown_J;
    public static event System.Action OnKeyDown_K;
    public static event System.Action OnKeyDown_L;
    public static event System.Action OnKeyDown_M;
    public static event System.Action OnKeyDown_N;
    public static event System.Action OnKeyDown_O;
    public static event System.Action OnKeyDown_P;
    public static event System.Action OnKeyDown_Q;
    public static event System.Action OnKeyDown_R;
    public static event System.Action OnKeyDown_S;
    public static event System.Action OnKeyDown_T;
    public static event System.Action OnKeyDown_U;
    public static event System.Action OnKeyDown_V;
    public static event System.Action OnKeyDown_W;
    public static event System.Action OnKeyDown_X;
    public static event System.Action OnKeyDown_Y;
    public static event System.Action OnKeyDown_Z;

    protected sealed override void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) OnKeyDown_A?.Invoke();
        if (Input.GetKeyDown(KeyCode.B)) OnKeyDown_B?.Invoke();
        if (Input.GetKeyDown(KeyCode.C)) OnKeyDown_C?.Invoke();
        if (Input.GetKeyDown(KeyCode.D)) OnKeyDown_D?.Invoke();
        if (Input.GetKeyDown(KeyCode.E)) OnKeyDown_E?.Invoke();
        if (Input.GetKeyDown(KeyCode.F)) OnKeyDown_F?.Invoke();
        if (Input.GetKeyDown(KeyCode.G)) OnKeyDown_G?.Invoke();
        if (Input.GetKeyDown(KeyCode.H)) OnKeyDown_H?.Invoke();
        if (Input.GetKeyDown(KeyCode.I)) OnKeyDown_I?.Invoke();
        if (Input.GetKeyDown(KeyCode.J)) OnKeyDown_J?.Invoke();
        if (Input.GetKeyDown(KeyCode.K)) OnKeyDown_K?.Invoke();
        if (Input.GetKeyDown(KeyCode.L)) OnKeyDown_L?.Invoke();
        if (Input.GetKeyDown(KeyCode.M)) OnKeyDown_M?.Invoke();
        if (Input.GetKeyDown(KeyCode.N)) OnKeyDown_N?.Invoke();
        if (Input.GetKeyDown(KeyCode.O)) OnKeyDown_O?.Invoke();
        if (Input.GetKeyDown(KeyCode.P)) OnKeyDown_P?.Invoke();
        if (Input.GetKeyDown(KeyCode.Q)) OnKeyDown_Q?.Invoke();
        if (Input.GetKeyDown(KeyCode.R)) OnKeyDown_R?.Invoke();
        if (Input.GetKeyDown(KeyCode.S)) OnKeyDown_S?.Invoke();
        if (Input.GetKeyDown(KeyCode.T)) OnKeyDown_T?.Invoke();
        if (Input.GetKeyDown(KeyCode.U)) OnKeyDown_U?.Invoke();
        if (Input.GetKeyDown(KeyCode.V)) OnKeyDown_V?.Invoke();
        if (Input.GetKeyDown(KeyCode.W)) OnKeyDown_W?.Invoke();
        if (Input.GetKeyDown(KeyCode.X)) OnKeyDown_X?.Invoke();
        if (Input.GetKeyDown(KeyCode.Y)) OnKeyDown_Y?.Invoke();
        if (Input.GetKeyDown(KeyCode.Z)) OnKeyDown_Z?.Invoke();
    }
}
