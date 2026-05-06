import React from 'react';
import { MdClose, MdWarning, MdDelete } from "react-icons/md";
import './ClientsDeleteModal.css';
import { useDispatch, useSelector } from 'react-redux';
import { toast } from 'react-toastify';
import { deleteClientData } from '../../../services/clientService';
import { resetClientForm, toggleDeleteClientModal } from '../../../assets/redux/clientSlice';

const ClientsDeleteModal = () => {
  const {client,deletedStatus} = useSelector((state) => state.clients);
  const {isLoading} =useSelector((state)=>state.ui);
  const dispatch = useDispatch();

  const handleDelete=async()=>{
   try {
      const result = await dispatch(deleteClientData(client.ClientID)).unwrap();
   } 
   catch (error) {
      toast.error("حدث خطأ في الاتصال بالخادم", {
         theme: "colored",
          position: "top-left",
       });
    }
      await dispatch(toggleDeleteClientModal(false));
      await dispatch(resetClientForm());
  }
  return (
    <div className="delete-client-wrapper">
          <div className="delete-client-card">
            <span className="delete-client-close-btn" 
             onClick={()=>dispatch(toggleDeleteClientModal(false))}>
            <span><MdClose /></span>
            </span>
    
            <div className="delete-client-content">
              <div className="delete-icon-box-c">
                <MdWarning />
              </div>
              <h2 className="delete-client-title">تنبيه الحذف</h2>
              <p className="delete-client-description">
                هل أنت متأكد من حذف جميع بيانات هذا العميل؟
                <br />
                <span className="client-id">كود العميل: {client?.ClientID}</span>
              </p>
            </div>
    
            <div className="delete-client-actions">
               <button  
               disabled={isLoading} 
               className="btn-action-c btn-yes-c" 
               onClick={handleDelete}>
                <MdDelete /> نعم، متأكد
              </button>
              <button 
              disabled={isLoading} 
              className="btn-action-c btn-no" 
              onClick={()=>dispatch(toggleDeleteClientModal(false))}>
                لا، إلغاء
              </button>
            </div>
    
          </div>
        </div>
  );
};

export default ClientsDeleteModal;