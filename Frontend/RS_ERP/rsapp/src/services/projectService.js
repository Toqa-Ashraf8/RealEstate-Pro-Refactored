import { createAsyncThunk } from "@reduxjs/toolkit";
import axios from 'axios';
import { variables } from "../assets/variables";
//-------------------------------------------------------------------------------
export const uploadProjectImage = createAsyncThunk("uploadProjectImage/projects", async (formdata) => {
    const resp = await axios.post(variables.PROJECTS_API + "UploadProjectImage", formdata)
        .then((res) => res.data);
    return resp;
})
export const saveCompleteProject = createAsyncThunk("saveCompleteProject/projects", async (parms) => {
    const resp = await axios.post(variables.PROJECTS_API + "UpsertProjectWithUnits", parms)
        .then((res) => res.data)
    return resp;
})
export const uploadUnitImage = createAsyncThunk("uploadUnitImage/projects", async (formDatau) => {
    const resp = await axios.post(variables.PROJECTS_API + "UploadUnitImage", formDatau)
        .then((res) => res.data);
    return resp;
})
export const deleteProject = createAsyncThunk("deleteProject/projects", async (id) => {
    const resp = await axios.post(variables.PROJECTS_API + "DeleteProject?id=" + id)
        .then((res) => res.data);
    return resp;
})
export const fetchProjectsList = createAsyncThunk("fetchProjectsList/projects", async () => {
    const resp = await axios.get(variables.PROJECTS_API + "GetAllProjects")
        .then((res) => res.data);
    return resp;
})
export const fetchProjectUnits = createAsyncThunk("fetchProjectUnits/projects", async (id) => {
    const resp = await axios.post(variables.PROJECTS_API + "GetProjectUnits?projectId=" + id)
        .then((res) => res.data);
    return resp;
})
export const fetchUnitsDetails = createAsyncThunk("fetchUnitsDetails/projects", async (id) => {
    const resp = await axios.post(variables.PROJECTS_API + "GetUnitsByProject?Id=" + id)
        .then((res) => res.data);
    return resp;
})
