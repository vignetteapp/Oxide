// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Oxide.Javascript.Interop;

namespace Oxide.Javascript
{
    internal class JSCore
    {

#pragma warning disable CA2101

        internal const string LIB_WEBCORE = @"WebCore";

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSStringCreateWithUTF8CString(IntPtr ptr);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSStringRelease(IntPtr str);

        [DllImport(LIB_WEBCORE, ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr JSStringGetCharactersPtr(IntPtr str);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSStringGetMaximumUTF8CStringSize(IntPtr str);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static unsafe extern uint JSStringGetUTF8CString(IntPtr str, byte* buffer, uint bufferSize);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSValueProtect(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSValueUnprotect(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern JSType JSValueGetType(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeBoolean(IntPtr ctx, [MarshalAs(UnmanagedType.I1)] bool boolean);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueToBoolean(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeNull(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeNumber(IntPtr ctx, double number);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeFromJSONString(IntPtr ctx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string jsonString);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern double JSValueToNumber(IntPtr ctx, IntPtr value, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeString(IntPtr ctx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string value);

        [DllImport(LIB_WEBCORE, EntryPoint = "JSValueToStringCopy")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))]
        internal static extern string JSValueToString(IntPtr ctx, IntPtr value, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeUndefined(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsArray(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsUndefined(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsObjectOfClass(IntPtr ctx, IntPtr value, IntPtr jsClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern JSTypedArrayType JSValueGetTypedArrayType(IntPtr ctx, IntPtr value, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSClassCreate(JSClassDefinition definition);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSClassRetain(IntPtr jsClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSClassRelease(IntPtr jsClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSClassGetPrivate(IntPtr jsClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSClassSetPrivate(IntPtr jsClass, IntPtr data);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMake(IntPtr ctx, IntPtr jsClass, IntPtr data);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeFunctionWithCallback(IntPtr ctx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string name, JSObjectCallAsFunctionCallback callAsFunction);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeConstructor(IntPtr ctx, IntPtr jsClass, JSObjectCallAsConstructorCallback callAsConstructor);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeArray(IntPtr ctx, uint argumentCount, IntPtr arguments, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeDate(IntPtr ctx, uint argumentCount, IntPtr arguments, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeError(IntPtr ctx, uint argumentCount, IntPtr arguments, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeRegExp(IntPtr ctx, uint argumentCount, IntPtr arguments, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeDeferredPromise(IntPtr ctx, IntPtr resolve, IntPtr reject, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeFunction(IntPtr ctx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string name, uint parameterCount, IntPtr parameterNames, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string body, IntPtr sourceURL, int startingLineNumber, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPrototype(IntPtr ctx, IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSObjectSetPrototype(IntPtr ctx, IntPtr obj, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectHasProperty(IntPtr ctx, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetProperty(IntPtr ctx, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectSetProperty(IntPtr ctx, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectDeleteProperty(IntPtr ctx, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectHasPropertyForKey(IntPtr ctx, IntPtr obj, IntPtr propertyKey, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPropertyForKey(IntPtr ctx, IntPtr obj, IntPtr propertyKey, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSObjectSetPropertyForKey(IntPtr ctx, IntPtr obj, IntPtr propertyKey, IntPtr value, JSPropertyAttribute attributes, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectDeletePropertyForKey(IntPtr ctx, IntPtr obj, IntPtr propertyKey, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPropertyAtIndex(IntPtr ctx, IntPtr obj, uint propertyIndex, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSObjectSetPropertyAtIndex(IntPtr ctx, IntPtr obj, uint propertyIndex, IntPtr value, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPrivate(IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectSetPrivate(IntPtr obj, IntPtr data);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectIsFunction(IntPtr ctx, IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectCallAsFunction(IntPtr ctx, IntPtr obj, IntPtr thisObject, uint argumentCount, IntPtr arguments, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectIsConstructor(IntPtr ctx, IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectCallAsConstructor(IntPtr ctx, IntPtr obj, uint argumentCount, IntPtr arguments, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectCopyPropertyNames(IntPtr ctx, IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSPropertyNameArrayRetain(IntPtr array);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSPropertyNameArrayRelease(IntPtr array);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSPropertyNameArrayGetCount(IntPtr array);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))]
        internal static extern string JSPropertyNameArrayGetNameAtIndex(IntPtr array, uint index);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSPropertyNameAccumulatorAddName(IntPtr accumulator, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectSetPrivateProperty(IntPtr ctx, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPrivateProperty(IntPtr ctx, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectDeletePrivateProperty(IntPtr ctx, IntPtr obj, IntPtr proeprtyName);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeTypedArray(IntPtr ctx, JSTypedArrayType arrayType, uint length, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeTypedArrayWithBytesNoCopy(IntPtr ctx, JSTypedArrayType arrayType, IntPtr bytes, uint byteLength, IntPtr bytesDeallocator, IntPtr deallocatorContext, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeTypedArrayWithArrayBufferAndOffset(IntPtr ctx, JSTypedArrayType arrayType, IntPtr buffer, uint byteOffset, uint length, IntPtr excetpion);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetTypedArrayBytesPtr(IntPtr ctx, IntPtr obj, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSObjectGetTypedArrayLength(IntPtr ctx, IntPtr obj, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSObjectGetTypedArrayByteLength(IntPtr ctx, IntPtr obj, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSObjectGetTypedArrayByteOffset(IntPtr ctx, IntPtr obj, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetTypedArrayBuffer(IntPtr ctx, IntPtr obj, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeArrayBufferWithBytesNoCopy(IntPtr ctx, IntPtr bytes, uint byteLength, IntPtr bytesDeallocator, IntPtr deallocatorContext, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetArrayBufferBytesPtr(IntPtr ctx, IntPtr obj, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSObjectGetArrayBufferByteLength(IntPtr ctx, IntPtr obj, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSContextGetGlobalObject(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSContextGroupCreate();

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSContextGroupRelease(IntPtr group);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSGlobalContextCreateInGroup(IntPtr group, IntPtr globalObjectClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSGlobalContextRelease(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSContextGetGroup(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSContextGetGlobalContext(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSEvaluateScript(IntPtr ctx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string script, IntPtr thisObj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string sourceURL, int startingLineNumber, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSCheckScriptSyntax(IntPtr ctx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string script, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string sourceURL, int startingLineNumber, out IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSGarbageCoillect(IntPtr ctx);

#pragma warning restore CA2101

    }
}
