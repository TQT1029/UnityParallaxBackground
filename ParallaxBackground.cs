using UnityEngine;

/// <summary>
/// Xử lý logic parallax cho các layer background.
/// Phải đi kèm BackgroundManager và nằm trong cùng GameObject.
/// GameObject chứa script này (backgroundObject) nên nằm trong Camera.
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("Get Variable Data")]
    [SerializeField] private BackgroundManager backgroundManager;

    // Mảng layer và số lượng
    private GameObject[] layers;
    private int layersCounting;

    [Space(10)]
    // Có thể bổ sung biến liên quan tới Ball nếu cần:
    // [SerializeField] private Rigidbody2D ballRB;
    // [SerializeField] private float ballVecX;
    // [SerializeField] private int ballDir;

    [Header("Layers Config")]
    [SerializeField] private float[] layersSpeeds; // tốc độ riêng từng layer
    public Renderer[] layersRenderer { get; private set; } // Renderer của từng layer
    public Vector2[] layersVector { get; private set; }    // Vector tốc độ cuộn UV của từng layer
    public Vector2[] layersOffset { get; private set; }    // Offset UV hiện tại của từng layer

    private void Start()
    {
        // Lấy dữ liệu từ BackgroundManager
        backgroundManager = GetComponent<BackgroundManager>();
        layers = backgroundManager.layers;
        layersCounting = backgroundManager.layersCounting;

        // Khởi tạo Renderer và thêm DynamicLayer nếu chưa có
        if (layersRenderer == null || layersRenderer.Length != layersCounting)
        {
            layersRenderer = new Renderer[layersCounting];

            for (int i = 0; i < layersCounting; i++)
            {
                layersRenderer[i] = layers[i].GetComponent<Renderer>();

                // Clone material để chỉnh offset riêng cho từng layer
                layersRenderer[i].material = Instantiate(layersRenderer[i].material);

                // Thêm DyanmicLayer nếu chưa có
                if (layersRenderer[i].GetComponent<DyanmicLayer>() == null)
                    layers[i].AddComponent<DyanmicLayer>();
            }
        }

        // Khởi tạo tốc độ & vector nếu chưa có hoặc sai kích thước
        if (layersSpeeds == null || layersSpeeds.Length != layersCounting ||
            layersVector == null || layersVector.Length != layersCounting ||
            layersOffset == null || layersOffset.Length != layersCounting)
        {
            layersSpeeds = new float[layersCounting];
            layersVector = new Vector2[layersCounting];
            layersOffset = new Vector2[layersCounting];

            for (int i = 0; i < backgroundManager.layersCounting; i++)
            {
                // Tính tốc độ từ 0.05 → 0.15 dựa trên thứ tự layer (layer xa chậm hơn)
                layersSpeeds[i] = Mathf.Lerp(0.05f, 0.15f,
                    layersCounting <= 1 ? 0f : (float)i / (layersCounting - 1));

                // Vector tốc độ X và Y bằng nhau (có thể tách riêng nếu cần)
                layersVector[i] = new Vector2(layersSpeeds[i], layersSpeeds[i]);
            }
        }
    }
}
