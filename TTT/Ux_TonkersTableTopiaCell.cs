using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Ux_TonkersTableTopiaCell : MonoBehaviour
{
    public Color backgroundColorLikeASunset = Color.white;
    public Sprite backgroundPictureBecausePlainIsLame = null;
    [HideInInspector] public int columnNumberPrimeRib = -1;
    [HideInInspector] public int howManyColumnsAreSneakingIn = 1;
    [HideInInspector] public int howManyRowsAreHoggingThisSeat = 1;
    [HideInInspector] public bool isMashedLikePotatoes = false;
    [HideInInspector] public Ux_TonkersTableTopiaCell mashedIntoWho = null;
    [HideInInspector] public int rowNumberWhereThePartyIs = -1;
}