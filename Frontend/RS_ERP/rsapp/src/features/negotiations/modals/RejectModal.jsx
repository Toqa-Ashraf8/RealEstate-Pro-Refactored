import React, { useEffect, useRef } from 'react'
import { useDispatch, useSelector } from 'react-redux';
import { 
    resetSelectedRequest,  
    updateAcceptedNegotiationData,  
    updateSelectedRequestData, 
    toggleRejectModal 
} from '../../../assets/redux/negotiationSlice';
import { AiTwotoneEdit } from "react-icons/ai";
import { toast } from 'react-toastify';
import { 
    processNegotiationReview, 
    updateNegotiationStatus 
} from '../../../services/negotiationService';
const RejectModal = () => {
const {
    selectedAcceptedNegotiation,
    selectedRequest,
    CurrentDate,
    rejected
} = useSelector((state) => state.negotiation);
const dispatch = useDispatch();
const suggestedPrice=useRef();

const enableSuggestedPrice=()=>{
    suggestedPrice.current.disabled=false;
    suggestedPrice.current.focus();
}
const handleChange=(e)=>{
    const {name,value}=e.target;
    dispatch(updateSelectedRequestData({[name]:value}));
    dispatch(updateAcceptedNegotiationData({[name]:value}));
}
const closeRejectModal=()=>{
    dispatch(toggleRejectModal(false));
    dispatch(resetSelectedRequest());
}
const confirmRejection=async()=>{
     const row={...selectedAcceptedNegotiation,CheckedDate:CurrentDate}
    try{
    if(rejected===0){
    await dispatch(processNegotiationReview(row)).unwrap();
     toast.error("تم رفض الطلب!", {
        theme: "colored",
        position: "top-left",
      });  
    }
    else if(rejected===1){
      await dispatch(updateNegotiationStatus(selectedAcceptedNegotiation)).unwrap();
       toast.error("تم إعادة رفض الطلب!", {
        theme: "colored",
        position: "top-left",
      });  
    }
      dispatch(toggleRejectModal(false)); 
    }       
    catch (error) {
        console.error("خطأ في العملية:", error);
        toast.warning("حدث خطأ، يرجى التأكد من البيانات");
    } 
}

  return (
             <div>
                <div className="modal-backdrop">
                    <div className="glass-modal animate__animated animate__zoomIn animate__faster">
                        <div className="modal-header">
                            <h3>رفض عرض العميل</h3>
                            <p>{selectedAcceptedNegotiation.ClientName} - {selectedAcceptedNegotiation.ProjectName}</p>
                        </div>
                        <div className="modal-inputs">
                            <div className="field" style={{display:'flex'}} >
                                <label>السعر البديل المقترح</label>
                                <div>
                                    <input 
                                    type="text" 
                                    disabled
                                    placeholder="أدخل مبلغ السعر الجديد"
                                    name='SuggestedPrice'
                                    value={selectedRequest.SuggestedPrice || ""}
                                    onChange={handleChange}
                                    style={{width:'250px'}}
                                    ref={suggestedPrice}
                                />  
                                 <span>
                                    <AiTwotoneEdit 
                                    size={25} color='teal' 
                                    style={{cursor:'pointer',marginRight:'20px'}}
                                    onClick={()=>enableSuggestedPrice()}
                                    />
                                    </span>
                                </div>
                               
                            </div>
                            <div className="field">
                                <label>سبب الرفض</label>
                                <textarea 
                                    rows="3" 
                                    placeholder="لماذا تم رفض هذا السعر؟"
                                    name='ReasonOfReject'
                                    value={selectedRequest.ReasonOfReject || ""}
                                    onChange={handleChange}
                                ></textarea>
                            </div>
                             
                        </div>
                        <div className="modal-actions">
                            <button 
                            className="confirm-btn"
                            onClick={()=>confirmRejection()}
                            >تأكيد الرفض</button>
                            <button className="cancel-btn" 
                            onClick={() =>closeRejectModal()}>إلغاء</button>
                        </div>
                    </div>
                </div>
    </div>
  )
}

export default RejectModal