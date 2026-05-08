using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace MessengerRando.Scripts
{
    class GraphicDimensionSwap : MonoBehaviour
    {
        public Renderer spriteRenderer;
        Shader bit8Shader;
        Shader bit16Shader;
        void Start()
        {
            bit8Shader = Shader.Find("Sprites/8_Bits");
            bit16Shader = Shader.Find("Sprites/16_Bits");

        }
        void Update()
        {
            if (spriteRenderer != null && bit8Shader != null && bit16Shader != null)
            {
                //If we are in 16 bit and shader is 8 bit.. Swap Shader
                if (Manager<DimensionManager>.Instance.CurrentDimension == EBits.BITS_16 && spriteRenderer.material.shader.name.Equals(bit8Shader.name))
                {
                    spriteRenderer.material.shader = bit16Shader;
                }
                else if (Manager<DimensionManager>.Instance.CurrentDimension == EBits.BITS_8 && spriteRenderer.material.shader.name.Equals(bit16Shader.name))
                {
                    spriteRenderer.material.shader = bit8Shader;
                }
                else if (!spriteRenderer.material.shader.name.Equals(bit16Shader.name) && !spriteRenderer.material.shader.name.Equals(bit8Shader.name))
                {
                    spriteRenderer.material.shader = bit8Shader;
                }
            }
        }
    }
}
