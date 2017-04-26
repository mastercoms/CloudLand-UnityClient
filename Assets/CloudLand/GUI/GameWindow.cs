using UnityEngine;

using Google.Protobuf.Collections;
using Org.Dragonet.Cloudland.Net.Protocol;

public class GameWindow : MonoBehaviour {

    public GUIStyle blankStyle;

    protected uint windowId;

    protected Rect rect;
    protected string title;

    protected RepeatedField<Org.Dragonet.Cloudland.Net.Protocol.GUIElement> elements;

	public virtual void Initialize(ServerWindowOpenMessage msg)
    {
        windowId = msg.WindowId;
        rect = new Rect(200, 100, msg.Width, msg.Height);
        elements = msg.Items;
        title = msg.Title;
    }

    void OnGUI()
    {
        GUI.Window((int)(0x7f000000 | windowId), rect, func, title);
    }

    void func(int unityWindowId)
    {
        for(int i = 0; i < elements.Count; i++)
        {
            Org.Dragonet.Cloudland.Net.Protocol.GUIElement ele = elements[i];
            Rect eleRect = new Rect(ele.X, ele.Y, ele.Width, ele.Height);
            bool pressed = false;
            switch(ele.Type)
            {
                case GUIElementType.Text:
                    GUI.Label(eleRect, ele.Value.Entries[0].StringValue);
                    break;
                case GUIElementType.Button:
                    pressed = GUI.Button(eleRect, ele.Value.Entries[0].StringValue);
                    if (pressed) buttonPressed(i, 0, 0);
                    break;
                case GUIElementType.LineInput:
                    ele.Value.Entries[0].StringValue = GUI.TextField(eleRect, ele.Value.Entries[0].StringValue);
                    break;
                case GUIElementType.AreaInput:
                    ele.Value.Entries[0].StringValue = GUI.TextArea(eleRect, ele.Value.Entries[0].StringValue);
                    break;
                case GUIElementType.Inventory:
                    drawInventoryElement(i, eleRect, ele);
                    break;
                default:
                    Debug.Log("Can't recognise GUI element type");
                    break;
            }
        }

        if(GUI.Button(new Rect(rect.width - 16, 4, 16, 16), "X"))
        {
            close(true);
        }
    }

    private void drawInventoryElement(int index, Rect eleRect, Org.Dragonet.Cloudland.Net.Protocol.GUIElement element)
    {
        int itemsPerLine = (int)((eleRect.width - 24) / 40.0f);
        int line = 0;

        GUI.Box(eleRect, "");
        for(uint i = 0; i < element.Value.Entries.Count; i++)
        {
            int id = element.Value.Entries[i].MetaValue.Entries[0].Int32Value;
            int meta = element.Value.Entries[i].MetaValue.Entries[1].Int32Value;
            int count = element.Value.Entries[i].MetaValue.Entries[2].Int32Value;

            Rect slot = new Rect(eleRect.x + 12 + (i % itemsPerLine) * 40, eleRect.y + line * 40f + 12, 32, 32);
            Rect slotCount = new Rect(eleRect.x + 12 + (i % itemsPerLine) * 40 + 16, eleRect.y + line * 40f + 28, 24, 8);

            bool clicked = false;

            clicked |= GUI.Button(slot, Inventory.getItemTexture(id, meta), blankStyle);
            clicked |= GUI.Button(slotCount, count.ToString(), blankStyle);

            if (clicked)
            {
                buttonPressed(index, (int)i, 0);
            }

            if( (i+1) % itemsPerLine == 0 )
            {
                line++;
            }
        }
    }

    private void buttonPressed(int index, int param1, int param2)
    {
        Debug.Log("BUTTON PRESSED " + string.Format("{0} : {1}, {2}", index, param1, param2));
        ClientWindowInteractMessage interact = new ClientWindowInteractMessage();
        interact.WindowId = windowId;
        interact.ElementIndex = index;
        interact.Action = ClientWindowInteractMessage.Types.WindowAction.LeftClick;
        interact.Param1 = param1;
        interact.Param2 = param2;
        // TODO: x and y

        ClientComponent.INSTANCE.GetClient().sendMessage(interact);
    }

    public void close(bool fromClient)
    {
        if(!fromClient && windowId != 0)
        {
            ClientWindowCloseMessage msgClose = new ClientWindowCloseMessage();
            msgClose.WindowId = windowId;
            ClientComponent.INSTANCE.GetClient().sendMessage(msgClose);
        }
        GameObject.Destroy(gameObject);
    }

    public void updateElement(int index, Org.Dragonet.Cloudland.Net.Protocol.GUIElement element)
    {
        elements[index] = element;
    }
}
