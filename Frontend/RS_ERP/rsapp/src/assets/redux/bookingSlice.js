import { createSlice, createAsyncThunk } from '@reduxjs/toolkit'
import axios from 'axios';
import { variables } from '../variables'; 
import { 
    bookingDetailRequest,
    deleteBookingData,
    fetchAllReservedClients, 
    fetchReservedClientById, 
    fillClientData, 
    generateInstallments, 
    saveChecksImages, 
    saveInstallmentCheck, 
    saveNationalIdImage, 
    searchBookingClients,
} from '../../services/bookingService';


const initialState = {
   bookingClient: {
        NationalID: "", 
        NationalIdImagePath: "", 
        SecondaryPhone: "", 
        Address: "", 
        Job: "",
    },
    initialClientData:{},
    InstallmentInformation: { 
        BookingID: 0, 
        TotalAmount: 0, 
        ReservationAmount: 0, 
        DownPayment: 0, 
        FirstInstallmentDate: "", 
        PaymentMethod: "-1", 
        CheckImagePath: "",
        InstallmentYears: "-1", 
        Reserved: 1
    },
    paymentType:{
        PaymentType:"",
        CheckImage:""
    },
    reservedClients:[],
    installmentDetails:[],
    nationalIdImage: "",
    checkImage: "",
    installmentCheckImageName:"",
    isPaymentModalOpen:false,
    isRevertPaymentModalOpen:false,
    successSaveBookingData:false,
    successSaveInstallmentData:false,
    successUpdate:false,
    isDeletedBooking:false,
    successDelete:false,
    reserved:-1,
    selectedInstallmentrow:-1,
    selectedDeleteIndex: -1,
    BookingDate:new Date().toISOString().split('T')[0],
}

