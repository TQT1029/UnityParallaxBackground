using UnityEngine;

/// <summary>
/// Xử lý chuyển động parallax cho từng layer riêng.
/// Lấy dữ liệu tốc độ và Renderer từ ParallaxBackground.
/// </summary>
public class DyanmicLayer : MonoBehaviour
{
    [SerializeField] private ParallaxBackground parallaxBackground;
    [SerializeField] private Vector2 scrollSpeed; // UV/giây

    [SerializeField] private Renderer rend;
    [SerializeField] private Vector2 offset;

    private int indexLayer;

    [Range(0f, 2f), SerializeField] private float maxDampingY = 0.5f; // Giới hạn lệch Y tối đa
    [SerializeField] private float smooth = 5f;   // Độ mượt hồi vị trí
    [SerializeField] private float dampingFactor; // Hệ số giảm/tăng chuyển động Y

    [SerializeField] private Vector3 previousPos; // Lưu vị trí trước để hồi lại khi đứng yên

    [SerializeField] private Rigidbody2D ballRB;  // Nhân vật điều khiển

    private void Awake()
    {
        previousPos = transform.localPosition;

        // Mặc định giới hạn Y theo độ cao hiện tại của layer
        maxDampingY = Mathf.Abs(transform.localPosition.y);
    }

    private void Start()
    {
        // Lấy tham chiếu tới ParallaxBackground
        parallaxBackground = transform.parent.gameObject.GetComponent<ParallaxBackground>();

        // Lấy Ball trong scene (nếu có)
        try
        {
            ballRB = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        }
        catch { }

        // Nếu lấy được dữ liệu từ ParallaxBackground
        if (parallaxBackground != null)
        {
            // Xác định index layer từ tên (Layer-0, Layer-1,...)
            indexLayer = transform.name[^1] - '0';

            scrollSpeed = parallaxBackground.layersVector[indexLayer];
            dampingFactor = scrollSpeed.y;
            rend = parallaxBackground.layersRenderer[indexLayer];
        }
        else
        {
            // Không có PB → đặt giá trị mặc định
            scrollSpeed = new Vector2(0.1f, 0f);
            rend = GetComponent<Renderer>();

            // Clone material
            rend.material = Instantiate(rend.material);
        }
    }

    private void LateUpdate()
    {
        // Cập nhật vị trí trước đó cho khung hình sau
        previousPos = transform.localPosition;
    }

    private void Update()
    {
        if (ballRB != null)
        {
            // Cuộn X theo vận tốc nhân vật
            offset.x += (ballRB.velocity.x / 10f) * scrollSpeed.x * Time.deltaTime;

            // Độ mượt nội suy
            float smoothFactor = 1f - Mathf.Exp(-smooth * Time.deltaTime);

            // Tính offset Y ngược chiều chuyển động
            float yOffset = -ballRB.velocity.y * dampingFactor * Time.deltaTime;

            // Giới hạn biên độ Y
            yOffset = Mathf.Clamp(yOffset, -maxDampingY, maxDampingY);

            // Tính vị trí mục tiêu
            Vector3 targetPos = previousPos + new Vector3(0f, yOffset, 0f);

            // Nếu trong phạm vi cho phép → Lerp tới
            if (-maxDampingY <= targetPos.y && targetPos.y <= maxDampingY)
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, smoothFactor);
        }
        else
        {
            // Không có nhân vật → cuộn đều
            offset += scrollSpeed * Time.deltaTime;
        }

        // Áp offset UV cho material
        rend.material.mainTextureOffset = offset;
    }
}
