import { createSlice } from "@reduxjs/toolkit";
import { fetchAllClients, fetchClientFullDetails } from "../../services/clientsProfileService";
import { searchClients } from "../../services/bookingService";

const initialState={
    clientData:[],
    bookingData:JSON.parse(localStorage.getItem('bookingDetails')) || [],
    bookedUnitsData:JSON.parse(localStorage.getItem('bookedUnits')) || []
}
const clientsProfile=createSlice({
    name:'clientsProfile',
    initialState,
    extraReducers:(builder)=>{
    builder.
    addCase(fetchAllClients.fulfilled,(state,action)=>{
        state.clientData=action.payload;
    })
    .addCase(fetchClientFullDetails.fulfilled,(state,action)=>{
             const clientData = action.payload.clientData || [];
            const unitsData = action.payload.bookedUnitsData || [];
                localStorage.setItem('bookingDetails', JSON.stringify(clientData));
                localStorage.setItem('bookedUnits', JSON.stringify(unitsData));
                state.bookingData = clientData;
                state.bookedUnitsData = unitsData;
    })
    .addCase(searchClients.fulfilled, (state, action) => {
        state.clientData=action.payload;
    })
   }
})
const clientProfileReducer=clientsProfile.reducer;
export default clientProfileReducer;
