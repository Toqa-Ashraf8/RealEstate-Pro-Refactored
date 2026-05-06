import { createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
import { variables } from "../assets/variables";

export const fillClientData = createAsyncThunk("fillClientData/booking", async (Clientdata) => {
    const resp = await axios.post(variables.BOOKINGS_API + "GetBookingClientData", Clientdata)
        .then((res) => res.data);
    return resp;
})
export const saveNationalIdImage = createAsyncThunk("saveNationalIdImage/booking", async (formData) => {
    const resp = await axios.post(variables.BOOKINGS_API + "NationalIdUploadRequest", formData)
        .then((res) => res.data);
    return resp;
})
export const saveChecksImages = createAsyncThunk("saveChecksImages/booking", async (formData_) => {
    const resp = await axios.post(variables.BOOKINGS_API + "CheckUploadRequest", formData_)
        .then((res) => res.data);
    return resp;
})

export const generateInstallments = createAsyncThunk("generateInstallments/booking", async (request) => {
    const resp = await axios.post(variables.BOOKINGS_API + "GenerateInstallments", request)
        .then((res) => res.data);
    return resp;
})
export const saveInstallmentCheck = createAsyncThunk("saveInstallmentCheck/booking", async (formData) => {
    const resp = await axios.post(variables.BOOKINGS_API + "InstallmentCheckUploadRequest", formData)
        .then((res) => res.data);
    return resp;
})
export const bookingDetailRequest = createAsyncThunk("bookingDetailRequest/booking", async (sentdata) => {
    const resp = await axios.post(variables.BOOKINGS_API + "ConfirmFullBooking", sentdata)
        .then((res) => res.data);
    return resp;
})
export const confirmReservation = createAsyncThunk("confirmReservation/booking", async (row) => {
    const resp = await axios.post(variables.BOOKINGS_API + "ConfirmReservation", row)
        .then((res) => res.data);
    return resp;
})
export const fetchAllReservedClients = createAsyncThunk("fetchAllReservedClients/booking", async () => {
    const resp = await axios.get(variables.BOOKINGS_API + "GetAllReservedClients")
        .then((res) => res.data);
    return resp;
})
//وقت تعديل الحجز لجلب بيانات الحجز كاملة  ب رقم تعريف العميل
export const fetchReservedClientById = createAsyncThunk("fetchReservedClientById/booking", async (id) => {
    const resp = await axios.post(variables.BOOKINGS_API + "GetReservedClientById?id="+id)
        .then((res) => res.data);
    return resp;
})
export const deleteBookingData = createAsyncThunk("deleteBookingData/booking", async (clientdata) => {
    const resp = await axios.post(variables.BOOKINGS_API + "DeleteBookingData",clientdata)
        .then((res) => res.data);
    return resp;
})
export const searchBookingClients = createAsyncThunk("searchBookingClients/booking", async (searchdata) => {
    const resp = await axios.post(variables.BOOKINGS_API + "SearchBookings",searchdata)
        .then((res) => res.data);
    return resp;
})
export const searchClients = createAsyncThunk("searchClients/booking", async (searchclient) => {
    const resp = await axios.post(variables.BOOKINGS_API + "SearchClients",searchclient)
        .then((res) => res.data);
    return resp;
})
