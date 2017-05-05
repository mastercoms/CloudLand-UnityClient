using Org.Dragonet.Cloudland.Net.Protocol;

class PlayerInventoryWindow : GameWindow
{

    GUIElement inventoryElem = new GUIElement();
    GUIElement craftingInputElem = new GUIElement();
    GUIElement craftingOutputElem = new GUIElement();

    public override void Initialize(ServerWindowOpenMessage msg)
    {
        // elements
        elements = new Google.Protobuf.Collections.RepeatedField<Org.Dragonet.Cloudland.Net.Protocol.GUIElement>();

        inventoryElem.Type = GUIElementType.Inventory;
        inventoryElem.Value = new SerializedMetadata();
        UpdateItems();
        inventoryElem.X = 32;
        inventoryElem.Y = 32;
        inventoryElem.Width = 500 - 64;
        inventoryElem.Height = 400 - 64;
        elements.Add(inventoryElem);

        craftingInputElem.Type = GUIElementType.Inventory;
        craftingInputElem.Value = new SerializedMetadata();
        UpdateCraftingInputs();
        craftingInputElem.X = 500;
        craftingInputElem.Y = 80;
        craftingInputElem.Width = 110;
        craftingInputElem.Height = 110;
        elements.Add(craftingInputElem);

        craftingOutputElem.Type = GUIElementType.Inventory;
        craftingOutputElem.Value = new SerializedMetadata();
        UpdateCraftingOutput();
        craftingOutputElem.X = 650;
        craftingOutputElem.Y = 100;
        craftingOutputElem.Width = 64;
        craftingOutputElem.Height = 64;
        elements.Add(craftingOutputElem);

        rect = new UnityEngine.Rect(UnityEngine.Screen.width / 2 - 400, UnityEngine.Screen.height / 2 - 240, 800, 480);
        title = "Inventory";
    }

    public void UpdateItems()
    {
        SerializedItem[] items = ClientComponent.INSTANCE.GetComponent<PlayerInventory>().items;
        for (uint i = 0; i < items.Length; i++)
        {
            SerializedMetadata.Types.MetadataEntry ent = new SerializedMetadata.Types.MetadataEntry();
            ent.Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Meta;
            SerializedMetadata itemMeta = new SerializedMetadata();
            itemMeta.Entries[0] = new SerializedMetadata.Types.MetadataEntry();
            itemMeta.Entries[0].Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Int32;
            itemMeta.Entries[0].Int32Value = items[i].Id;
            itemMeta.Entries[1] = new SerializedMetadata.Types.MetadataEntry();
            itemMeta.Entries[1].Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Int32;
            itemMeta.Entries[1].Int32Value = (int)items[i].Count;
            itemMeta.Entries[2] = new SerializedMetadata.Types.MetadataEntry();
            itemMeta.Entries[2].Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Meta;
            itemMeta.Entries[2].MetaValue = items[i].BinaryMeta;
            ent.MetaValue = itemMeta;
            inventoryElem.Value.Entries[i] = ent;
        }
    }

    public void UpdateCraftingInputs()
    {
        SerializedItem[] cInItems = ClientComponent.INSTANCE.GetComponent<PlayerInventory>().craftingInput;
        for (uint i = 0; i < cInItems.Length; i++)
        {
            SerializedMetadata.Types.MetadataEntry ent = new SerializedMetadata.Types.MetadataEntry();
            ent.Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Meta;
            SerializedMetadata itemMeta = new SerializedMetadata();
            itemMeta.Entries[0] = new SerializedMetadata.Types.MetadataEntry();
            itemMeta.Entries[0].Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Int32;
            itemMeta.Entries[0].Int32Value = cInItems[i].Id;
            itemMeta.Entries[1] = new SerializedMetadata.Types.MetadataEntry();
            itemMeta.Entries[1].Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Int32;
            itemMeta.Entries[1].Int32Value = (int)cInItems[i].Count;
            itemMeta.Entries[2] = new SerializedMetadata.Types.MetadataEntry();
            itemMeta.Entries[2].Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Meta;
            itemMeta.Entries[2].MetaValue = cInItems[i].BinaryMeta;
            ent.MetaValue = itemMeta;
            craftingInputElem.Value.Entries[i] = ent;
        }
    }

    public void UpdateCraftingOutput()
    {
        SerializedItem[] cOutItems = ClientComponent.INSTANCE.GetComponent<PlayerInventory>().craftingOutput;
        for (uint i = 0; i < cOutItems.Length; i++)
        {
            SerializedMetadata.Types.MetadataEntry ent = new SerializedMetadata.Types.MetadataEntry();
            ent.Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Meta;
            SerializedMetadata itemMeta = new SerializedMetadata();
            itemMeta.Entries[0] = new SerializedMetadata.Types.MetadataEntry();
            itemMeta.Entries[0].Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Int32;
            itemMeta.Entries[0].Int32Value = cOutItems[i].Id;
            itemMeta.Entries[1] = new SerializedMetadata.Types.MetadataEntry();
            itemMeta.Entries[1].Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Int32;
            itemMeta.Entries[1].Int32Value = (int)cOutItems[i].Count;
            itemMeta.Entries[2] = new SerializedMetadata.Types.MetadataEntry();
            itemMeta.Entries[2].Type = SerializedMetadata.Types.MetadataEntry.Types.DataType.Meta;
            itemMeta.Entries[2].MetaValue = cOutItems[i].BinaryMeta;
            ent.MetaValue = itemMeta;
            craftingOutputElem.Value.Entries[i] = ent;
        }
    }
}
