﻿/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Brfalse MSIL
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;

using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using System.Reflection;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Brfalse)]
    public class Brfalse : MSIL
    {
        public Brfalse(Compiler Cmp)
            : base("brfalse", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            //This is branch type IL
            var xOffset = ((OpBranch)instr).Value;
            //The brach label
            var xTrueLabel = ILHelper.GetLabel(aMethod, xOffset);
            //Just make a pop because we want only size of it
            var xSize = Core.vStack.Pop().Size;

            /*
                value is pushed onto the stack by a previous operation.
                value is popped from the stack;
                if value is false, branch to target.
            */

            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        switch (xSize)
                        {
                            case 1:
                            case 2:
                            case 4:
                                {
                                    //***What we are going to do is***
                                    //1) Pop the content into EAX
                                    //2) Compare the content with 0x0 --> False
                                    //3) If they are equal than jump to branch
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceRef = "0x0" });
                                    Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JE, DestinationRef = xTrueLabel });
                                }
                                break;
                            case 8:
                                {
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceRef = "0x0" });

                                    string xFalseLabel = xTrueLabel + ".false";
                                    Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JNE, DestinationRef = xFalseLabel });
                                    Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
                                    Core.AssemblerCode.Add(new Cmp { DestinationReg = Registers.EAX, SourceRef = "0x0" });
                                    Core.AssemblerCode.Add(new Jmp { Condition = ConditionalJumpEnum.JE, DestinationRef = xTrueLabel });
                                    Core.AssemblerCode.Add(new Label(xFalseLabel));
                                }
                                break;
                            default:
                                //Size > 4 is never called don't know why
                                throw new Exception("@Brfalse: Unexpected size called := " + xSize);
                        }
                    }
                    break;
                #endregion
                #region _x64_
                case CPUArch.x64:
                    {

                    }
                    break;
                #endregion
                #region _ARM_
                case CPUArch.ARM:
                    {

                    }
                    break;
                #endregion
            }
        }
    }
}
