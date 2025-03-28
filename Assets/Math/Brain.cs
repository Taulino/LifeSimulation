using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace EPQ
{
    public class Brain
    {
        public const int InputSize = 8;
        public const int Hidden_2Size = 5;
        public const int HiddenSize = 3;
        public const int OutputSize = 2;

        public float[] currentFirstLayer;
        public float[] currentSecondLayer;
        public float[] currentThirdLayer;

        public const float MutationChance = 0.04f;

        public Matrix inputToHidden;
        public Matrix hiddenToHidden;
        public Matrix hiddenToOutput;

        public Brain(Matrix inputToHidden, Matrix hiddenToHidden, Matrix hiddenToOutput)
        {
            this.inputToHidden = inputToHidden;
            this.hiddenToHidden = hiddenToHidden;
            this.hiddenToOutput = hiddenToOutput;
        }
        public static Brain Mutate(Brain brain, float mutationChance) =>
            new Brain(
                Matrix.Mutate(brain.inputToHidden, mutationChance),
                Matrix.Mutate(brain.hiddenToHidden, mutationChance),
                Matrix.Mutate(brain.hiddenToOutput, mutationChance));
        public static Brain Random() =>
            new Brain(
                Matrix.Random(Hidden_2Size, InputSize + 1),
                Matrix.Random(HiddenSize, Hidden_2Size + 1),
                Matrix.Random(OutputSize, HiddenSize + 1));

        public IReadOnlyList<float> Think(IReadOnlyList<float> inputs)
        {
            currentFirstLayer = inputs.ToArray().Select(BipolarSigmoid).ToArray();
            IReadOnlyList<float> inputsWith1 = inputs.ToList().AttachOne().ToList();
            IReadOnlyList<float> sums = (inputToHidden * inputsWith1.VectorToColumnMatrix()).ColumnMatrixToVector();
            
            IReadOnlyList<float> hiddens = sums.Select(BipolarSigmoid).ToList();
            currentSecondLayer = hiddens.ToArray();
            IReadOnlyList<float> hiddensWith1 = hiddens.AttachOne();
            IReadOnlyList<float> hiddensToOutput = (hiddenToHidden * hiddensWith1.VectorToColumnMatrix()).ColumnMatrixToVector()
                .Select(BipolarSigmoid).ToList();
            currentThirdLayer = hiddensToOutput.ToArray();

            IReadOnlyList<float> hiddensToOutputWith1 = hiddensToOutput.AttachOne();
            IReadOnlyList<float> outputs = (hiddenToOutput * hiddensToOutputWith1.VectorToColumnMatrix()).ColumnMatrixToVector()
                                            .Select(BipolarSigmoid).ToList();
            return outputs;
        }

        private static float ReLU(float x) => Math.Max(0, x);
        private static float Sigmoid(float x) => 1 / (1 + (float)Math.Exp(-x));
        private static float BipolarSigmoid(float x)
        {
            if (x > 10) return 1;
            else if (x < -10) return -1;
            return (1 - Mathf.Exp(-x)) / (1 + Mathf.Exp(-x));
        }
        private static float Binary(float x) => x >= 0 ? 1 : 0;
        public static float Tanh(float x)
        {
            if (x > 10) return 1;
            else if (x < -10) return -1;

            return (Mathf.Exp(x) - Mathf.Exp(-x)) / (Mathf.Exp(x) + Mathf.Exp(-x));
        } 
        

        public static Brain Cross(Brain mom, Brain dad) =>
            new Brain(
                Matrix.Cross(mom.inputToHidden, dad.inputToHidden, MutationChance),
                Matrix.Cross(mom.hiddenToHidden, dad.hiddenToHidden, MutationChance),
                Matrix.Cross(mom.hiddenToOutput, dad.hiddenToOutput, MutationChance));
    }
}