const bookingSlice = createSlice({
    name: 'booking',
    initialState,
    reducers: {
        setBookingClientData: (state, action) => {
            state.bookingClient = { ...state.bookingClient, ...action.payload };
        },
        resetBookingForm: (state) => {
            state.bookingClient = initialState.bookingClient;
            state.InstallmentInformation = initialState.InstallmentInformation;
            state.checkImage = "",
            state.nationalIdImage = "";
           
        },
        //حساب قيمة المقدم قبل الحجز
        caluclateDownPayment: (state, action) => {
            if (state.InstallmentInformation.ReservationAmount > 0) {
                const negoiationPrice = action.payload;
                state.InstallmentInformation.TotalAmount = negoiationPrice;
                const balance = 
                state.InstallmentInformation.TotalAmount - state.InstallmentInformation.ReservationAmount;
                state.InstallmentInformation.DownPayment = balance * 0.25; 
            }
           
        },
        //حساب قيمة المقدم بقيمة الحجز من الداتا بيز
        calculateNewDownPayment:(state,action)=>{
            const negoiationPrice = action.payload.total;
            state.InstallmentInformation.TotalAmount = negoiationPrice;
             const newBalance = 
             state.InstallmentInformation.TotalAmount - action.payload.newReservationAmount;
             state.InstallmentInformation.DownPayment = newBalance * 0.25;
        },
        setInstallmentData: (state, action) => {
            state.InstallmentInformation = { ...state.InstallmentInformation, ...action.payload };
        },
        togglePaymentModal:(state,action)=>
        {
            state.isPaymentModalOpen=action.payload;
        },
        setPaymentModalValues:(state,action)=>{
            state.paymentType={...state.paymentType,...action.payload}
        },
        resetPaymentModal:(state)=>{
            state.paymentType={PaymentType:"",CheckImage:""};
            state.installmentCheckImageName="";
        },
        //reserved=1 لو يجيب قيم جدول الاقساط وبيانات العميل من الداتا بيز 
        //reserved=0  يعمل جدول اقساط وبيانات عميل جديد 
        setReservationStatus:(state,action)=>{
            state.reserved=action.payload;
        },
       updateDownPaymentManual: (state, action) => {
            state.InstallmentInformation = {
                ...state.InstallmentInformation,
                DownPayment: action.payload
            };
        },
        setInitialClientData:(state,action)=>{
            state.initialClientData=action.payload;    
        },
        toggleRevertPymentModal:(state,action)=>{
            state.isRevertPaymentModalOpen=action.payload;
        },
        setPendingPayment: (state, action) => {
            const {index,isEdit} = action.payload;
            state.selectedInstallmentrow=index;
            if(isEdit===1){
                const rowData = state.installmentDetails[index];
                state.paymentType = {
                PaymentType: rowData.PaymentType || "",
                CheckImage: rowData.CheckImage || ""
            };
            }
            else {
            state.paymentType = { PaymentType: "", CheckImage: "" };
            }
        },
        confirmpaidStatus: (state) => {
            const index = state.selectedInstallmentrow;
            if (index !== -1) {
            state.installmentDetails[index] = {
                ...state.installmentDetails[index],
                Paid: 1, 
                PaymentType: state.paymentType.PaymentType,
                CheckImage: state.paymentType.CheckImage
            };
            state.selectedInstallmentrow = -1;
            }
            state.isPaymentModalOpen = false; 
        },
        toggleRevertModal:(state,action)=>{
            state.selectedDeleteIndex=action.payload;
            state.isRevertPaymentModalOpen=true;
        },
       
        confirmRevertPayment: (state) => {
            const index = state.selectedDeleteIndex;
            if (index !== -1) {
            state.installmentDetails[index] = {
                ...state.installmentDetails[index],
                Paid: 0,
                PaymentType: "",
                CheckImage: ""
            };
            state.isRevertPaymentModalOpen = false;
            state.selectedDeleteIndex = -1;
        }
      },
      deleteBookingRow:(state,action)=>{
        state.reservedClients=
        state.reservedClients.filter((client)=>client.BookingID!==action.payload);
      },
      hydrateFromStorage: (state, action) => {
        state.reserved=1;
        const data = action.payload;
        state.initialClientData = data.InitialClientData;
        state.bookingClient = data.ClientExDetails;
        state.InstallmentInformation = data.BookingDetails;
        state.installmentDetails = data.installments;
    },

    },
    extraReducers: (builder) => {
        builder
            .addCase(fillClientData.fulfilled, (state, action) => {
             state.initialClientData = action.payload.dt[0];
            })         
            .addCase(saveNationalIdImage.fulfilled, (state, action) => {
                state.nationalIdImage = action.payload;
            })       
            .addCase(saveChecksImages.fulfilled, (state, action) => {
                state.checkImage = action.payload;
            })
            .addCase(generateInstallments.fulfilled, (state, action) => {
                state.installmentDetails = action.payload;
            })        
            .addCase(saveInstallmentCheck.fulfilled, (state, action) => {
                state.installmentCheckImageName = action.payload;
            })        
            .addCase(bookingDetailRequest.fulfilled, (state, action) => {
                state.bookingClient.BookingID = action.payload.booking_id;
            })
            .addCase(fetchAllReservedClients.fulfilled, (state, action) => {
                state.reservedClients=action.payload;
            })
          
             .addCase(fetchReservedClientById.fulfilled, (state, action) => {
                state.initialClientData=action.payload.InitialClientData;
                state.bookingClient = action.payload.ClientExDetails;
                state.InstallmentInformation = action.payload.BookingDetails; 
                state.installmentDetails = action.payload.installments;
               localStorage.setItem('activeBookingClient', JSON.stringify(action.payload));
          })
            .addCase(deleteBookingData.fulfilled, (state, action) => {
                state.isDeletedBooking=action.payload;
            })
            .addCase(searchBookingClients.fulfilled, (state, action) => {
               state.reservedClients=action.payload;
            })
            
    }
})
export const {
    setBookingClientData, 
    resetBookingForm, 
    caluclateDownPayment,
    setInstallmentData,
    togglePaymentModal,
    setPaymentModalValues,
    resetPaymentModal,
    setReservationStatus,
    updateDownPaymentManual,
    calculateNewDownPayment,
    setInitialClientData,
    setIndexofInstallmentRow,
    toggleRevertPymentModal,
    setPendingPayment,
    confirmpaidStatus,
    toggleRevertModal,
    confirmRevertPayment,
    deleteBookingRow,
    hydrateFromStorage
} = bookingSlice.actions;
const bookingReducer = bookingSlice.reducer;
export default bookingReducer;