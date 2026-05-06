import { createSlice } from "@reduxjs/toolkit";
import axios from "axios";
import { variables } from "../variables";
import { toast } from "react-toastify";
import { 
    deleteClientData, 
    fetchClients, 
    fetchFirstClient, 
    fetchLastClient, 
    fetchNegotiationsByClient, 
    fetchNextClient, 
    fetchPreviousClient, 
    fetchPriceByUnit, 
    fetchProjects, 
    fetchUnitsByProject, 
    saveClient 
} from "../../services/clientService";
const initialState = {
    client: { ClientID: 0, ClientName: "", PhoneNumber: "", ClientStatus: "-1",Notes: ""},
    clientsList: [],
    projects: [],
    negotiation:{serialCode:"", ProjectCode:"-1",UnitID:"-1",
                 OriginalPrice:"",NegotiationPrice:"",DiscountAmount:"",
                 NegotiationStatus:"في انتظار موافقة الإدارة",
                 NegotiationDate:new Date().toISOString().split('T')[0],
                 checkedByAdmin:0,Reserved:0},
    negotiationsList:[], 
    units: [],
    totalUnitPrice:{},
    rowIndex:-1, 
    selectedNegotiationIndex:-1,
    isNegotiationModalOpen:false,
    isDeleteClientModalOpen:false,
    isSearchClientsModalOpen:false,
    isDeleteNegotiationModalOpen:false,
    deletedStatus:false,
  
}

