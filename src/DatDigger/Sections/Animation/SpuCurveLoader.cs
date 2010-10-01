using System;

namespace DatDigger.Sections.Animation
{
    static class SpuCurveLoader
    {
        public static void LoadCurves(BinaryReaderEx reader, SpuBinary spuBinary)
        {
            var numBones = spuBinary.NumBones;

            foreach (var section in spuBinary.Sections)
            {
                var bones = CreateBoneArray(numBones);

                foreach (var chunk in section.Chunks)
                {
                    reader.BaseStream.Position = spuBinary.SpuBasePosition + chunk.Offset;

                    for (var i = 0; i < chunk.NumChildren; i++)
                    {
                        var boneId = reader.ReadInt16(Endianness.BigEndian);
                        var type = reader.ReadInt16(Endianness.BigEndian);
                        var count = reader.ReadInt16(Endianness.BigEndian);
                        reader.ReadInt16(); // Skip 2 unused bytes

                        if ((type == 0 && chunk.Flag != 1) || (type != 0 && chunk.Flag == 1))
                        {
                            throw new InvalidOperationException(String.Format("Flag/Type mismatch? Flag = {0} Type = {1}", chunk.Flag, type));
                        }

                        AnimatedComponent component = DetermineAnimatedComponent(type);

                        Curves.CurveBase curve;
                        switch (type)
                        {
                            case 0x00:
                                curve = CreateQuaternionCurve(reader, count);
                                break;
                            case 0x04:
                            case 0x05:
                            case 0x06:
                            case 0x07:
                            case 0x08:
                            case 0x09:
                                curve = CreateCompressedLinearCurve(reader, component, count);
                                break;
                            case 0x0B:
                            case 0x0C:
                            case 0x0D:
                            case 0x0E:
                            case 0x0F:
                            case 0x10:
                                curve = CreateConstantCurve(reader, component, count);
                                break;
                            case 0x11:
                            case 0x12:
                            case 0x13:
                                curve = CreateUncompressedLinearCurve(reader, component, count);
                                break;
                            default:
                                throw new InvalidOperationException("Unknown curve type " + type);
                        }

                        if (curve.CurveValues.Count == 0) { throw new InvalidOperationException("No Curve Values!?"); }
                        AddCurveToBone(bones, boneId, curve);
                    }
                }

                for (var i = 0; i < numBones; i++)
                {
                    var bone = bones[i];
                    if (bone == null) { continue; }

                    var boneNode = new AnimatedBoneNode();
                    boneNode.Bone = bone;
                    boneNode.Parent = section;
                    section.Children.Add(boneNode);
                }
            }
        }

        private static AnimatedBone[] CreateBoneArray(int numBones)
        {
            var result = new AnimatedBone[numBones];

            return result;
        }

        private static Curves.ConstantCurve CreateConstantCurve(BinaryReaderEx reader, AnimatedComponent component, short count)
        {
            if (count != 1) { throw new InvalidOperationException("Unexpected count " + count + " for ConstantCurve"); }

            var val = reader.ReadSingle(Endianness.BigEndian);
            var result = new Curves.ConstantCurve(component, val);
            return result;
        }

        private static Curves.LinearCurve CreateUncompressedLinearCurve(BinaryReaderEx reader, AnimatedComponent component, short count)
        {
            var result = new Curves.LinearCurve(component);

            System.Diagnostics.Trace.WriteLine("Found UncompressedLinearCurve but we don't parse them properly yet :(");

            reader.BaseStream.Position += 6 * count;
            return result;
        }

