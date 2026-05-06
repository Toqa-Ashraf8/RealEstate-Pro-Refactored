import { createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
import { variables } from "../assets/variables";

export const fetchAllClients=createAsyncThunk("fetchAllClients/clientsProfile",async()=>{
    const resp=await axios.get(variables.CLIENTSPROFILE_API+"GetAllClients")
    .then((res)=>res.data);
    return resp;
})
export const fetchClientFullDetails=createAsyncThunk("fetchClientFullDetails/clientsProfile",async(clientId)=>{
    const resp=await axios.post(variables.CLIENTSPROFILE_API+"GetClientDetails?clientid="+clientId)
    .then((res)=>res.data);
    return resp;
})
