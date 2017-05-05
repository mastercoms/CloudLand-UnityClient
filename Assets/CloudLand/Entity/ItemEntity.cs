using System;
using Org.Dragonet.Cloudland.Net.Protocol;
using Google.Protobuf.Collections;
using UnityEngine;

public class ItemEntity : Entity {

    public Material itemMaterial;

    private const float speed = 16;

    private bool pickingUp = false;
    private float pickupSpan = 1f;

    // Update is called once per frame
    void Update()
    {
        checkUpdateMeta();

        transform.FindChild("Cube").Rotate(new Vector3(0f, Time.deltaTime * speed, 0f));

        if(pickingUp)
        {
            pickupSpan += Time.deltaTime;
            if(pickupSpan >= 1f)
            {
                pickupSpan = 0f;
                ClientPickUpItemMessage pickup = new ClientPickUpItemMessage();
                pickup.EntityId = (ulong)(entityId & 0xFFFFFFFFFFFFFFFFL);
                ClientComponent.INSTANCE.GetClient().sendMessage(pickup);
                // Debug.Log("PICKING UP <" + pickup.EntityId + "> " + Time.deltaTime);
            }
        }
    }

    void OnTriggerExit()
    {
        pickingUp = false;
        pickupSpan = 1f;
    }

    void OnTriggerEnter()
    {
        pickupSpan = 1f;
        pickingUp = true;
    }

    protected override void ApplyMeta(MapField<uint, SerializedMetadata.Types.MetadataEntry> meta)
    {
        if(meta.ContainsKey(0xBA))
        {
            // Debug.Log("GOT ITEM META");
            SerializedMetadata.Types.MetadataEntry ent = null;
            meta.TryGetValue(0xBA, out ent);
            if (ent == null) return;
            Debug.Log("GOT ITEM META -- not null");
            MapField<uint, SerializedMetadata.Types.MetadataEntry> item = ent.MetaValue.Entries;
            int item_id = item[0].Int32Value;
            int item_meta = item[1].Int32Value;
            // int item_count = item[2].Int32Value;
            MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
            Material mat = new Material(itemMaterial);
            mat.mainTexture = Inventory.getItemTexture(item_id);
            renderer.material = mat;
        }
    }
}
