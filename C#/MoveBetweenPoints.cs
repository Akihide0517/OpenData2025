using UnityEngine;
using System.Collections;

public class MoveBetweenPoints : MonoBehaviour
{
    public Transform[] points; // 移動ポイントのリスト
    public float speed = 2f; // 移動速度
    public float rotationSpeed = 5f; // 回転速度
    public float stopDistance = 0.5f; // ポイント到達とみなす距離
    public float stopDuration = 10f; // 停止する時間

    private int currentPointIndex = 0; // 現在のターゲットポイントのインデックス
    private bool isStopped = false; // 停止中かどうか
    private float originalSpeed; // 元の速度を保持

    void Start()
    {
        // 元の速度を保存
        originalSpeed = speed;
    }

    void Update()
    {
        if (points.Length == 0)
        {
            Debug.LogWarning("No points assigned!");
            return;
        }

        // 停止中は処理をスキップ
        if (isStopped) return;

        // 現在のターゲットポイントを取得
        Transform target = points[currentPointIndex];

        // ターゲットへの方向を計算
        Vector3 direction = (target.position - transform.position).normalized;

        // 現在の向きからターゲットへの向きを計算
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 徐々にターゲットの方向を向く
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // ターゲットに向かって移動
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // ターゲットに到達したら次のポイントへ
        if (Vector3.Distance(transform.position, target.position) <= stopDistance)
        {
            currentPointIndex++;

            // 全ポイントを回ったらリセット（ループする場合）
            if (currentPointIndex >= points.Length)
            {
                currentPointIndex = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // playerタグを持つオブジェクトに接触した場合
        if (other.CompareTag("Player"))
        {
            StartCoroutine(StopMovementTemporarily());
            Debug.LogWarning("assigned!");
        }
    }

    private IEnumerator StopMovementTemporarily()
    {
        isStopped = true;
        speed = 0; // 速度を0にする
        yield return new WaitForSeconds(stopDuration); // 指定時間待機
        speed = originalSpeed; // 速度を元に戻す
        isStopped = false;
    }
}
