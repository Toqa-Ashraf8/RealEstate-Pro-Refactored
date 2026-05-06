import React, { useState } from 'react';
import './ConfirmModal.css';
import { useDispatch, useSelector } from 'react-redux';
import {toast} from 'react-toastify'
import { toggleConfirmModal } from '../../../assets/redux/negotiationSlice';
import { 
  processNegotiationReview, 
  updateNegotiationStatus 
} from '../../../services/negotiationService';

const ConfirmModal = () => {
const {
  selectedRejectedNegotiation,
  selectedRequest,
  CurrentDate,
  rejected
} = useSelector((state) => state.negotiation);
const dispatch = useDispatch();

    
const acceptRequest = async () => {
  const dataSource = (rejected === 1) ? selectedRejectedNegotiation : selectedRequest;
 const acceptedrow = {...dataSource,CheckedDate:CurrentDate};

    try {
    if (rejected === 0) {
      await dispatch(processNegotiationReview(acceptedrow)).unwrap();
      toast.success("تم قبول الطلب!",{
        theme:'colored'
      });
    } 
    else if (rejected === 1) {
      await dispatch(updateNegotiationStatus(dataSource)).unwrap();
      toast.success("تم تحديث الطلب!",{
        theme:'colored'
      }
      );
    }
    dispatch(toggleConfirmModal(false));
  } 
  catch (error) {
    toast.warning("حدث خطأ أثناء الحفظ");
  }    
 
};

  return (
    <div className="modal-o">
      <div className="modal-container">
        <p className="modal-message">هل أنت متأكد من قبول هذا الطلب؟</p>
        <div className="modal-actions">
          <button className="btn btn-danger" 
          onClick={()=>dispatch(toggleConfirmModal(false))}>لا</button>
          <button className="btn-yes" onClick={()=>acceptRequest()}>نعم</button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmModal;