using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    // Oculus Touch Controller
    public OVRInput.Controller controlL;  // 左手のコントローラー
    public OVRInput.Controller controlR;  // 右手のコントローラー

    // 車のRigidbody
    private Rigidbody carRigidbody;
    public GameObject targetObject; //警報を発したときに車を止める範囲をコライダーのtrrigerで決めているがそのコライダーを持つオブジェクトをここで指定する
    public float delay = 3f; // 非アクティブになるまでの時間（秒）

    // 車の移動に関連するパラメータ
    public float moveSpeed = 1.0f;      // 車の移動速度
    public float rotationSpeed = 100.0f; // 車の回転速度

    // Start is called before the first frame update
    void Start()
    {
        // 車のRigidbodyを取得
        carRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // 左右のXYボタンで前進・後退
        HandleMovement();

        // 右ジョイスティックで車の回転方向を決定
        HandleSteering();
    }

    // 左右XYボタンで車を前進・後退させる
    void HandleMovement()
    {
        // 右コントローラーのXYボタン入力を取得
        if (OVRInput.Get(OVRInput.Button.One, controlR)) // XYボタン1（右コントローラー）: 前進
        {
            carRigidbody.velocity = transform.forward * moveSpeed; // 前進
        }
        else if (OVRInput.Get(OVRInput.Button.Two, controlR)) // XYボタン2（右コントローラー）: 後退
        {
            carRigidbody.velocity = -transform.forward * moveSpeed; // 後退
        }
        else if (OVRInput.Get(OVRInput.Button.One, controlL))//ABボタン１（左コントローラー）:警告
        {
            ActivateAndDisableAfterDelay();//クラクションで車を止める
        }
        else
        {
            carRigidbody.velocity = Vector3.zero; // ボタンが押されていない場合は停止
        }
    }

    // 右ジョイスティックで車の回転方向を決定
    void HandleSteering()
    {
        // 右ジョイスティックの左右入力を取得
        float rightJoystickX = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;

        // 右ジョイスティック左右の入力で回転方向を決定
        if (rightJoystickX > 0.1f) // 右に倒した場合
        {
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f); // 時計回り
        }
        else if (rightJoystickX < -0.1f) // 左に倒した場合
        {
            transform.Rotate(0f, -rotationSpeed * Time.deltaTime, 0f); // 反時計回り
        }
    }

    // ボタンを押した際に呼び出すメソッド
    public void ActivateAndDisableAfterDelay()
    {
        if (targetObject != null)
        {
            // オブジェクトをアクティブにする
            targetObject.SetActive(true);

            // 指定時間後に非アクティブにする
            StartCoroutine(DisableAfterDelay());
        }
        else
        {
            Debug.LogWarning("Target object is not assigned!");
        }
    }

    private IEnumerator DisableAfterDelay()
    {
        // 指定時間待機
        yield return new WaitForSeconds(delay);

        // オブジェクトを非アクティブにする
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }
}

