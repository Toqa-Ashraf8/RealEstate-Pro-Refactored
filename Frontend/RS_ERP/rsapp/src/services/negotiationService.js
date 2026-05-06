import { createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
import { variables } from "../assets/variables";

//جلب عدد الطلبات التي مازالت معلقة لم يقبلها او يرفضها المدير
export const fetchPendingCount=createAsyncThunk("fetchPendingCount/negotiaion",async()=>{
    const resp=await axios.get(variables.NEGOTIATIONS_API+"GetPendingNegotiationsCount")
    .then((res)=>res.data);
    return resp;
})
//جلب الطلبات التي مازالت لم يوافق عليها او يرفضها المدير 
export const fetchPendingNegotiations=createAsyncThunk("fetchPendingNegotiations/negotiaion",async()=>{
    const resp=await axios.get(variables.NEGOTIATIONS_API+"GetPendingNegotiations")
    .then((res)=>res.data);
    return resp;
})
//قبول او رفض الطلب عن طريق المدير 
export const processNegotiationReview=createAsyncThunk("processNegotiationReview/negotiaion",async(acceptedrow)=>{
    const resp=await axios.post(variables.NEGOTIATIONS_API+"ProcessNegotiationReview",acceptedrow)
    .then((res)=>res.data);
    return resp;
})
//  جلب الطلبات المرفوضة وعددها
export const fetchRejectedNegotiations=createAsyncThunk("fetchRejectedNegotiations/negotiaion",async()=>{
    const resp=await axios.get(variables.NEGOTIATIONS_API+"GetRejectedNegotiations")
    .then((res)=>res.data);
    return resp;
})
//  جلب الطلبات المقبولة وعددها
export const fetchApprovedNegotiations=createAsyncThunk("fetchApprovedNegotiations/negotiaion",async()=>{
    const resp=await axios.get(variables.NEGOTIATIONS_API+"GetApprovedNegotiations")
    .then((res)=>res.data);
    return resp;
})
//  تحديث حالة طلب مقبول إلى مرفوض
export const updateNegotiationStatus=createAsyncThunk("updateNegotiationStatus/negotiaion",async(approvedRequest)=>{
    const resp=await axios.post(variables.NEGOTIATIONS_API+"UpdateNegotiationReview",approvedRequest)
    .then((res)=>res.data);
    return resp;
})