const clientSlice = createSlice({
    name: "clients",
    initialState,
    reducers: {
        setClientData: (state, action) => {
            state.client = { ...state.client, ...action.payload };
        },
        resetClientForm: (state) => {
            state.client = initialState.client;
            state.totalUnitPrice={};
            state.negotiationsList=[];
        },
        toggleNegotiationModal:(state,action)=>{
            state.isNegotiationModalOpen=action.payload;
        },
        setNegotiationData:(state,action)=>{
            state.negotiation={...state.negotiation,...action.payload}
        },
        prepareNewNegotiation:(state,action)=>{
            state.negotiation = { ...initialState.negotiation, serialCode: action.payload };
        },
        calculateDiscount:(state)=>{
            if(state.negotiation.OriginalPrice>0 && state.negotiation.NegotiationPrice>0){
                const discount=(state.negotiation.NegotiationPrice/state.negotiation.OriginalPrice)*100; 
                state.negotiation.DiscountAmount=parseFloat(discount.toFixed(1));
            }
       },
       editingNegotiationRow:(state,action)=>{
         state.rowIndex=action.payload;
         if(state.rowIndex!==-1){
            state.negotiation = { ...state.negotiationsList[action.payload]};
         }
         state.isNegotiationModalOpen=true; 
       },
       saveNegotiationToTable:(state,action)=>{
            if(state.rowIndex===-1)
            {
                state.negotiationsList=[...state.negotiationsList,state.negotiation];
            }
            else{state.negotiationsList[state.rowIndex]=state.negotiation;}
            state.isNegotiationModalOpen=false; 
        },
        toggleDeleteClientModal:(state,action)=>{
         state.isDeleteClientModalOpen=action.payload;
        },
       toggleSearchClientsModal:(state,action)=>{
         state.isSearchClientsModalOpen=action.payload;

       },
       setClientsForm:(state,action)=>{
        const selectedClient = state.clientsList[action.payload];
        state.client = selectedClient;
        state.isSearchClientsModalOpen = false;
       },
        toggleDeleteNegotiaionModal:(state,action)=>{
            state.isDeleteNegotiationModalOpen=action.payload;
        },
        selectedDeleteNegotiationRow:(state,action)=>{
             state.selectedNegotiationIndex=action.payload;
             state.isDeleteNegotiationModalOpen=true;
        },
        confirmDeleteNegotiation:(state,action)=>{
            state.negotiationsList=state.negotiationsList.filter((items,index)=> index !==state.selectedNegotiationIndex);
            state.isDeleteNegotiationModalOpen=false;
        },
       
      
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchProjects.fulfilled, (state, action) => {
                state.projects = action.payload;
            })            
            .addCase(fetchUnitsByProject.fulfilled, (state, action) => {
                state.units = action.payload;
            })
            .addCase(fetchPriceByUnit.fulfilled, (state, action) => {
                state.negotiation.OriginalPrice = action.payload[0].TotalPrice;
            })
           
            .addCase(saveClient.fulfilled, (state, action) => {
                if(action.payload.nullData===true){
                  toast.error(" برجاء إدخال بيانات العميل لإكمال الحفظ!", {
                  theme: "colored",
                  position: "top-left",
                });
                }
                state.client.ClientID = action.payload.id;   
            })
            .addCase(deleteClientData.fulfilled, (state, action) => {
                state.deletedStatus = action.payload.deleted;
                if(state.deletedStatus===true){
                    toast.success("تم حذف بيانات العميل بنجاح", {
                    theme: "colored",
                    position: "top-left",
                 });
                }
                else{
                     toast.error("لا توجد بيانات للحذف ! حاول مرة أخري", {
                     theme: "colored",
                     position: "top-left",
                    });
                }
            })
            .addCase(fetchClients.fulfilled, (state, action) => {
                state.clientsList = action.payload;
            })
            .addCase(fetchNegotiationsByClient.fulfilled, (state, action) => {
                state.negotiationsList = action.payload;
            })
           
            .addCase(fetchFirstClient.fulfilled, (state, action) => {
                state.client = action.payload.dt[0];
                state.negotiationsList=action.payload.negotiations_f;
                if(action.payload.isnull===true){
                    state.client=initialState.client;
                    toast.error("لا يوجد بيانات لعملاء , حاول مره أخري", {
                      theme: "colored",
                      position: "top-left",
                    });
                }
            })
            .addCase(fetchLastClient.fulfilled, (state, action) => {
                state.client = action.payload.dt[0];
                state.negotiationsList=action.payload.negotiations_l;
                   if(action.payload.isnull===true){
                    state.client=initialState.client;
                    toast.error("لا يوجد بيانات لعملاء , حاول مره أخري", {
                      theme: "colored",
                      position: "top-left",
                    });
                }
            })
            .addCase(fetchNextClient.fulfilled, (state, action) => {
                state.client = action.payload.dt[0];
                state.negotiationsList=action.payload.negotiations_n;
                if(action.payload.islast===true){
                    toast.info("لا توجد سجلات تالية,انت بالفعل في السجل الأخير", {
                      theme: "colored",
                      position: "top-left",
                    });
                }
                if(action.payload.empty_db===true){
                    state.client=initialState.client;
                    toast.error("لا يوجد بيانات لعملاء , حاول مره أخري", {
                      theme: "colored",
                      position: "top-left",
                    });
                }
            })
           
            .addCase(fetchPreviousClient.fulfilled, (state, action) => {
                state.loading = false;
                state.client = action.payload.dt[0];
                state.negotiationsList=action.payload.negotiations_p;
               if(action.payload.isfirst===true){
                 toast.info("لا توجد سجلات سابقة,انت بالفعل في السجل الأول", {
                      theme: "colored",
                      position: "top-left",
                    });
               }
                if(action.payload.empty_db===true){
                    state.client=initialState.client;
                    toast.error("لا يوجد بيانات لعملاء , حاول مره أخري", {
                      theme: "colored",
                      position: "top-left",
                    });
                }
            })
    }
})
export const { 
    setClientData, 
    resetClientForm,
    toggleNegotiationModal ,
    setNegotiationData,
    prepareNewNegotiation,
    calculateDiscount,
    saveNegotiationToTable,
    toggleDeleteClientModal,
    toggleSearchClientsModal,
    setClientsForm,
    editingNegotiationRow,
    toggleDeleteNegotiaionModal,
    selectedDeleteNegotiationRow,
    confirmDeleteNegotiation,
} = clientSlice.actions;
const clientReducer = clientSlice.reducer;
export default clientReducer;