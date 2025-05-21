using System.Collections;
using UnityEngine;

public static class UIAnimations
{
    public static IEnumerator PlayUIAnim(string name, GameObject menu, bool Both = false, float delay = 0)
    {
        yield return new WaitForSecondsRealtime(delay);
        var anim = menu.GetComponent<Animator>();

        if (anim.GetBool(name)) anim.SetBool(name, false);
        else
        {
            menu.SetActive(true);
            anim.SetBool(name, true);
        }

        if (Both)
        {
            yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

            if (!anim.GetBool(name)) menu.SetActive(false);
        }
        else yield return new WaitForSecondsRealtime(1);
    }
}