        private static Curves.LinearCurve CreateCompressedLinearCurve(BinaryReaderEx reader, AnimatedComponent component, short count)
        {
            var result = new Curves.LinearCurve(component);

            bool flag = (count & 0x8000) != 0;
            int numValues = count & 0x7FFF;
            float offset = reader.ReadSingle(Endianness.BigEndian);
            float scale = reader.ReadSingle(Endianness.BigEndian);
            var lengths = reader.ReadBytes(numValues);
            for (var i = 0; i < numValues; i++)
            {
                var length = lengths[i];
                if (length == 0) { length = 1; }
                var indices = reader.ReadBytes(length);
                if (flag)
                {
                    for (var j = 0; j < length; j++)
                    {
                        ushort sval = reader.ReadUInt16(Endianness.BigEndian);
                        bool isNeg = (sval & 0x8000) != 0;
                        float val = (sval & 0x7FFF) / 32767.0f;
                        if (isNeg) { val = -val; }
                        val = val * scale + offset;
                        
                        Curves.LinearCurveValue curveVal = new Curves.LinearCurveValue()
                        {
                            Time = (float)(indices[j] + 256*i),
                            Value = val
                        };
                        result.Values.Add(curveVal);
                    }
                }
                else
                {
                    for (var j = 0; j < length; j++)
                    {
                        byte sval = reader.ReadByte();
                        bool isNeg = (sval & 0x80) != 0;
                        float val = (sval & 0x7F) / 127.0f;
                        if (isNeg) { val = -val; }
                        val = val * scale + offset;
                        Curves.LinearCurveValue curveVal = new Curves.LinearCurveValue()
                        {
                            Time = (float)(indices[j] + 256 * i),
                            Value = val
                        };
                        result.Values.Add(curveVal);
                    }
                }
            }

            // Align reader position to 4-byte boundary
            reader.BaseStream.Position = (reader.BaseStream.Position + 3) & 0x7FFFFFFFFFFFFFFC;

            return result;
        }

        private static Curves.QuaternionCurve CreateQuaternionCurve(BinaryReaderEx reader, short count)
        {
            var result = new Curves.QuaternionCurve();

            bool flag = (count & 0x80) != 0;
            int numLengths = count & 0xFF7F;

            var lengths = reader.ReadBytes(numLengths);

            // Lengths section is always a multiple of 4-bytes in length, so advance the stream past the unused bytes
            var unusedBytes = ((numLengths + 3) & 0x7FFFFFFC) - numLengths;

            reader.BaseStream.Position += unusedBytes;

            for (var i = 0; i < numLengths; i++)
            {
                byte len = lengths[i];
                for (var j = 0; j < len; j++)
                {
                    long data = 0;
                    data = (long)reader.ReadUInt16(Endianness.BigEndian) << 32;
                    data |= (long)reader.ReadUInt16(Endianness.BigEndian) << 16;
                    data |= (long)reader.ReadUInt16(Endianness.BigEndian);

                    int index = (int)(data >> 43) & 0x1F; // 5-bit index of this quaternion
                    int signFlags = (int)(data >> 39) & 0x0F; // 4-bit sign Flags
                    int val = (int)(data >> 17) & 0x3FFFFF; // 22-bit integer
                    int wVal = (int)(data & 0x1FFFF); // 17-bit integer

                    int globalIdx = index + 32 * i;

                    float w = (float)wVal / 131071.0f;
                    if (flag)
                    {
                        w = 1 - w * w;
                    }

                    double halfPi = Math.PI * 0.5;

                    double dVal = (double)val;
                    double sqrtVal = Math.Sqrt(dVal);
                    double sqrtValWholePart = Math.Truncate(sqrtVal);

                    double v44 = halfPi * (1 - (sqrtValWholePart / 2047));
                    double tmp = (sqrtValWholePart < 0.001) ? 0 : halfPi * (dVal - sqrtValWholePart * sqrtValWholePart) / (2 * sqrtValWholePart);

                    double tmp6 = Math.Sqrt(1 - w * w);
                    double tX = Math.Sin(-tmp);
                    double tY = Math.Sin(halfPi - tmp);
                    double tZ = Math.Sin(v44);
                    double tW = Math.Sin(v44 - halfPi);

                    float rX = (float)(tY * -tW * tmp6);
                    float rY = (float)(tZ * tmp6);
                    float rZ = (float)(-tX * -tW * tmp6);

                    SlimDX.Quaternion qValue = new SlimDX.Quaternion();
                    if ((signFlags & 0x8) != 0)
                    {
                        signFlags ^= 0xF; // Switch all the bits
                        qValue.W = -w;
                    }
                    else
                    {
                        qValue.W = w;
                    }

                    // Flag Set means Negate
                    qValue.X = ((signFlags & 0x4) != 0) ? -rX : rX;
                    qValue.Y = ((signFlags & 0x2) != 0) ? -rY : rY;
                    qValue.Z = ((signFlags & 0x1) != 0) ? -rZ : rZ;

                    qValue.Normalize();

                    var curveValue = new Curves.QuaternionCurveValue()
                    {
                        Time = (float)globalIdx,
                        Value = qValue
                    };

                    result.Values.Add(curveValue);
                }
            }

            return result;
        }

