using SRML.SR.SaveSystem.Data;
using SRML.SR.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreVaccing.Components
{
    public class FeralizeOnStart : MonoBehaviour, ExtendedData.Participant
    {
        private SlimeFeral slimeFeral;
        private bool shouldFeralize;

        public void Awake()
        {
            slimeFeral = GetComponent<SlimeFeral>();
            if (!slimeFeral)
                Destroy(this);
        }

        public void WriteData(CompoundDataPiece piece) => piece.SetPiece("shouldFeralize", slimeFeral.IsFeral());

        public void ReadData(CompoundDataPiece piece)
        {
            shouldFeralize = piece.HasPiece("shouldFeralize") ? piece.GetValue<bool>("shouldFeralize") : true;
            if (shouldFeralize)
            {
                if (!slimeFeral.IsFeral())
                    slimeFeral.SetFeral();
            }
            Destroy(this);
        }
    }
}
