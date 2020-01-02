﻿using UnityEngine;
using System;
using XLua;

namespace IFramework.Lua
{
    [GCOptimize]
    [LuaCallCSharp]
    public struct Pedding
    {
        public byte c;
    }

    [GCOptimize]
    [LuaCallCSharp]
    public struct MyStruct
    {
        public MyStruct(int p1, int p2)
        {
            a = p1;
            b = p2;
            c = p2;
            e.c = (byte)p1;
        }
        public int a;
        public int b;
        public decimal c;
        public Pedding e;
    }

    [LuaCallCSharp]
    public enum MyEnum
    {
        E1,
        E2
    }

    [CSharpCallLua]
    public delegate int IntParam(int p);

    [CSharpCallLua]
    public delegate Vector3 Vector3Param(Vector3 p);

    [CSharpCallLua]
    public delegate MyStruct CustomValueTypeParam(MyStruct p);

    [CSharpCallLua]
    public delegate MyEnum EnumParam(MyEnum p);

    [CSharpCallLua]
    public delegate decimal DecimalParam(decimal p);

    [CSharpCallLua]
    public delegate void ArrayAccess(Array arr);

    [CSharpCallLua]
    public interface IExchanger
    {
        void exchange(Array arr);
    }

    [LuaCallCSharp]
    public class NoGc : MonoBehaviour
    {
        IntParam f1;
        Vector3Param f2;
        CustomValueTypeParam f3;
        EnumParam f4;
        DecimalParam f5;

        ArrayAccess farr;
        Action flua;
        IExchanger ie;
        LuaFunction add;

        [NonSerialized]
        public double[] a1 = new double[] { 1, 2 };
        [NonSerialized]
        public Vector3[] a2 = new Vector3[] { new Vector3(1, 2, 3), new Vector3(4, 5, 6) };
        [NonSerialized]
        public MyStruct[] a3 = new MyStruct[] { new MyStruct(1, 2), new MyStruct(3, 4) };
        [NonSerialized]
        public MyEnum[] a4 = new MyEnum[] { MyEnum.E1, MyEnum.E2 };
        [NonSerialized]
        public decimal[] a5 = new decimal[] { 1.00001M, 2.00002M };

        public float FloatParamMethod(float p)
        {
            return p;
        }

        public Vector3 Vector3ParamMethod(Vector3 p)
        {
            return p;
        }

        public MyStruct StructParamMethod(MyStruct p)
        {
            return p;
        }

        public MyEnum EnumParamMethod(MyEnum p)
        {
            return p;
        }

        public decimal DecimalParamMethod(decimal p)
        {
            return p;
        }

        // Use this for initialization
        void Start()
        {
            XLuaEnvironment.OnDispose += () =>
            {
                f1 = null;
                f2 = null;
                f3 = null;
                f4 = null;
                f5 = null;
                farr = null;
                flua = null;
                ie = null;
                add = null;

            };
            XLuaEnvironment.DoString(@"
                function id(...)
                    return ...
                end

                function add(a, b) return a + b end
            
                function array_exchange(arr)
                    arr[0], arr[1] = arr[1], arr[0]
                end

                local v3 = CS.UnityEngine.Vector3(7, 8, 9)
                local vt = CS.IFramework.Lua.MyStruct(5, 6)

                function lua_access_csharp()
                    monoBehaviour:FloatParamMethod(123) --primitive
                    monoBehaviour:Vector3ParamMethod(v3) --vector3
                    local rnd = math.random(1, 100)
                    local r = monoBehaviour:Vector3ParamMethod({x = 1, y = 2, z = rnd}) --vector3
                    assert(r.x == 1 and r.y == 2 and r.z == rnd)
                    monoBehaviour:StructParamMethod(vt) --custom struct
                    r = monoBehaviour:StructParamMethod({a = 1, b = rnd, e = {c = rnd}})
                    assert(r.b == rnd and r.e.c == rnd)
                    monoBehaviour:EnumParamMethod(CS.IFramework.Lua.MyEnum.E2) --enum
                    monoBehaviour:DecimalParamMethod(monoBehaviour.a5[0])
                    monoBehaviour.a1[0], monoBehaviour.a1[1] = monoBehaviour.a1[1], monoBehaviour.a1[0] -- field
                end

                exchanger = {
                    exchange = function(self, arr)
                        array_exchange(arr)
                    end
                }

                A = { B = { C = 789}}
                GDATA = 1234;
            ");

            XLuaEnvironment.GlobalTable.Set("monoBehaviour", this);

            XLuaEnvironment.GlobalTable.Get("id", out f1);
            XLuaEnvironment.GlobalTable.Get("id", out f2);
            XLuaEnvironment.GlobalTable.Get("id", out f3);
            XLuaEnvironment.GlobalTable.Get("id", out f4);
            XLuaEnvironment.GlobalTable.Get("id", out f5);

            XLuaEnvironment.GlobalTable.Get("array_exchange", out farr);
            XLuaEnvironment.GlobalTable.Get("lua_access_csharp", out flua);
            XLuaEnvironment.GlobalTable.Get("exchanger", out ie);
            XLuaEnvironment.GlobalTable.Get("add", out add);

            XLuaEnvironment.GlobalTable.Set("g_int", 123);
            XLuaEnvironment.GlobalTable.Set(123, 456);
            int i;
            XLuaEnvironment.GlobalTable.Get("g_int", out i);
            Debug.Log("g_int:" + i);
            XLuaEnvironment.GlobalTable.Get(123, out i);
            Debug.Log("123:" + i);
        }


        // Update is called once per frame
        void Update()
        {
            // c# call lua function with value type but no gc (using delegate)
            f1(1); // primitive type
            f2(new Vector3(1, 2, 3)); // vector3
            MyStruct mystruct1 = new MyStruct(5, 6);
            f3(mystruct1); // custom complex value type
            f4(MyEnum.E1); //enum
            decimal dec1 = -32132143143100109.00010001010M;
            f5(dec1); //decimal

            // using LuaFunction.Func<T1, T2, TResult>
            add.Func<int, int, int>(34, 56); // LuaFunction.Func<T1, T2, TResult>

            // lua access c# value type array no gc
            farr(a1); //primitive value type array
            farr(a2); //vector3 array
            farr(a3); //custom struct array
            farr(a4); //enum arry
            farr(a5); //decimal arry

            // lua call c# no gc with value type
            flua();

            //c# call lua using interface
            ie.exchange(a2);

            //no gc LuaTable use
            XLuaEnvironment.GlobalTable.Set("g_int", 456);
            int i;
            XLuaEnvironment.GlobalTable.Get("g_int", out i);

            XLuaEnvironment.GlobalTable.Set(123.0001, mystruct1);
            MyStruct mystruct2;
            XLuaEnvironment.GlobalTable.Get(123.0001, out mystruct2);

            decimal dec2 = 0.0000001M;
            XLuaEnvironment.GlobalTable.Set((byte)12, dec1);
            XLuaEnvironment.GlobalTable.Get((byte)12, out dec2);

            int gdata = XLuaEnvironment.GlobalTable.Get<int>("GDATA");
            XLuaEnvironment.GlobalTable.SetInPath("GDATA", gdata + 1);

            int abc = XLuaEnvironment.GlobalTable.GetInPath<int>("A.B.C");
            XLuaEnvironment.GlobalTable.SetInPath("A.B.C", abc + 1);

        }

        void OnDestroy()
        {
           
        }
    }
}