using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Ux_TonkersTableTopiaCell : MonoBehaviour
{
    [HideInInspector] public int rowNumberWhereThePartyIs = -1;
    [HideInInspector] public int columnNumberPrimeRib = -1;
    [HideInInspector] public int howManyRowsAreHoggingThisSeat = 1;
    [HideInInspector] public int howManyColumnsAreSneakingIn = 1;
    [HideInInspector] public bool isMashedLikePotatoes = false;
    [HideInInspector] public Ux_TonkersTableTopiaCell mashedIntoWho = null;

    public Sprite backgroundPictureBecausePlainIsLame = null;
    public Color backgroundColorLikeASunset = Color.white;
}