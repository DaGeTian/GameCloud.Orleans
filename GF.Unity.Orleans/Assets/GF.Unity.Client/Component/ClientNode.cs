using System;
using System.Collections.Generic;
using System.Text;
using GF.Unity.Common;

public class ClientNode<TDef> : Component<TDef> where TDef : DefNode, new()
{
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    public override void init()
    {
        EbLog.Note("ClientNode.init()");

        var settings = EcEngine.Instance.Settings;
    }

    //-------------------------------------------------------------------------
    public override void release()
    {
        EbLog.Note("ClientNode.release()");
    }

    //-------------------------------------------------------------------------
    public override void update(float elapsed_tm)
    {
    }

    //-------------------------------------------------------------------------
    public override void handleEvent(object sender, EntityEvent e)
    {
    }
}
