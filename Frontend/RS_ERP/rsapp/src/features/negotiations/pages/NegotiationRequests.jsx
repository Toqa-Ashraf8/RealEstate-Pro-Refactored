import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { 
    setNegotiationStatus, 
    prepareApproveAction, 
    setClientRequestData, 
    toggleConfirmModal, 
    toggleRejectModal 
} from '../../../assets/redux/negotiationSlice';
import { IoMdCheckmarkCircleOutline } from "react-icons/io";
import { CgCloseR } from "react-icons/cg";
import 'animate.css';
import RejectModal from '../modals/RejectModal';
import './NegotiationRequests.css'
import { MoveLeft } from 'lucide-react';
import ConfirmModal from '../modals/ConfirmModal';
import { useNavigate } from 'react-router-dom';
import { 
    fetchApprovedNegotiations, 
    fetchPendingCount, 
    fetchPendingNegotiations, 
    fetchRejectedNegotiations 
} from '../../../services/negotiationService';

const NegotiationRequests = () => {
const {
        isRejectModalOpen,
        isConfirmModalOpen,
        pendingCount,
        rejectedCount,
        approvedCount,
        pendingRequests
} = useSelector((state) => state.negotiation);
const dispatch = useDispatch();
const navigate=useNavigate();

const rejectRequest=(i)=>{
    dispatch(toggleRejectModal(true));
    dispatch(setClientRequestData(i));
    dispatch(prepareApproveAction(1));
    dispatch(setNegotiationStatus(0));     
}

const acceptRequest=(i)=>{
     dispatch(setClientRequestData(i));
     dispatch(prepareApproveAction(0));
     dispatch(setNegotiationStatus(1));
     dispatch(toggleConfirmModal(true));
} 
useEffect(() => {
      const Fetch=async()=>{
        try {
           await dispatch(fetchPendingCount());
           await dispatch(fetchPendingNegotiations());
           await dispatch(fetchRejectedNegotiations());
           await dispatch(fetchApprovedNegotiations());

        } catch (error) {
          console.log("فشل في جلب البيانات",error)
        }
      }
      Fetch();
    }, [dispatch]);

 return ( 
        <div className="dashboard-wrapper animate__animated animate__fadeIn">
          {isRejectModalOpen && <RejectModal/>}
          {isConfirmModalOpen && <ConfirmModal/>}
            <div className="custom-glass-header">
                <div className="title-section">
                    <h3>طلبات التفاوض / الشراء <span className="badge-count">
                    {pendingCount || 0}</span>
                    </h3>
                </div>
                <div className="status-container" style={{cursor:'pointer'}}>
                    <div 
                    className="stat-item reject"
                    onClick={()=>navigate('/rejected_negotiations')}
                    >مرفوض: {rejectedCount ||0}</div>
                    <div 
                    className="stat-item approve"
                     onClick={()=>navigate('/accepted_negotiations')}
                    >مقبول: {approvedCount || 0}</div>
                </div>
            </div>


            <div className="table-responsive-glass">
                <table className="modern-table">
                    <thead>
                        <tr>
                            <th>مقدم الطلب</th>
                            <th>كود العميل</th>
                            <th>العميل</th>
                            <th>المشروع / الوحدة</th>
                            <th>التحليل المالي</th>
                            <th>التاريخ</th>
                            <th>الإجراءات</th>
                        </tr>
                    </thead>
                    <tbody>
                        {pendingRequests?.map((neg, index) => (
                            <tr key={index}  className="animate__animated animate__fadeInUp" 
                            style={{ animationDelay: `${index * 0.1}s` }}
                            >
                                <td>{neg.Requester}</td>
                                <td>{neg.ClientID}</td>
                                <td className="font-bold">{neg.ClientName}</td>
                                <td>
                                    <div className="unit-info">
                                        <span>{neg.ProjectName}</span>
                                        <span>/</span>
                                        <small>{neg.unitName}</small>
                                    </div>
                                </td>
                                <td>
                                    <div 
                                    className="price-tag-container" 
                                    style={{display:'flex',justifyContent:'center'}}>
                                        <span 
                                        className="price-old" style={{fontSize:'16px'}}>
                                        {neg.OriginalPrice}
                                        </span>
                                        <span>  <MoveLeft size={25}/></span>
                                        <span className="price-new">{neg.NegotiationPrice}</span>
                                    </div>
                                </td>
                                <td>{neg.NegotiationDate?.split('T')[0]}</td>
                                <td>
                                    <div className="btns-group"
                                    style={{display:'flex',justifyContent:'center',gap:'20px'}}>
                                        <button 
                                        className="action-btn accept-btn" title="قبول"
                                         onClick={()=>acceptRequest(index)}
                                        >
                                            <IoMdCheckmarkCircleOutline size={22} />
                                        </button>
                                        <button 
                                            className="action-btn reject-btn" 
                                            title="رفض"
                                           onClick={() =>rejectRequest(index)}
                                        >
                                            <CgCloseR size={22} />
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default NegotiationRequests;