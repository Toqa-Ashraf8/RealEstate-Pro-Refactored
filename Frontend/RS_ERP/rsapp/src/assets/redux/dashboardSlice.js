import { createSlice } from "@reduxjs/toolkit";
import { 
    availableUnitsCount, 
    fetchClientsCount, 
    fetchMonthlyReservations, 
    fetchNegotiationsCount, 
    fetchProjectsCount, 
    fetchProjectsUnitsStats, 
    reservedUnitsCount 
} from "../../services/dashboardServices"; 

const initialState={
    projectsUnitsCounts:[],
    reservationstatistics:[],
    projectsCount:"",
    clientsCount:"",
    negotiationsCount:"",
    reservedUnits:"",
    availableUnits:"",

}
const dashboardSlice=createSlice({
    name:'dashboard',
    initialState,
    reducers:{
        changvl:(state,action)=>{
            state.unit={...state.unit,...action.payload};
        }
    },
    extraReducers:(builder)=>{
        builder
        .addCase(fetchProjectsUnitsStats.fulfilled,(state,action)=>{
            state.projectsUnitsCounts=action.payload;
        })
        .addCase(fetchMonthlyReservations.fulfilled,(state,action)=>{
            state.reservationstatistics=action.payload;
        })
        .addCase(fetchProjectsCount.fulfilled,(state,action)=>{
            state.projectsCount=action.payload;
        })
        .addCase(fetchClientsCount.fulfilled,(state,action)=>{
            state.clientsCount=action.payload;
        }) 
        .addCase(fetchNegotiationsCount.fulfilled,(state,action)=>{
            state.negotiationsCount=action.payload;
        })
        .addCase(reservedUnitsCount.fulfilled,(state,action)=>{
            state.reservedUnits=action.payload;
        })
        .addCase(availableUnitsCount.fulfilled,(state,action)=>{
            state.availableUnits=action.payload;
        })
    }
})
export const {changvl}=dashboardSlice.actions;
const dashReducer=dashboardSlice.reducer;
export default dashReducer;