import React from 'react';
import { MdClose, MdWarning, MdDelete } from "react-icons/md";
import './DeleteNegotiationModal.css';
import { useDispatch, useSelector } from 'react-redux';
import {
  confirmDeleteNegotiation,
  toggleDeleteNegotiaionModal
} from "../../../assets/redux/clientSlice"
const DeleteNegotiationModal = () => {
  const {isLoading}=useSelector((state)=>state.ui);
  const dispatch = useDispatch();

  return (
    <div className="neg-row-modal-overlay">
      <div className="neg-row-modal-card"> 
        <span className="neg-row-modal-close" 
        onClick={()=>dispatch(toggleDeleteNegotiaionModal(false))}>
          <span><MdClose /></span>
        </span>

        <div className="neg-row-modal-content">
          <div className="neg-row-modal-icon">
            <MdWarning />
          </div>
          <p className="neg-row-modal-text">
            هل أنت متأكد من عملية الحذف؟
          </p>
        </div>

        <div className="neg-row-modal-btns">
          <button className="neg-row-btn-cancel" 
          disabled={isLoading}
          onClick={()=>dispatch(toggleDeleteNegotiaionModal(false))}>
            لا
          </button>
           <button className="neg-row-btn-confirm"
            disabled={isLoading}
            onClick={()=>dispatch(confirmDeleteNegotiation())}>
            <MdDelete /> نعم
          </button>
        </div>

      </div>
    </div>
  );
};

export default DeleteNegotiationModal;