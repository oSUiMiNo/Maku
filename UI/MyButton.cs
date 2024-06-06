using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class MyButton : MyUI
{
    // 一括セット用のスクリプトからセットする。
    [SerializeField]
    public ReactiveProperty<RuntimeAnimatorController> animatorController = new ReactiveProperty<RuntimeAnimatorController>();


    Animator animator;


    protected sealed override void Awake0()
    {
        animator = CheckAddComponent<Animator>(gameObject);
        animatorController.Subscribe(value => animator.runtimeAnimatorController = value);
    }


    protected sealed override void Start()
    {
        //DebugView.Log($"{presenter}");
        //Clicked.Subscribe(value => presenter.Clicked.Value = value);

        //On_Click.Subscribe(async _ => await presenter.Execute());
        //On_ClickMargin.Subscribe(async _ => await presenter.Desist());

        On_Enter.Subscribe(_ => PlayAnim(true));
        On_Exit.Subscribe(_ => PlayAnim(false));
    }



    private void PlayAnim( bool state )
    {
        if (animator == null) return;
        animator.SetBool("MouseOver", state);
    }
}
