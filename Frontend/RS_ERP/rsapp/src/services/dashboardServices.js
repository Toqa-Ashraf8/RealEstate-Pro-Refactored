import { createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
import { variables } from "../assets/variables"; 

export const fetchProjectsUnitsStats=createAsyncThunk("fetchProjectsUnitsStats/dashboard",
    async()=>{
        const resp=await axios.get(variables.DASHBOARD_API+"GetProjectsUnitsStats")
        .then((res)=>res.data);
        return resp;
})  

export const fetchMonthlyReservations=createAsyncThunk("fetchMonthlyReservations/dashboard",
    async()=>{
        const resp=await axios.get(variables.DASHBOARD_API+"GetDailyStats")
        .then((res)=>res.data);
        return resp;
}) 

export const fetchProjectsCount=createAsyncThunk("fetchProjectsCount/dashboard",
    async()=>{
        const resp=await axios.get(variables.DASHBOARD_API+"GetProjectsCount")
        .then((res)=>res.data);
        return resp;
}) 
export const fetchClientsCount=createAsyncThunk("fetchClientsCount/dashboard",
    async()=>{
        const resp=await axios.get(variables.DASHBOARD_API+"GetClientsCount")
        .then((res)=>res.data);
        return resp;
}) 
export const fetchNegotiationsCount=createAsyncThunk("fetchNegotiationsCount/dashboard",
    async()=>{
        const resp=await axios.get(variables.DASHBOARD_API+"GetNegotiationsCount")
        .then((res)=>res.data);
        return resp;
}) 
export const reservedUnitsCount=createAsyncThunk("reservedUnitsCount/dashboard",
    async()=>{
        const resp=await axios.get(variables.DASHBOARD_API+"GetReservedUnits")
        .then((res)=>res.data);
        return resp;
}) 
export const availableUnitsCount=createAsyncThunk("availableUnitsCount/dashboard",
    async()=>{
        const resp=await axios.get(variables.DASHBOARD_API+"SetAvailableUnits")
        .then((res)=>res.data);
        return resp;
}) 