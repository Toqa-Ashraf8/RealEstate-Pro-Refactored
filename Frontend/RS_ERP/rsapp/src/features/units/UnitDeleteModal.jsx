import React from 'react';
import { MdClose, MdWarning, MdDelete } from "react-icons/md";
import './UnitDeleteModal.css';
import {  deleteUnitFromList, hideDeleteUnitModal } from '../../assets/redux/projectSlice';
import { useDispatch, useSelector } from 'react-redux';

const UnitDeleteModal = () => {
  const{isLoading}=useSelector((state)=>state.ui);
  const dispatch = useDispatch();

  return (
    <div className="row-modal-overlay">
      <div className="row-modal-card">
        
        <div className="row-modal-close" 
        onClick={()=>dispatch(hideDeleteUnitModal())}
        >
          <MdClose />
        </div>

        <div className="row-modal-content">
          <div className="row-modal-icon">
            <MdWarning />
          </div>
          <p className="row-modal-text">
            هل أنت متأكد من عملية الحذف؟
          </p>
        </div>

        <div className="row-modal-btns">
          <button className="row-btn-cancel" 
          onClick={()=>dispatch(hideDeleteUnitModal())}
          >
            لا
          </button>
          <button className="row-btn-confirm"
          disabled={isLoading}
          onClick={()=>dispatch(deleteUnitFromList())}
          >
            <MdDelete /> نعم
          </button>
        </div>

      </div>
    </div>
  );
};

export default UnitDeleteModal;