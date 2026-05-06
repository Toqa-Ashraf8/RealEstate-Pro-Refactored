import React, { useEffect } from 'react';
import { 
    RotateCcw, 
    User, 
    Tag, 
    Info, 
    CalendarCheck 
} from 'lucide-react';
import './RejectedNegotiations.css';
import { useDispatch, useSelector } from 'react-redux';
import 
{ 
    prepareRejectAction, 
    selectRejectedForUpdate, 
    toggleConfirmModal, 
} from'../../../assets/redux/negotiationSlice'; 
import ConfirmModal from '../modals/ConfirmModal';
import { fetchRejectedNegotiations } from '../../../services/negotiationService';

const RejectedNegotiations = () => {
const {isConfirmModalOpen,rejectedRequests} = useSelector((state) => state.negotiation);
const dispatch = useDispatch();

const reapproveRequest=(index)=>{
    dispatch(selectRejectedForUpdate(index))
    dispatch(dispatch(prepareRejectAction(1)));
    dispatch(toggleConfirmModal(true));
}
useEffect(() => {
    const fetchData = async () => {
    await dispatch(fetchRejectedNegotiations());
    }
    fetchData();
}, [dispatch]);
console.log("rejectedRequests",rejectedRequests)
    return (
        <div className="clean-page-wrapper">
           {isConfirmModalOpen && <ConfirmModal/>} 
            <div className="clean-header">
                <h2>قائمة الطلبات المستبعدة</h2>
            </div>

            <div className="clean-list-container">
                {rejectedRequests.map((req, index) => (
                    <div key={index} className="clean-item-row">
                
                        <div className="section client-section">
                            <div className="clean-avatar"><User size={16} /></div>
                            <div>
                                <h4>{req.ClientName || ""}</h4>
                                <span>كود: {req.ClientID || ""}</span>
                            </div>
                        </div>

                        <div className="section unit-section">
                            <p><Tag size={12} /> {req.ProjectName || ""} - {req.unitName || ""}</p>
                            <p className="rejected-price_r">العرض: {req.NegotiationPrice || ""}</p>
                        </div>

                        <div className="section reason-section">
                            <div className="reason-label"><Info size={12} /> سبب الرفض:</div>
                            <p>{req.ReasonOfReject || ""}</p>
                        </div>

                        <div className="section date-section">
                            <p><CalendarCheck size={12} /> تاريخ الطلب</p>
                            <p className="rejected-date">
                            {req.NegotiationDate ? req.NegotiationDate.split('T')[0] : ""}
                            </p>
                        </div>

                        <div className="section date-section">
                            <p><CalendarCheck size={12} /> تاريخ الرفض</p>
                            <p className="rejected-date">
                             {req.CheckedDate ? req.CheckedDate.split('T')[0] : ""}
                             </p>
                        </div>

                        <div className="section action-section">
                          <button 
                          className="btn btn-success" 
                          style={{width:'120px',height:'30px',fontSize:'13px'}}
                          onClick={()=>reapproveRequest(index)}
                          >
                                استرجاع للقبول
                            </button>
                        </div>
                        
                    </div>
                ))}
            </div>
        </div>
    );
};

export default RejectedNegotiations;