        private static void AddCurveToBone(AnimatedBone[] bones, int boneId, Curves.CurveBase curve)
        {
            if (boneId < 0 || boneId >= bones.Length)
            {
                throw new ArgumentOutOfRangeException("boneId");
            }

            AnimatedBone bone = bones[boneId];
            if (bone == null)
            {
                bone = new AnimatedBone();
                bone.BoneId = boneId;
                bones[boneId] = bone;
            }

            switch (curve.AnimatedComponent)
            {
                case AnimatedComponent.Quaternion:
                    {
                        if (bone.RotationCurve != null)
                        {
                            throw new InvalidOperationException("Two rotation curves for a bone?");
                        }

                        var qCurve = curve as Curves.QuaternionCurve;
                        if (qCurve == null)
                        {
                            throw new InvalidOperationException("Curve is not a quaternion curve?");
                        }

                        bone.RotationCurve = qCurve;
                    }
                    break;
                case AnimatedComponent.TranslationX:
                    {
                        if (bone.TranslationX != null)
                        {
                            throw new InvalidOperationException("Two TranslationX curves for a bone?");
                        }

                        var fCurve = curve as Curves.Curve<float>;
                        if (fCurve == null)
                        {
                            throw new InvalidOperationException("Curve is not a Curve<float>");
                        }

                        bone.TranslationX = fCurve;
                    }
                    break;
                case AnimatedComponent.TranslationY:
                    {
                        if (bone.TranslationY != null)
                        {
                            throw new InvalidOperationException("Two TranslationY curves for a bone?");
                        }

                        var fCurve = curve as Curves.Curve<float>;
                        if (fCurve == null)
                        {
                            throw new InvalidOperationException("Curve is not a Curve<float>");
                        }

                        bone.TranslationY = fCurve;
                    }
                    break;
                case AnimatedComponent.TranslationZ:
                    {
                        if (bone.TranslationZ != null)
                        {
                            throw new InvalidOperationException("Two TranslationZ curves for a bone?");
                        }

                        var fCurve = curve as Curves.Curve<float>;
                        if (fCurve == null)
                        {
                            throw new InvalidOperationException("Curve is not a Curve<float>");
                        }

                        bone.TranslationZ = fCurve;
                    }
                    break;
                case AnimatedComponent.ScaleX:
                    {
                        if (bone.ScaleX != null)
                        {
                            throw new InvalidOperationException("Two ScaleX curves for a bone?");
                        }

                        var fCurve = curve as Curves.Curve<float>;
                        if (fCurve == null)
                        {
                            throw new InvalidOperationException("Curve is not a Curve<float>");
                        }

                        bone.ScaleX = fCurve;
                    }
                    break;
                case AnimatedComponent.ScaleY:
                    {
                        if (bone.ScaleY != null)
                        {
                            throw new InvalidOperationException("Two ScaleY curves for a bone?");
                        }

                        var fCurve = curve as Curves.Curve<float>;
                        if (fCurve == null)
                        {
                            throw new InvalidOperationException("Curve is not a Curve<float>");
                        }

                        bone.ScaleY = fCurve;
                    }
                    break;
                case AnimatedComponent.ScaleZ:
                    {
                        if (bone.ScaleZ != null)
                        {
                            throw new InvalidOperationException("Two ScaleZ curves for a bone?");
                        }

                        var fCurve = curve as Curves.Curve<float>;
                        if (fCurve == null)
                        {
                            throw new InvalidOperationException("Curve is not a Curve<float>");
                        }

                        bone.ScaleZ = fCurve;
                    }
                    break;
            }
        }

        private static AnimatedComponent DetermineAnimatedComponent(short type)
        {
            switch (type)
            {
                case 0x00:
                    return AnimatedComponent.Quaternion;
                case 0x04:
                case 0x0B:
                case 0x11:
                    return AnimatedComponent.TranslationX;
                case 0x05:
                case 0x0C:
                case 0x12:
                    return AnimatedComponent.TranslationY;
                case 0x06:
                case 0x0D:
                case 0x13:
                    return AnimatedComponent.TranslationZ;
                case 0x07:
                case 0x0E:
                    return AnimatedComponent.ScaleX;
                case 0x08:
                case 0x0F:
                    return AnimatedComponent.ScaleY;
                case 0x09:
                case 0x10:
                    return AnimatedComponent.ScaleZ;
                default:
                    throw new InvalidOperationException("Unknown Curve Type " + type);
            }
        }
    }
}
