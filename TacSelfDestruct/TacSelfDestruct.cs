using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TacSelfDestruct : PartModule
{
    List<Part> allParts;

    [KSPEvent(guiActive = true, guiName = "Self Destruct!", active = true)]
    public void ExplodeAllEvent()
    {
        allParts = vessel.parts;
        StartCoroutine(DoSelfDestruct());
    }

    [KSPEvent(guiActive = true, guiName = "Explode!", active = true)]
    public void ExplodeEvent()
    {
        part.explode();
    }

    [KSPAction("Self Destruct!")]
    public void ExplodeAllAction(KSPActionParam param)
    {
        allParts = vessel.parts;
        StartCoroutine(DoSelfDestruct());
    }

    [KSPAction("Explode!")]
    public void ExplodeAction(KSPActionParam param)
    {
        part.explode();
    }

    private IEnumerator<WaitForSeconds> DoSelfDestruct()
    {
        while (vessel.parts.Count > 0)
        {
            // We do not want to blow up the root part nor the self destruct part until last.
            Part part = vessel.parts.Find(p => p != vessel.rootPart && p != this.part && p.children.Count == 0);
            if (part != null)
            {
                part.explode();
            }
            else
            {
                // Explode the rest of the parts
                vessel.parts.ForEach(p => p.explode());
            }
            yield return new WaitForSeconds(0.02f);
        }
    }
}
