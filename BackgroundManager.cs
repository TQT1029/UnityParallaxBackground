using UnityEngine;

/// <summary>
/// Quản lý các layer con của background.
/// Đảm bảo đặt tên các layer theo dạng "Layer-0", "Layer-1", ...
/// </summary>
[ExecuteInEditMode] // Chạy cả trong Edit Mode để set tên layer tự động
public class BackgroundManager : MonoBehaviour
{
    // Mảng lưu các layer (GameObject con)
    [SerializeField] public GameObject[] layers { get; private set; }

    // Số lượng layer
    [SerializeField] public int layersCounting { get; private set; }

    private void Awake()
    {
        // Nếu chưa khởi tạo hoặc bị rỗng
        if (layers == null || layersCounting == 0)
        {
            layersCounting = transform.childCount; // Đếm số con
            layers = new GameObject[layersCounting];

            for (int i = 0; i < layersCounting; i++)
            {
                // Lấy từng GameObject con
                layers[i] = transform.GetChild(i).gameObject;

                // Đặt lại tên nếu chưa đúng chuẩn
                if (layers[i].name != $"Layer-{i}")
                    layers[i].name = $"Layer-{i}";
            }
        }
    }
}
