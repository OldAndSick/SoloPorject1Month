using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string name;            // 말하는 사람 이름
    [TextArea(3, 5)]
    public string sentence;        // 대화 내용 (인스펙터에서 넓게 보임)

    public bool isLeftSpeaker;     // true면 왼쪽 강조, false면 오른쪽 강조
    public Sprite characterSprite; // (선택) 상황에 따라 표정을 바꿀 때 사용
}