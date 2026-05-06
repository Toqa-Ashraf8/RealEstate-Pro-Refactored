import {createSlice}from '@reduxjs/toolkit'
import { loginUser, registerUsers } from '../../services/authService';

const initialState={
    user:{},
    token:sessionStorage.getItem('token'),
    userDetails:JSON.parse(sessionStorage.getItem('user')) || {},
    isLoggedin:!!sessionStorage.getItem('token')
}
const authSlice=createSlice({
    name:'auth',
    initialState,
    reducers:{
        setUserData:(state,action)=>{
            state.user={...state.user,...action.payload};
        },
        resetUserForm:(state,action)=>{
            state.user=initialState.user;
        },
        logOut:(state,action)=>{
           sessionStorage.clear();
            state.token = null;
            state.userDetails = {};
            state.isLoggedin = false;
        }
       
    },
    extraReducers:(builder)=>{
        builder
        .addCase(registerUsers.fulfilled,(state,action)=>{ 
            if(action.payload.token){
                sessionStorage.setItem('token',action.payload.token) 
                state.token=action.payload.token;
            }
            if(action.payload.user){
                sessionStorage.setItem('user',JSON.stringify(action.payload.user));
                state.userDetails=action.payload.user;
            }
       
        })
        .addCase(loginUser.fulfilled,(state,action)=>{ 
            if(action.payload.token){
                sessionStorage.setItem('token',action.payload.token) 
                state.token=action.payload.token;
            }
            if(action.payload.user){
                sessionStorage.setItem('user',JSON.stringify(action.payload.user));
                state.userDetails=action.payload.user;
            }
        
        })
    }
})
export const{setUserData,resetUserForm,logOut}=authSlice.actions;
const authReducer=authSlice.reducer;
export default authReducer;