using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string name;            // ���ϴ� ��� �̸�
    [TextArea(3, 5)]
    public string sentence;        // ��ȭ ���� (�ν����Ϳ��� �а� ����)

    public bool isLeftSpeaker;     // true�� ���� ����, false�� ������ ����
    public Sprite characterSprite; // (����) ��Ȳ�� ���� ǥ���� �ٲ� �� ���
}