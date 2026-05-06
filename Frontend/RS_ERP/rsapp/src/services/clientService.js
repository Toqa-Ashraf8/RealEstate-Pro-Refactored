import { createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
import { variables } from "../assets/variables"; 


export const fetchProjects = createAsyncThunk("fetchProjects/clients", async () => {
    const resp = await axios.get(variables.CLIENTS_API + "GetAllProjects")
        .then((res) => res.data);
    return resp;
})
export const fetchUnitsByProject = createAsyncThunk("fetchUnitsByProject/clients", async (id) => {
    const resp = await axios.post(variables.CLIENTS_API + "GetUnitsByProject?projectid=" + id)
        .then((res) => res.data);
    return resp;
})
export const fetchPriceByUnit = createAsyncThunk("fetchPriceByUnit/clients", async (id) => {
    const resp = await axios.post(variables.CLIENTS_API + "GetUnitPrice?unitid=" + id)
        .then((res) => res.data);
    return resp;
})
export const saveClient=createAsyncThunk("saveClient/clients",async(parms)=>{
    const resp=await axios.post(variables.CLIENTS_API+"UpsertClient",parms)
        .then((res)=>res.data);
    return resp;
})
export const deleteClientData=createAsyncThunk("deleteClientData/clients",async(id)=>{
    const resp=await axios.delete(variables.CLIENTS_API+"DeleteClient?id="+id)
    .then((res)=>res.data);
    return resp;
})
export const fetchClients=createAsyncThunk("fetchClients/clients",async()=>{
    const resp=await axios.get(variables.CLIENTS_API+"GetAllClients")
    .then((res)=>res.data);
    return resp;
})
export const fetchNegotiationsByClient=createAsyncThunk("fetchNegotiationsByClient/clients",async(clientid)=>{
    const resp=await axios.post(variables.CLIENTS_API+"GetClientNegotiations?ClientID="+clientid)
    .then((res)=>res.data);
    return resp;
})
export const fetchFirstClient=createAsyncThunk("fetchFirstClient/clients",async()=>{
    const resp=await axios.get(variables.CLIENTS_API+"GetFirstClient")
    .then((res)=>res.data);
    return resp;
})
export const fetchLastClient=createAsyncThunk("fetchLastClient/clients",async()=>{
    const resp=await axios.get(variables.CLIENTS_API+"GetLastClient")
    .then((res)=>res.data);
    return resp;
})
export const fetchNextClient=createAsyncThunk("fetchNextClient/clients",async(id)=>{
    const resp=await axios.post(variables.CLIENTS_API+"GetNextClientById?id="+id)
    .then((res)=>res.data);
    return resp;
})
export const fetchPreviousClient=createAsyncThunk("fetchPreviousClient/clients",async(id)=>{
    const resp=await axios.post(variables.CLIENTS_API+"GetPreviousClientById?id="+id)
    .then((res)=>res.data);
    return resp;
})