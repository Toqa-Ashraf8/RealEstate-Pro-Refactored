import React, { useEffect } from 'react';
import { RotateCcw, User, Tag, CalendarCheck } from 'lucide-react';
import './AcceptedNegotiations.css';
import { useDispatch, useSelector } from 'react-redux';
import { 
    prepareApproveAction,
    prepareRejectAction,
    selectAcceptedForUpdate, 
    toggleRejectModal 
} from '../../../assets/redux/negotiationSlice';
import RejectModal from '../modals/RejectModal';
import { fetchApprovedNegotiations } from '../../../services/negotiationService';

const AcceptedNegotiations = () => {
const {isRejectModalOpen,acceptedRequests} = useSelector((state) => state.negotiation);
const dispatch = useDispatch();

const rejectApprovedRequest=(index)=>{
    dispatch(selectAcceptedForUpdate(index));
    dispatch(prepareRejectAction(1));
    dispatch(toggleRejectModal(true));
}
    useEffect(() => {
        const fetchData = async () => {
            await dispatch(fetchApprovedNegotiations());
        }
        fetchData();
    }, [dispatch]);
console.log("acceptedRequests",acceptedRequests)

    return (
        <div className="acc-page-wrapper">
             {isRejectModalOpen && <RejectModal/>}
            <div className="acc-header">
                <h2>قائمة الطلبات المقبولة</h2>
            </div>

            <div className="acc-list-container">
                {acceptedRequests.map((req, index) => (
                    <div 
                    key={index} className="acc-item-row"
                    >
                        <div className="acc-section acc-client-section">
                            <div className="acc-avatar"><User size={16}/></div>
                            <div>
                                <h4>{req.ClientName}</h4>
                                <span>كود: {req.ClientID}</span>
                            </div>
                        </div>

                
                        <div className="acc-section acc-unit-section">
                            <p><Tag size={12} /> {req.ProjectName} - {req.unitName}</p>
                            <p className="acc-price">العرض: {req.NegotiationPrice}</p>
                        </div>
                        <div className="section date-section">
                            <p><CalendarCheck size={12} /> تاريخ الطلب</p>
                            <p className="rejected-date">
                            {req.NegotiationDate ? req.NegotiationDate.split('T')[0] : ""}
                            </p>
                        </div>
                    
                        <div className="acc-section acc-date-section">
                            <p><CalendarCheck size={12} /> تاريخ القبول</p>
                            <p className="acc-date-text">
                            {req.CheckedDate ? req.CheckedDate.split('T')[0] : ""}
                            </p>
                        </div>
                        <div className="acc-section acc-action-section">
                            <button 
                            className="btn btn-danger" 
                            style={{width:'120px',height:'30px',fontSize:'13px'}}
                            onClick={()=>rejectApprovedRequest(index)}
                            >
                                استرجاع للرفض
                            </button>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default AcceptedNegotiations;