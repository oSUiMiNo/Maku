using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using MyUtil;

public class MyButton : MyUI
{
    // 一括セット用のスクリプトからセットする。
    [SerializeField]
    public ReactiveProperty<RuntimeAnimatorController> animatorController = new ReactiveProperty<RuntimeAnimatorController>();


    Animator animator;


    protected sealed override void Awake0()
    {
        //animator = CheckAddComponent<Animator>(gameObject);
        animator = gameObject.CheckAddCompo<Animator>();
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
        if (animatorController.Value == null) return;
        animator.SetBool("MouseOver", state);
    }
}
