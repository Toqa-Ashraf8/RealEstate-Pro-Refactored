import { createSlice } from '@reduxjs/toolkit';
import { fetchReservedClientById, generateInstallments } from '../../services/bookingService';

const initialState = {
  initialClientBookedData: JSON.parse(localStorage.getItem('activeBookingClient'))?.InitialClientData || {},
  clientBooked: JSON.parse(localStorage.getItem('activeBookingClient'))?.ClientExDetails || {},
  installmentInfoBooked: JSON.parse(localStorage.getItem('activeBookingClient'))?.BookingDetails || {},
  installmentsBooked: JSON.parse(localStorage.getItem('activeBookingClient'))?.installments || [],
  BookingDate: new Date().toISOString().split('T')[0],
  reserved: -1,
  
  // 🔽 ضيفي الحالات دي عشان الـ Modals تشتغل من الـ Slice ده
  isPaymentModalOpen: false,
  isRevertPaymentModalOpen: false,
  pendingPaymentIndex: null,
  isEditMode: 0
};

const manageBookingSlice = createSlice({
  name: 'manageBooking',
  initialState,
  reducers: {
    setClientB: (state, action) => {
        state.clientBooked = {...state.clientBooked, ...action.payload};
    },
    setInstallmentB: (state, action) => {
        state.installmentInfoBooked = {...state.installmentInfoBooked, ...action.payload};
    },
    calculateNewDownPayment: (state, action) => {
        const negoiationPrice = action.payload.total;
        state.installmentInfoBooked.TotalAmount = negoiationPrice;
        const newBalance = state.installmentInfoBooked.TotalAmount - action.payload.newReservationAmount;
        state.installmentInfoBooked.DownPayment = newBalance * 0.25;
    },
    hydrateFromStorage: (state, action) => {
        const data = action.payload;
        if (data) {
            state.clientBooked = { ...data.clientBooked, ...state.clientBooked };
            state.initialClientBookedData = data.initialClientBookedData ;
            state.installmentInfoBooked = data.installmentInfoBooked ||  state.installmentInfoBooked;
            state.installmentsBooked = data.installmentsBooked || state.installmentsBooked || [];
            state.reserved = 1;
        }
    },

    // 🔽 أكشنز التحكم في فتح وغلق المودال وتحديد القسط الحالي
    togglePaymentModal: (state, action) => {
        state.isPaymentModalOpen = action.payload;
    },
    toggleRevertModal: (state, action) => {
        state.isRevertPaymentModalOpen = !state.isRevertPaymentModalOpen;
        if(action.payload !== undefined) state.pendingPaymentIndex = action.payload;
    },
    setPendingPayment: (state, action) => {
        state.pendingPaymentIndex = action.payload.index;
        state.isEditMode = action.payload.isEdit;
    },

    // 🔽 الأكشن السحري اللي هيغير حالة القسط لـ "تم الدفع" (تأكيد الدفع)
    confirmInstallmentPayment: (state, action) => {
        const index = state.pendingPaymentIndex;
        if (index !== null && state.installmentsBooked[index]) {
            state.installmentsBooked[index].Paid = 1; // تحويله لـ تم الدفع
            // تقدري تضيفي هنا الـ PaymentType والـ CheckImage لو جايين من المودال
            if(action.payload) {
                state.installmentsBooked[index].PaymentType = action.payload.PaymentType || "";
                state.installmentsBooked[index].CheckImage = action.payload.CheckImage || "";
            }
        }
    },

    // 🔽 أكشن إلغاء الدفع (Revert) يرجع القسط "مستحق"
    revertInstallmentPayment: (state) => {
        const index = state.pendingPaymentIndex;
        if (index !== null && state.installmentsBooked[index]) {
            state.installmentsBooked[index].Paid = 0; // إرجاعه مستحق
            state.installmentsBooked[index].PaymentType = "";
            state.installmentsBooked[index].CheckImage = "";
        }
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchReservedClientById.fulfilled, (state, action) => {
            state.initialClientBookedData = action.payload.InitialClientData;
            state.clientBooked = action.payload.ClientExDetails;
            state.installmentInfoBooked = action.payload.BookingDetails; 
            state.installmentsBooked = action.payload.installments;
            localStorage.setItem('activeBookingClient', JSON.stringify(action.payload));
        })
      .addCase(generateInstallments.fulfilled, (state, action) => {
             state.installmentsBooked = action.payload;
      }) 
  }
});

export const {
  setClientB, 
  setInstallmentB,
  hydrateFromStorage,
  calculateNewDownPayment,
  togglePaymentModal,
  toggleRevertModal,
  setPendingPayment,
  confirmInstallmentPayment,
  revertInstallmentPayment
} = manageBookingSlice.actions;

const manageBookingReducer = manageBookingSlice.reducer;
export default manageBookingReducer;