using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{

    public const int LockedPieceBonus = 100;
    public const int ComboBonus = 50;
    public const int BaseScore = 10;
    public const int HugePieceBonus = 30;
    public const int ExplodeBonus = 100;
    public const int ExplosivePieceBonus = 50;

    [SerializeField] Text tScore;
    private int score;

    public int Score => score;

    private void Start()
    {
        tScore.text = score.ToString();
    }

    public void AddScore(int amount)
    {
        int currentScore = score;
        score += amount;
        this.DOKill();
        DOTween.To(() => currentScore, (value) =>
        {
            tScore.text = value.ToString();
        }, score, 0.5f).SetTarget(this);
    }

    public int Evaluate(List<Cluster> matches, int combo)
    {
        int totalScore = 0;
        int comboBonusScore = (matches.Count - 1) * ComboBonus;
        //Debug.Log($"Combo Bonus Score: {comboBonusScore}");
        totalScore += comboBonusScore;
        foreach (var cluster in matches)
        {
            int clusterScore = 0;
            if (cluster.ContainsPieceOfType(PieceType.HUGE))
            {
                clusterScore += (HugePieceBonus + NormalCluster(cluster.Size - 3)) * combo;
            }
            else
            {
                clusterScore += NormalCluster(cluster.Size) * combo;
                if (cluster.Size >= 5)
                {
                    //Debug.Log($"Explosive Piece Bonus : " + ExplosivePieceBonus);
                    clusterScore += ExplosivePieceBonus;
                }
            }

            foreach (var piece in cluster.listPiece)
            {
                if (piece.type == PieceType.LOCKED)
                {
                    //Debug.Log($"Locked Piece Bonus : " + LockedPieceBonus);
                    clusterScore += LockedPieceBonus;
                }
                else if (piece.type == PieceType.EXPLOSIVE)
                {
                    //Debug.Log($"Explode Bonus : " + ExplodeBonus);
                    clusterScore += ExplodeBonus;
                }
            }
            //Debug.Log($"Cluster Score: {clusterScore}");
            totalScore += clusterScore;
        }

        int NormalCluster(int size)
        {
            return BaseScore + (size - 3) * 20;
        }

        return totalScore;
    }
}
