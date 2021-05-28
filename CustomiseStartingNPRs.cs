using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Aurora;
using Aurora.Properties;
using Lib;
using HarmonyLib;


namespace CustomiseStartingNPRs
{
    public class CustomiseStartingNPRs : AuroraPatch.Patch
    {
        public override string Description => "Customise your starting NPRs at gamestart";
        protected override void Loaded(Harmony harmony)
        {
            LogInfo("Loading patch CustomiseStartingNPRs...");

            // aw.cs is Game.cs
            Type game = AuroraAssembly.GetType("aw");

            var myGenNPRtranspiler = typeof(CustomiseStartingNPRs).GetMethod("GenerateStartingNPRTranspile");

            // GenerateStartingNPR is "g2"
            IEnumerable<MethodInfo> alltheg2s = game.GetMethods().Where(thing => thing.Name == "g2");
            MethodInfo GenerateStartingNPRAuroraMethod = null;
            // 3 arguments - "iz", double,double, returns a void
            foreach (MethodInfo thismethod in alltheg2s)
            {
                int i = thismethod.GetParameters().Length;
                Type retype = thismethod.ReturnType;
                if (i == 3 && retype == typeof(void))
                {
                    String p = thismethod.GetParameters()[0].ParameterType.ToString();
                    String q = thismethod.GetParameters()[1].ParameterType.ToString();
                    String r = thismethod.GetParameters()[2].ParameterType.ToString();

                    if (p == "iz" && q == "System.Double" && r == "System.Double")
                    {
                        GenerateStartingNPRAuroraMethod = thismethod;
                    }

                }
            }

            if (GenerateStartingNPRAuroraMethod != null)
            {
                LogInfo("Ready to transpile GenerateStartingNPR");
                harmony.Patch(GenerateStartingNPRAuroraMethod, transpiler: new HarmonyMethod(myGenNPRtranspiler));
            }
            else
            {
                LogInfo("Didn't patch GenerateStartingNPR as it wasnt found");
            }

        }

        public static IEnumerable<CodeInstruction> GenerateStartingNPRTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            //allow npr form to come up
            codes[151].opcode = OpCodes.Ldc_I4_1;
            return (codes);
        }
    }

}
