using UnityEngine;

using Org.Dragonet.Cloudland.Net.Protocol;

public abstract class Entity : MonoBehaviour {

    public ulong entityId;

    public SerializedMetadata meta;

    public bool updateMeta = true;

    protected void checkUpdateMeta()
    {
        if (updateMeta)
        {
            updateMeta = false;
            ApplyMeta(meta.Entries);
        }
    }

    protected abstract void ApplyMeta(Google.Protobuf.Collections.MapField<uint, SerializedMetadata.Types.MetadataEntry> meta);
}
