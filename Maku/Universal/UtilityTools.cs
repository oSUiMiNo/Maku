using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;



public class Roulet
{
    /// <summary>
    /// �w�肵���񐔕���1�̊m���œ����肪�o�郋�[���b�g
    /// </summary>
    /// <param name="times">�����1��</param>
    /// <returns></returns>
    public static bool BoolRoulet(int times)
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond);
        int randomNumber = UnityEngine.Random.Range(0, times); //min�͊܂ނ�max�͊܂܂Ȃ��炵���B

        if (randomNumber != 0) return false;
        return true;
    }
}




public class TimesCounter
{
    int countTime;
    public int Counter { get; private set; }
    public int TotalCounter { get; private set; }

    public TimesCounter(int countTime)
    {
        this.countTime = countTime;
        Counter = 0;
        TotalCounter = 0;
    }

    /// <summary>
    /// 1��J�E���g����B
    /// </summary>
    public void Count()
    {
        Counter++;
        TotalCounter++;

        if (Counter % countTime == 0)
            Counter = 0;
    }

    /// <summary>
    /// �J�E���g�����Z�b�g����B
    /// </summary>
    public void ResetCount()
    {
        Counter = 0;
    }

    /// <summary>
    /// �g�[�^���̃J�E���g�����Z�b�g����B
    /// </summary>
    public void ResetTotalCount()
    {
        Counter = 0;
    }

    /// <summary>
    /// �J�E���g�񐔂𖞂����Ă���ꍇ�͐^
    /// </summary>
    /// <returns></returns>
    public bool Hit()
    {
        if (Counter != 0) return false;
        return true;
    }

    /// <summary>
    /// �ŏ���1��ڈȊO�ŁA�J�E���g�񐔂𖞂����Ă���ꍇ�͐^
    /// </summary>
    /// <returns></returns>
    public bool HitExeptFirst()
    {
        if (TotalCounter == 0) return false;
        if (Counter != 0) return false;
        return true;
    }

    /// <summary>
    /// �J�E���g�����w�肵�������������Ă��邩�ǂ�����Ԃ��B
    /// </summary>
    public bool CountEqualOrOver(int a)
    {
        if (Counter < a) return false;
        return true;
    }

    /// <summary>
    /// �g�[�^���̃J�E���g�����w�肵�������������Ă��邩�ǂ�����Ԃ��B
    /// </summary>
    public bool TotalCountEqualOrOver(int a)
    {
        if (TotalCounter < a) return false;
        return true;
    }
}





public class TimeHandler : SingletonCompo<TimeHandler>
{
    public double GlobalDetailSecond { get; private set; } = 0;

    public class Clock : MyExtention
    {
        public GameObject Display { get; set; }
        public TextMeshProUGUI DisplayTex { get; set; }
        public bool IsVisuzlized
        {
            get
            {
                if (!DisplayCraated) return false;
                return Display.activeSelf;
            }
        }
        public string ClockName { get; private set; }
        public float SpeedMultiplier { get; private set; }
        public double DetailedSecond { get; private set; }
        public int Second { get; private set; }
        public int Minute { get; private set; }
        public bool DisplayCraated { get { return Display != null; } }

        public Clock(string clockName, float speedMultiplier, bool visualize)
        {
            this.ClockName = clockName;
            this.SpeedMultiplier = speedMultiplier;

            if (visualize) CreateDisplay();
        }

        public void CountTime(double deltaTime)
        {
            DetailedSecond = deltaTime * SpeedMultiplier;
            Second = (int)DetailedSecond;
            Minute = (int)(Second / 60);

            UpdateDisplay(Second.ToString());
        }

        private void CreateDisplay()
        {
            if (DisplayCraated) return;
            Display = CreateChild($"{ClockName}Display", GameObject.Find("ClockCanvas"), new List<Type> { typeof(TextMeshProUGUI) });
            Debug.Log($"���[�[�[�[�[�[�[�[�[�[1�@�@{Display}");
            DisplayTex = CheckComponent<TextMeshProUGUI>(Display);

            Debug.Log($"���[�[�[�[�[�[�[�[�[�[2�@�@{DisplayTex}");
        }

        private void UpdateDisplay(string content)
        {
            if (!IsVisuzlized) return;
            DisplayTex.text = content;
        }

        public void ShowDisPlay()
        {
            if (IsVisuzlized) return;
            Display.SetActive(true);
        }

        public void HideDisPlay()
        {
            if (!IsVisuzlized) return;
            Display.SetActive(false);
        }
    }

    public static Dictionary<string, Clock> Clocks = new Dictionary<string, Clock>();


    protected override void Update()
    {
        CountAllClocksTime();
    }

    void CountAllClocksTime()
    {
        GlobalDetailSecond += Time.deltaTime;
        if (Clocks.Count() == 0) return;
        foreach (var a in Clocks)
        {
            Clock clock = a.Value;
            clock.CountTime(GlobalDetailSecond);
        }
    }

    public static Clock CreateClock(string clockName, float speedMultiplier, bool visualize)
    {
        Clock clock = new Clock(clockName, speedMultiplier, visualize);
        Clocks.Add(clockName, clock);
        return clock;
    }
}
