using UnityEngine;
using System.Collections;
using System;

public class NativeSample : MonoBehaviour,
    ITakePhotoReceiverListener, IPayReceiverListener, IAudioControlListener, ISpeechListener
{
    //-------------------------------------------------------------------------
    public void audioChanged(string chang)
    {
        Debug.Log("audioChanged");
    }

    //-------------------------------------------------------------------------
    public void getPicFail(string fail)
    {
        //加载图片失败
        Debug.Log("加载图片失败");

    }

    //-------------------------------------------------------------------------
    public void getPicSuccess(string getpic_result)
    {
        //加载图片成功
        Debug.Log("加载图片成功");

    }

    //-------------------------------------------------------------------------
    public void PayResult(_ePayOptionType option_type, bool is_success, object result)
    {
        Debug.Log("PayResult::" + result);
    }

    //-------------------------------------------------------------------------
    public void speechResult(_eSpeechResult result_code, string most_possibleresult)
    {
        Debug.Log("speechResult::result_code:: " + (_eSpeechResult)result_code + "   most_possibleresult::" + most_possibleresult);
    }

    //-------------------------------------------------------------------------
    void Start()
    {
        //初始化各个Listener
        _initNativeMsgReceiverListener();
    }

    //-------------------------------------------------------------------------
    void Update()
    {

    }

    //-------------------------------------------------------------------------
    void _initNativeMsgReceiverListener()
    {
        var native_receiver = NativeReceiver.instance();
        native_receiver.TakePhotoReceiverListener = this;
        native_receiver.AudioControlListener = this;
        var speech_receiver = SpeechReceiver.instance();
        speech_receiver.SpeechListener = this;
        var pay_receiver = PayReceiver.instance();
        pay_receiver.PayReceiverListener = this;
    }
}
