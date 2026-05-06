import { createSlice ,createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
import { variables } from "../variables";
import { 
    fetchApprovedNegotiations,
    fetchPendingCount, 
    fetchPendingNegotiations, 
    fetchRejectedNegotiations, 
    processNegotiationReview, 
    updateNegotiationStatus
} from "../../services/negotiationService";
const initialState={
 pendingCount:"",
 rejectedCount:0,
 approvedCount:0,
//
  pendingRequests:[],
  acceptedRequests:[],
  rejectedRequests:[],   
//
selectedRequest:{
    ClientID:"",
    ClientName:"",
    ProjectCode:"",
    UnitID:"",
    NegotiationCondition:0,
    SuggestedPrice:0,
    ReasonOfReject:"",
    Reserved:0
},
 isRejectModalOpen:false,
 isConfirmModalOpen:false,
 isReConfirmModalOpen:false,
//
 selectedAcceptedNegotiation:{},
 selectedRejectedNegotiation:{},
 //
 isSavedSuccessfully:false,
 //if approved=1 -- Approve Negotiation / rejected=1 -- Reject Negotiation
    approved:0,
    rejected:0,
    CurrentDate:new Date().toISOString().split('T')[0],
    acceptedNeogitationId:-1
}

const negotiationSlice=createSlice({
    name:'negotiation',
    initialState,
    reducers:{
        toggleRejectModal:(state,action)=>{
            state.isRejectModalOpen=action.payload;
        },
        setClientRequestData:(state,action)=>{
           state.selectedRequest=state.pendingRequests[action.payload];
        },
        //إضافة قيم مودال رفض الطلب الي بيانات الصف لارسالها للحفظ 
        updateSelectedRequestData:(state,action)=>{
            state.selectedRequest={...state.selectedRequest,...action.payload};
        },
        //تغيير الحالة للطلب الي مقبول او مرفوض لو 1 مقبول و 0 مرفوض
        setNegotiationStatus:(state,action)=>{
            if(action.payload===1){
                state.selectedRequest.NegotiationCondition=1
            }
            else if(action.payload===0){
                state.selectedRequest.NegotiationCondition=0
            }
        },
        resetSelectedRequest:(state,action)=>{
            state.selectedRequest=initialState.selectedRequest;
        },
        toggleConfirmModal:(state,action)=>{
            state.isConfirmModalOpen=action.payload;
        },
       
        // تحويل طلب مقبول لطلب مرفوض
        selectAcceptedForUpdate:(state,action)=>{
            state.selectedAcceptedNegotiation=state.acceptedRequests[action.payload];
             if(state.selectedAcceptedNegotiation.NegotiationCondition===true){
                state.selectedAcceptedNegotiation.NegotiationCondition=0;
             }
        },
        // عند اعاده رفض الطلب نرسل قيم المودال (سبب الرفض - سعر المقترح ) مع الطلب المقبول 
        updateAcceptedNegotiationData:(state,action)=>{
            state.selectedAcceptedNegotiation={...state.selectedAcceptedNegotiation,...action.payload} 
        },
      
        //اذا كانت القيمة 1 الطلب مقبول وليس هناك قيم مودال رفض 
        prepareApproveAction:(state,action)=>{
            state.approved=action.payload;
            state.selectedRequest.ReasonOfReject="";
            state.selectedRequest.SuggestedPrice=0;
        },
        //اختيار سطر من الطلبات المفروضة لتحويل الي مقبول
          selectRejectedForUpdate:(state,action)=>{
            state.selectedRejectedNegotiation=state.rejectedRequests[action.payload];
             if(state.selectedRejectedNegotiation.NegotiationCondition===false){
                state.selectedRejectedNegotiation.NegotiationCondition=1;
             }
        },
        // لو كانت قيمة rejected=1 تحويله لمرفوض
         prepareRejectAction:(state,action)=>{
            state.rejected=action.payload;     
        },
      
    },
   
    extraReducers:(builder)=>{
        builder
        .addCase(fetchPendingCount.fulfilled,(state,action)=>{
            state.pendingCount=action.payload;
        })
        .addCase(fetchPendingNegotiations.fulfilled,(state,action)=>{
            state.pendingRequests=action.payload;
        })
        .addCase(processNegotiationReview.fulfilled,(state,action)=>{
            state.isSavedSuccessfully=action.payload.saved;
            if(state.isSavedSuccessfully){
                const pendingUnit=state.selectedRequest.UnitID;
                state.pendingRequests = 
                state.pendingRequests.filter(neg => neg.UnitID !== pendingUnit);
                if (state.pendingCount > 0) {
                   state.pendingCount -= 1;
                   
                 }
                 if (state.selectedRequest.NegotiationCondition === 1) {
                   state.approvedCount = (Number(state.approvedCount) || 0) + 1;
                 } else if (state.selectedRequest.NegotiationCondition === 0) {
                    state.rejectedCount = (Number(state.rejectedCount) || 0) + 1;
                  }   
                   state.selectedRequest = initialState.selectedRequest;       
             }
        }) 
        .addCase(fetchRejectedNegotiations.fulfilled,(state,action)=>{
            state.rejectedCount=action.payload.count;
            state.rejectedRequests=action.payload.dt;
        })
        .addCase(fetchApprovedNegotiations.fulfilled,(state,action)=>{
         state.approvedCount=action.payload.count_a;
         state.acceptedRequests=action.payload.dt;
        })
        .addCase(updateNegotiationStatus.fulfilled,(state,action)=>{
            const NegotiaitionIdToRemove = action.meta.arg.NegotiaionID; 
            if(action.payload.Re_Approved===true){
             state.rejectedRequests = state.rejectedRequests.filter(neg => neg.NegotiaionID !== NegotiaitionIdToRemove); 
            }
            else if(action.payload.Re_Rejected===true){
             state.acceptedRequests = state.acceptedRequests.filter(neg => neg.NegotiaionID !== NegotiaitionIdToRemove); 
            }
             if (state.pendingCount > 0) {
                 state.pendingCount -= 1;
            }
               
             state.selectedRejectedNegotiation = {};
             state.selectedAcceptedNegotiation = {};
        })
    }
})
export const{
    toggleRejectModal,
    setClientRequestData,
    updateSelectedRequestData,
    setNegotiationStatus,
    resetSelectedRequest,
    toggleConfirmModal,
    selectAcceptedForUpdate,
    updateAcceptedNegotiationData,
    prepareApproveAction,
    prepareRejectAction,
    selectRejectedForUpdate,
}=negotiationSlice.actions;
const negotiationReducer=negotiationSlice.reducer;
export default negotiationReducer;