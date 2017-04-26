using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScreen : MonoBehaviour {

    public static string addr;
    public static int rd;

    private string ip = "127.0.0.1";

    private float renderDistance = 12.0f;

	void OnGUI()
    {
        GUILayout.Label("======== CONNECT TO THE SERVER ========");
        GUILayout.Label("LITTEN-ENDIAN? : " + System.BitConverter.IsLittleEndian);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Render distance: ");
        renderDistance = GUILayout.HorizontalSlider(renderDistance, 6.0f, 16.0f);
        GUILayout.Label(((int)renderDistance).ToString());
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Address: ");
        ip = GUILayout.TextField(ip);
        GUILayout.EndHorizontal();
        if(GUILayout.Button("<=> Connect"))
        {
            int ird = (int)renderDistance;
            ird *= ird;
            Chunk.renderDistanceSquared = ird;
            rd = (int)renderDistance;
            addr = System.Net.Dns.GetHostAddresses(ip)[0].ToString();
            Debug.Log(addr);
            SceneManager.LoadScene("scene");
        }
    }
}
