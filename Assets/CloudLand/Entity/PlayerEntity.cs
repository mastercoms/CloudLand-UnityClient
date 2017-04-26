using Org.Dragonet.Cloudland.Net.Protocol;
using Google.Protobuf.Collections;
using UnityEngine;

public class PlayerEntity : HeadEntity
{
    protected override void ApplyMeta(MapField<uint, SerializedMetadata.Types.MetadataEntry> meta)
    {
        if (meta.ContainsKey(0))
        {
            TextMesh txt = transform.gameObject.GetComponent<TextMesh>();
            if (txt == null)
            {
                txt = (TextMesh)transform.gameObject.AddComponent(typeof(TextMesh));
            }
            txt.text = "USERNAME=" + meta[0].StringValue;
        }
    }
}
