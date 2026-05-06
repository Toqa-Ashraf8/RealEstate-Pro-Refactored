import { configureStore } from '@reduxjs/toolkit';
import projReducer from './projectSlice';
import clientReducer from './clientSlice';
import negotiationReducer from './negotiationSlice';
import bookingReducer from './bookingSlice';
import uiReducer from './uiSlice';
import authReducer from './authSlice';
import dashReducer from './dashboardSlice';
import clientProfileReducer from './clientsProfileSlice';
export const store = configureStore({
    reducer: {
        projects: projReducer,
        clients: clientReducer,
        negotiation:negotiationReducer,
        booking:bookingReducer,
        ui:uiReducer,
        auth:authReducer,
        dashboard:dashReducer,
        clientsProfile:clientProfileReducer
    }
})
