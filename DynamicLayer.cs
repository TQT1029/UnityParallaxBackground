using UnityEngine;

public class DyanmicLayer : MonoBehaviour
{
    [SerializeField] private ParallaxBackground parallaxBackground;

    [SerializeField] private Vector2 scrollSpeed; // đơn vị: UV/giây
    [SerializeField] private Renderer rend;
    [SerializeField] private Vector2 offset;
    private int indexLayer;

    [Range(0f, 2f), SerializeField] private float maxDampingY = 0.5f;
    [SerializeField] private float smooth = 1f;
    [SerializeField] private float dampingFactor;

    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 previousPos;

    [SerializeField] private Rigidbody2D ballRB;

    private void Awake()
    {
        startPos = previousPos = transform.localPosition;


        maxDampingY = Mathf.Abs(transform.localPosition.y);
    }
    private void Start()
    {

        parallaxBackground = transform.parent.gameObject.GetComponent<ParallaxBackground>();
        try
        {
            ballRB = GameObject.Find("Ball").GetComponent<Rigidbody2D>();
        }
        catch { }

        if (parallaxBackground != null)
        {
            indexLayer = transform.name[^1] - '0';

            scrollSpeed = parallaxBackground.layersVector[indexLayer];
            dampingFactor = scrollSpeed.y;

            rend = parallaxBackground.layersRenderer[indexLayer];
        }
        else
        {
            scrollSpeed = new Vector2(0.1f, 0f);
            rend = GetComponent<Renderer>();
            // Đảm bảo mỗi BG có bản material riêng nếu bạn chỉnh offset khác nhau
            rend.material = Instantiate(rend.material);
        }
    }

    private void LateUpdate()
    {
        previousPos = transform.localPosition;
    }
    private void Update()
    {
        if (ballRB != null)
        {
            offset.x += (ballRB.velocity.x / 10f) * scrollSpeed.x * Time.deltaTime;

            // Hệ số mượt
            float smoothFactor = 1f - Mathf.Exp(-smooth * Time.deltaTime);

            // Khi đứng yên -> target = previousPos
            // Khi đi lên  (velocity.y > 0) -> nền đi xuống (ngược chiều) => đảo dấu
            // Khi đi xuống (velocity.y < 0) -> nền đi lên
            float yOffset = -ballRB.velocity.y / 10 * dampingFactor;
            
            Vector3 targetPos = previousPos + new Vector3(0f, yOffset, 0f);
            // Giới hạn biên độ để không trôi quá xa
            if (-maxDampingY <= targetPos.y && targetPos.y <= maxDampingY && maxDampingY != 0)
                // Lerp về vị trí target
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, smoothFactor);
            else if (maxDampingY != 0)
            {
                targetPos = previousPos + new Vector3(0f, -yOffset, 0f);
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, smoothFactor);
            }
        }
        else
        {
            // Nếu không có rigidbody nhân vật thì cuộn nền đều theo thời gian
            offset += scrollSpeed * Time.deltaTime;
        }

        rend.material.mainTextureOffset = offset;
    }

}
